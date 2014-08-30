using System;
using System.IO;
using System.Text;
using System.Linq;
using Rice.Server.Core;
using Rice.Game;
using System.Net;

namespace Rice.Server.Packets.Auth
{
    public static class Authentication
    {
        [RicePacket(20, RiceServer.ServerType.Auth)]
        public static void UserAuth(RicePacket packet)
        {
            packet.Reader.ReadInt32(); // Skip 4 bytes.
            var username = packet.Reader.ReadUnicodeStatic(40);
            var password = packet.Reader.ReadASCII();

            string pwhash = Utilities.MD5(password.Substring(0, password.Length - 1));

            User user = User.Retrieve(username, pwhash);

            if (user == null || user.Status == UserStatus.Invalid)
            {
                Log.WriteLine("Attempt to log into non-existant account or use invalid password.");

                var invalid = new RicePacket(22);

                invalid.Writer.Write(0);
                invalid.Writer.Write(1);
                invalid.Writer.Write(new byte[68]);

                packet.Sender.Send(invalid);
                return;
            }
            else if (user.Status == UserStatus.Banned)
            {
                Log.WriteLine("Attempt to log into suspended account {0}.", user.Name);

                packet.Sender.Error("Your account has been suspended.");
                packet.Sender.Kill();
                return;
            }

            Player player = new Player(user);
            player.AuthClient = packet.Sender;
            player.Ticket = RiceServer.CreateTicket();
            RiceServer.AddPlayer(player);

            var ack = new RicePacket(22);

            ack.Writer.Write(player.Ticket); // Ticket
            ack.Writer.Write(0); // Auth Result

            ack.Writer.Write(Environment.TickCount); // Time
            ack.Writer.Write(new byte[64]); // Filler ("STicket")

            ack.Writer.Write((ushort)23); // ServerList ID
            ack.Writer.Write(1); // Server Count

            ack.Writer.WriteUnicodeStatic("Rice Emulator", 32); // Server Name
            ack.Writer.Write(1); // Server ID

            ack.Writer.Write((float)RiceServer.GetPlayers().Length); // Player Count
            ack.Writer.Write(7000f); // Max Player Count

            ack.Writer.Write(1); // Server State

            ack.Writer.Write(Environment.TickCount); // Game Update Time
            ack.Writer.Write(Environment.TickCount); // Lobby Update Time
            ack.Writer.Write(Environment.TickCount); // Area1 Update Time
            ack.Writer.Write(Environment.TickCount); // Area2 Update Time
            ack.Writer.Write(Environment.TickCount); // Ranking Update Time

            byte[] ip = IPAddress.Parse(RiceServer.Config.PublicIP).GetAddressBytes();
            ack.Writer.Write(ip); // GameServer IP
            ack.Writer.Write(ip); // LobbyServer IP
            ack.Writer.Write(ip); // AreaServer 1 IP
            ack.Writer.Write(ip); // AreaServer 2 IP
            ack.Writer.Write(ip); // Ranking IP

            ack.Writer.Write(RiceServer.Config.GamePort); // GameServer Port
            ack.Writer.Write(RiceServer.Config.LobbyPort); // LobbyServer Port
            ack.Writer.Write(RiceServer.Config.AreaPort); // AreaServer 1 Port
            ack.Writer.Write((ushort)11041); // AreaServer 2 Port
            ack.Writer.Write((ushort)10701); // AreaServer 1 UDP Port
            ack.Writer.Write((ushort)10702); // AreaServer 2 UDP Port
            ack.Writer.Write(RiceServer.Config.RankingPort); // Ranking Port

            ack.Writer.Write((ushort)0); // what

            packet.Sender.Send(ack);
        }
    }
}
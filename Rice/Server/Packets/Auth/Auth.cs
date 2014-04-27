using System;
using System.IO;
using System.Text;
using System.Linq;
using Rice.Server.Core;

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

            Log.WriteLine("UserAuth: ID: {0}, Password: {1}", username, password);

            var ack = new RicePacket(22);

            ack.Writer.Write(new Random().Next(int.MaxValue)); // Ticket
            ack.Writer.Write(0); // Auth Result (0 is Success, 2 is banned)

            ack.Writer.Write(Environment.TickCount); // Time
            ack.Writer.Write(new byte[64]); // Filler ("STicket")

            ack.Writer.Write((ushort)23); // ServerList ID
            ack.Writer.Write(1); // Server Count

            ack.Writer.WriteUnicodeStatic("TestServer", 32); // Server Name
            ack.Writer.Write(1); // Server ID

            ack.Writer.Write((float)RiceServer.Game.GetClients().Length + 1f); // Player Count
            ack.Writer.Write(7000f); // Max Player Count

            ack.Writer.Write(1); // Server State

            ack.Writer.Write(Environment.TickCount); // Game Update Time
            ack.Writer.Write(Environment.TickCount); // Lobby Update Time
            ack.Writer.Write(Environment.TickCount); // Area1 Update Time
            ack.Writer.Write(Environment.TickCount); // Area2 Update Time
            ack.Writer.Write(Environment.TickCount); // Ranking Update Time

            // 127.0.0.1 for local testing
            ack.Writer.Write(2130706433u); // GameServer IP
            ack.Writer.Write(2130706433u); // LobbyServer IP
            ack.Writer.Write(2130706433u); // AreaServer 1 IP
            ack.Writer.Write(2130706433u); // AreaServer 2 IP
            ack.Writer.Write(2130706433u); // Ranking IP

            ack.Writer.Write((ushort)11021); // GameServer Port
            ack.Writer.Write((ushort)11011); // LobbyServer Port
            ack.Writer.Write((ushort)11031); // AreaServer 1 Port
            ack.Writer.Write((ushort)11041); // AreaServer 2 Port
            ack.Writer.Write((ushort)10701); // AreaServer 1 UDP Port
            ack.Writer.Write((ushort)10702); // AreaServer 2 UDP Port
            ack.Writer.Write((ushort)11078); // Ranking Port

            ack.Writer.Write((ushort)0); // what

            packet.Sender.Send(ack);
        }
    }
}
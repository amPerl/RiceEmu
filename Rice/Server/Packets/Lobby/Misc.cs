using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rice.Game;
using Rice.Server.Core;

namespace Rice.Server.Packets.Lobby
{
    public static class Misc
    {
        [RicePacket(41, RiceServer.ServerType.Lobby)]
        public static void CheckInLobby(RicePacket packet)
        {
            Log.WriteLine("CheckInLobby request.");

            uint version = packet.Reader.ReadUInt32();
            uint ticket = packet.Reader.ReadUInt32();
            string username = packet.Reader.ReadUnicodeStatic(0x28);
            uint time = packet.Reader.ReadUInt32();
            string stringTicket = packet.Reader.ReadASCIIStatic(0x40);

            var serverTicket = RiceServer.GetTicket(ticket);

            if (serverTicket == null || !serverTicket.ValidateOrigin(packet.Sender, username))
            {
#if DEBUG
                packet.Sender.Player = new Player(Rice.Game.User.Retrieve(username));
                RiceServer.AddPlayer(packet.Sender.Player);
                serverTicket = RiceServer.CreateDebugTicket(packet.Sender, ticket);
#else
                Log.WriteLine("Ticket is non-existent or invalid for current user.");
                packet.Sender.Error("water u even doin");
                return;
#endif
            }
            else
            {
                packet.Sender.Player = serverTicket.GetOwner();
            }


            var ack = new RicePacket(42); // CheckInLobbyAck
            ack.Writer.Write(ticket); // Ticket
            ack.Writer.Write(0); // Permission ???
            packet.Sender.Send(ack);

            var timeAck = new RicePacket(47); // LobbyTimeAck
            timeAck.Writer.Write(Environment.TickCount);
            timeAck.Writer.Write(Environment.TickCount);
            packet.Sender.Send(timeAck);

            Log.WriteLine("User {0} entered lobby.", username);
        }
    }
}

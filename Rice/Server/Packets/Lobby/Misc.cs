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

            Player player = null;
            bool verifyTicket = true;

#if DEBUG
            verifyTicket = false;
            player = new Player(Rice.Game.User.Retrieve(username));
#else

            foreach (var p in RiceServer.GetPlayers())
            {
                if (p.User.Username == username && p.Ticket == ticket)
                {
                    player = p;
                    player.LobbyClient = packet.Sender;
                    break;
                }
            }
#endif

            if (player == null)
            {
                Log.WriteLine("Rejecting {0} (ticket {1}) for invalid user-ticket combination.", username, ticket);
                packet.Sender.Error("Invalid ticket-user combination.");
                return;
            }

            var ack = new RicePacket(42); // CheckInLobbyAck
            ack.Writer.Write(0); // Result
            ack.Writer.Write(0); // Permission
            packet.Sender.Send(ack);

            var timeAck = new RicePacket(47); // LobbyTimeAck
            timeAck.Writer.Write(Environment.TickCount);
            timeAck.Writer.Write(Environment.TickCount);
            packet.Sender.Send(timeAck);

            Log.WriteLine("User {0} entered lobby.", username);
        }
    }
}

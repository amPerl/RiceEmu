using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rice.Game;
using Rice.Server.Core;

namespace Rice.Server.Packets.Game
{
    public static class Chat
    {
        [RicePacket(146, RiceServer.ServerType.Game)]
        public static void ChatMessage(RicePacket packet)
        {
            string type = packet.Reader.ReadUnicodeStatic(10);
            bool green = packet.Reader.ReadUInt32() == 0xFF00FF00; // ignore this, use packet.Sender.Player.User.Status
            string message = packet.Reader.ReadUnicodePrefixed();

            string sender = packet.Sender.Player.ActiveCharacter.Name;

#if DEBUG
            if (message.ToLower().StartsWith("gibe_plos "))
            {
                string[] split = message.Split(' ');
                if (split.Length == 2)
                {
                    Item item;
                    if (packet.Sender.Player.ActiveCharacter.GrantItem(split[1], 1, out item))
                    {
                        packet.Sender.Error($"Gave 1x {item.itemEntry.Name}");
                        Log.WriteLine($"Gave {sender} 1x {item.itemEntry.Name}");

                        packet.Sender.Player.ActiveCharacter.FlushModInfo(packet.Sender);
                        return;
                    }

                    packet.Sender.Error($"Failed to give item");
                    return;
                }
            }
#endif

            Console.WriteLine("({0}) <{1}> {2}", type, sender, message);

            var ack = new RicePacket(147);
            ack.Writer.WriteUnicodeStatic(type, 10);
            ack.Writer.WriteUnicodeStatic(sender, 18);
            ack.Writer.WriteUnicode(message);

            switch (type)
            {
                case "room":
                    RiceServer.Game.Broadcast(ack); // TODO: broadcast only to users in same area
                    break;

                case "channel":
                    RiceServer.Game.Broadcast(ack);
                    break;

                case "party":
                    RiceServer.Game.Broadcast(ack); // TODO: broadcast only to users in same party
                    break;

                case "team":
                    RiceServer.Game.Broadcast(ack); // TODO: broadcast only to users in same crew
                    break;

                default:
                    Log.WriteError("Undefined chat message type.");
                    break;
            }
        }
    }
}

using Rice.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rice.Server.Packets.Area
{
    public class Chat
    {
        [RicePacket(571, RiceServer.ServerType.Area)]
        public static void AreaChat(RicePacket packet)
        {
            string type = packet.Reader.ReadUnicodeStatic(10);
            packet.Reader.ReadBytes(18 * 2); // sender 18 chars max
            string message = packet.Reader.ReadUnicodePrefixed();

            string sender = packet.Sender.Player.ActiveCharacter.Name;

            var ack = new RicePacket(571);
            ack.Writer.WriteUnicodeStatic(type, 10);
            ack.Writer.WriteUnicodeStatic(sender, 18);
            ack.Writer.WriteUnicode(message);

            switch (type)
            {
                case "area":
                    packet.Sender.Player.ActiveCharacter.Area.Broadcast(ack);
                    break;

                default:
                    Log.WriteError("Undefined chat message type.");
                    break;
            }
        }
    }
}

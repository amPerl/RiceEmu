using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rice.Server.Core;

namespace Rice.Server.Packets.Game
{
    public static class Misc
    {
        [RicePacket(120, RiceServer.ServerType.Game)]
        public static void CheckInGame(RicePacket packet)
        {
            Log.WriteLine("CheckInGame");

            // TODO: Verify

            var ack = new RicePacket(121);
            ack.Writer.Write(1L);
            packet.Sender.Send(ack);
        }

        [RicePacket(125, RiceServer.ServerType.Game)]
        public static void JoinChannel(RicePacket packet)
        {
            var ack = new RicePacket(126);

            ack.Writer.WriteUnicodeStatic("speeding", 10);
            ack.Writer.WriteUnicodeStatic("charName", 16);
            ack.Writer.Write((ushort)123); // Serial
            ack.Writer.Write((ushort)123); // Age

            packet.Sender.Send(ack);
        }

        [RicePacket(0x30E, RiceServer.ServerType.Game)]
        public static void FirstPosition(RicePacket packet)
        {
            var ack = new RicePacket(0x30F);

            ack.Writer.Write(0L);
            ack.Writer.Write(0L);
            ack.Writer.Write(0L);
            ack.Writer.Write(3);

            packet.Sender.Send(ack);
        }
    }
}

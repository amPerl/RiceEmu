using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rice.Server.Core;

namespace Rice.Server.Packets.Area
{
    public static class Misc
    {
        [RicePacket(7, RiceServer.ServerType.Area | RiceServer.ServerType.Game | RiceServer.ServerType.Lobby | RiceServer.ServerType.Auth)]
        public static void NullPing(RicePacket packet) { }

        [RicePacket(540, RiceServer.ServerType.Area)]
        public static void TimeSync(RicePacket packet)
        {
            var ack = new RicePacket(0x21C);
            ack.Writer.Write(packet.Reader.ReadUInt32());
            ack.Writer.Write(0);
            packet.Sender.Send(ack);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rice.Server.Core;
using Rice.Server.Structures;

namespace Rice.Server.Packets.Game
{
    public static class Area
    {
        [RicePacket(782, RiceServer.ServerType.Game)]
        public static void FirstPosition(RicePacket packet)
        {
            var ack = new RicePacket(0x30F);
            ack.Writer.Write(0L);
            ack.Writer.Write(0L);
            ack.Writer.Write(0L);
            ack.Writer.Write(3);
            packet.Sender.Send(ack);
        }

        [RicePacket(788, RiceServer.ServerType.Game)]
        public static void MyCityPosition(RicePacket packet)
        {
            Log.WriteLine(BitConverter.ToString(packet.Buffer));

            int channelId = packet.Reader.ReadInt32();

            var ack = new RicePacket(783);
            var charInfo = packet.Sender.Player.ActiveCharacter.GetInfo();
            ack.Writer.Write(charInfo.City);
            ack.Writer.Write(channelId);

            ack.Writer.Write(charInfo.Position); // Position
            ack.Writer.Write(charInfo.PosState); // PositionState, 0 for MoonPalace introduction

            ack.Writer.Write((byte)0);
            packet.Sender.Send(ack);
        }

        [RicePacket(162, RiceServer.ServerType.Game)]
        public static void SaveCarPos(RicePacket packet)
        {
            var channelId = packet.Reader.ReadInt32();
            var position = packet.Reader.ReadVector4();
            int cityId = packet.Reader.ReadInt32();
            int posState = packet.Reader.ReadInt32();

            packet.Sender.Player.ActiveCharacter.SaveCarPos(channelId, position, cityId, posState);
        }

        [RicePacket(300, RiceServer.ServerType.Game)]
        public static void JoinArea(RicePacket packet)
        {
            var ack = new RicePacket(301);
            ack.Writer.Write(packet.Reader.ReadUInt32());
            ack.Writer.Write(1);
            packet.Sender.Send(ack);
        }

        [RicePacket(780, RiceServer.ServerType.Game)]
        public static void AreaList(RicePacket packet)
        {
            // client calls 2 functions (not using any packet data), returns  137 * (*(_DWORD *)(pktBuf + 2) - 1) + 143;
            var ack = new RicePacket(781);
            ack.Writer.Write(1);
            ack.Writer.Write(new byte[137]);
            packet.Sender.Send(ack);
        }

        [RicePacket(3200, RiceServer.ServerType.Game)]
        public static void CityLeave(RicePacket packet)
        {
            var ack = new RicePacket(3201);
            ack.Writer.Write(1); // if ( *(_DWORD *)(pktBuf + 2) == 1 )
            ack.Writer.Write(packet.Reader.ReadBytes(514)); // apparently these fuckers want their own 514 bytes back
            packet.Sender.Send(ack);
        }
    }
}

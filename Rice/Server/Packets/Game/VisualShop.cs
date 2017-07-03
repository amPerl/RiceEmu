using Rice.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rice.Game;
using Rice.Server.Structures.Resources;

namespace Rice.Server.Packets.Game
{
    public static class VisualShop
    {
        [RicePacket(1400, RiceServer.ServerType.Game)]
        public static void GetMyHancoin(RicePacket packet)
        {
            var ack = new RicePacket(1401);
            ack.Writer.Write((uint)packet.Sender.Player.User.Credits);
            ack.Writer.Write(0L); // mileage?? masangpluto pls
            packet.Sender.Send(ack);
        }

        [RicePacket(1203, RiceServer.ServerType.Game)]
        public static void BuyVisualItem(RicePacket packet)
        {
            uint tableIdx = packet.Reader.ReadUInt32();
            uint carId = packet.Reader.ReadUInt32();
            string plateName = packet.Reader.ReadUnicodeStatic(20);
            uint periodIdx = packet.Reader.ReadUInt32();
            bool useMileage = packet.Reader.ReadUInt16() > 0;
            long curCash = packet.Reader.ReadInt64(); // don't use this, obviously
            Log.WriteDebug($"BuyVisualItem idx {tableIdx} car {carId} plate {plateName} period {periodIdx} mileage? {useMileage} cash {curCash}");

            // TODO: complete stub
        }

        [RicePacket(1300, RiceServer.ServerType.Game)]
        public static void BuyHistoryList(RicePacket packet)
        {
            uint offset = packet.Reader.ReadUInt32();
            uint count = packet.Reader.ReadUInt32();
            uint tab = packet.Reader.ReadUInt32(); // 1 = purchase history, 2 = sent gift, 3 = received gift
            Log.WriteDebug($"BuyHistoryList offset {offset} count {count} tab {tab}");

            // TODO: complete stub
        }

        [RicePacket(1306, RiceServer.ServerType.Game)]
        public static void IsValidCharName(RicePacket packet)
        {
            string charName = packet.Reader.ReadUnicodeStatic(21);
            Log.WriteDebug($"IsValidCharName name {charName}");

            // TODO: complete stub
        }
    }
}

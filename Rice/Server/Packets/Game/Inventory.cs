using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rice.Server.Core;
using Rice.Game;

namespace Rice.Server.Packets.Game
{
    public static class Inventory
    {
        [RicePacket(400, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void ItemList(RicePacket packet)
        {
            // unsigned long CurCarID (which car the item's currently equipped on, if any) (uncertain about CurCarID/LastCarID, check with Coaster later)
            // unsigned short State
            // unsigned short Slot
            // long StackNum
            // unsigned long LastCarID (which car the item was last equipped on, or the car ID associated with the car if the item's a key)
            // unsigned long AssistA
            // unsigned long AssistB
            // unsigned long AssistC
            // unsigned long AssistD
            // unsigned long AssistE
            // unsigned long AssistF
            // unsigned long AssistG
            // unsigned long AssistH
            // unsigned long AssistI
            // unsigned long AssistJ
            // unsigned long Box
            // unsigned long Belonging
            // long Upgrade
            // long UpgradePoint
            // unsigned long ExpireTick
            // float ItemHealth (only applicable to NEO parts, -1 everywhere else)
            // unsigned long unk2 (?, always 0)
            // unsigned long TableIdx
            // unsigned long InvenIdx
            // long Random (random seed, only applies to certain item types)
            var inventory = packet.Sender.Player.ActiveCharacter.Inventory;
            var ack = new RicePacket(401);
            ack.Writer.Write(262144); // ListUpdate (262144 = First packet from list queue, 262145 = sequential)
            ack.Writer.Write(inventory.Count); // ItemNum
            foreach (var item in inventory)
            {
                ack.Writer.Write(item.GetInfo());
            }
            packet.Sender.Send(ack);
        }

        [RicePacket(403, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void DropItem(RicePacket packet)
        {
            var tableIdx = packet.Reader.ReadInt32();
            var invIdx = packet.Reader.ReadInt32();
            var count = packet.Reader.ReadInt32();

            var character = packet.Sender.Player.ActiveCharacter;

            var toDrop = character.Inventory.FirstOrDefault(i => i.invIdx == invIdx && i.tblIdx == tableIdx);
            if (toDrop == null || count < 1)
            {
                packet.Sender.Error("What item?");
                Log.WriteError("What item?");
                return;
            }

            Log.WriteDebug($"{tableIdx} {invIdx} {count} item {toDrop?.ID ?? "NONE"}");
            bool dropped = character.DropItem((int) toDrop.invIdx, out toDrop, (uint) count);
            if (!dropped)
            {
                packet.Sender.Error("Failed to drop item.");
                Log.WriteError("Failed to drop item.");
                return;
            }

            //DropItemAck
            var ack = new RicePacket(404);
            ack.Writer.Write(tableIdx);
            ack.Writer.Write(invIdx);
            ack.Writer.Write(count);
            packet.Sender.Send(ack);

            Log.WriteDebug($"todrop stack {toDrop.stackNum}");

            character.FlushModInfo(packet.Sender);
        }

        [RicePacket(409, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void EquipItem(RicePacket packet)
        {
            var invIdx = packet.Reader.ReadUInt32();
            var destSlotIdx = packet.Reader.ReadUInt32();
            var carId = packet.Reader.ReadUInt32();

            var character = packet.Sender.Player.ActiveCharacter;

            Log.WriteDebug($"equip item {invIdx} slot {destSlotIdx} car {carId}");

            bool equipped = character.EquipItem((int)invIdx, (short)destSlotIdx, (int)carId);
            if (!equipped)
            {
                packet.Sender.Error("could not equip item");
                Log.WriteError($"Could not equip item {invIdx}");
                return;
            }

            var ack = new RicePacket(410);
            ack.Writer.Write(invIdx);
            ack.Writer.Write(destSlotIdx);
            ack.Writer.Write(carId);
            packet.Sender.Send(ack);

            character.FlushModInfo(packet.Sender);
        }

        [RicePacket(411, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void UnEquipItem(RicePacket packet)
        {
            var invIdx = packet.Reader.ReadUInt32();
            var carId = packet.Reader.ReadUInt32();

            var character = packet.Sender.Player.ActiveCharacter;

            Log.WriteDebug($"unequip item {invIdx} car {carId}");

            bool unEquipped = character.UnEquipItem((int)invIdx, (int)carId);
            if (!unEquipped)
            {
                packet.Sender.Error("could not unequip item");
                Log.WriteError($"Could not unequip item {invIdx}");
                return;
            }

            var ack = new RicePacket(412);
            ack.Writer.Write(invIdx);
            ack.Writer.Write(carId);
            packet.Sender.Send(ack);

            character.FlushModInfo(packet.Sender);
        }
    }
}
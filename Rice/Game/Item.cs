using Rice.Server.Core;
using Rice.Server.Structures;
using Rice.Server.Structures.Resources;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models = Rice.Server.Database.Models;

namespace Rice.Game
{
    public class Item
    {
        public uint curCarID;
        public ushort state;
        public ushort slot;
        public uint stackNum;
        public uint lastCarID;
        public uint assistA;
        public uint assistB;
        public uint assistC;
        public uint assistD;
        public uint assistE;
        public uint assistF;
        public uint assistG;
        public uint assistH;
        public uint assistI;
        public uint assistJ;
        public uint box;
        public uint belonging;
        public int upgrade;
        public int upgradePoint;
        public uint expireTick;
        public float itemHealth;
        public int tblIdx;
        public uint invIdx;
        public int random;
        public string ID;
        public ulong CID;
        public GenericItemEntry itemEntry;

        private Item(string id, Models.Item dbItem)
        {
            string lookupId = id;

            if (dbItem != null)
            {
                lookupId = dbItem.ItemID;
                CID = (ulong)dbItem.CID;
                curCarID = (uint)dbItem.CurCarID;
                state = (ushort)dbItem.State;
                slot = (ushort)dbItem.Slot;
                stackNum = (uint)dbItem.StackNum;
                lastCarID = (uint)dbItem.LastCarID;
                assistA = (uint)dbItem.AssistA;
                assistB = (uint)dbItem.AssistB;
                assistC = (uint)dbItem.AssistC;
                assistD = (uint)dbItem.AssistD;
                assistE = (uint)dbItem.AssistE;
                assistF = (uint)dbItem.AssistF;
                assistG = (uint)dbItem.AssistG;
                assistH = (uint)dbItem.AssistH;
                assistI = (uint)dbItem.AssistI;
                assistJ = (uint)dbItem.AssistJ;
                box = (uint)dbItem.Boxed;
                belonging = (uint)dbItem.Bound;
                upgrade = dbItem.UpgradeLevel;
                upgradePoint = dbItem.UpgradePoints;
                expireTick = 0;
                //expireTick = dbItem.ExpireTimestamp;
                itemHealth = dbItem.ItemHealth;
                invIdx = (uint)dbItem.InvIdx;
                random = dbItem.Random;
            }

            tblIdx = ItemTable.Items.FindIndex(i => i.ID == lookupId);
            if (tblIdx == -1)
            {
                itemEntry = null;
                return;
            }
            ID = lookupId;
            itemEntry = ItemTable.Items[tblIdx];
        }

        public static Item FromDB(Models.Item dbItem) => new Item(dbItem.ItemID, dbItem);

        public static Item Retrieve(ulong cid, uint InvIdx)
        {
            using (var rc = Database.GetContext())
            {
                var item = rc.Items.SingleOrDefault(i => i.CID == (long) cid && i.InvIdx == (int) InvIdx);
                return item != null ? new Item(item.ItemID, item) : null;
            }
        }

        public static List<Item> Retrieve(ulong cid)
        {
            using (var rc = Database.GetContext())
                return rc.Items.Where(i => i.CID == (long) cid).ToList().Select(i => new Item(i.ItemID, i)).ToList();
        }

        public static Item CreateEntry(ulong CID, string ID, uint StackNum, float health = 1f, int carId = 0)
        {
            var tblIdx = ItemTable.Items.FindIndex(i => i.ID == ID);
            if (tblIdx < 0)
            {
                Log.WriteError($"Failed to find item id {ID} for Item.CreateEntry");
                return null;
            }

            using (var rc = Database.GetContext())
            {
                var lastItem = rc.Items.Where(i => i.CID == (long) CID).OrderByDescending(i => i.InvIdx).FirstOrDefault();

                var newItem = new Models.Item
                {
                    CID = (long)CID,
                    ItemID = ID,
                    StackNum = (int)StackNum,
                    ItemHealth = health,
                    InvIdx = (lastItem?.InvIdx ?? 0) + 1,
                    CurCarID = carId,
                    LastCarID = carId
                };

                rc.Items.Add(newItem);
                rc.SaveChanges();

                return new Item(newItem.ItemID, newItem);
            }
        }

        public bool IncreaseStackNum()
        {
            using (var rc = Database.GetContext())
            {
                var item = rc.Items.Find(invIdx);
                if (item == null)
                {
                    Log.WriteError($"Couldn't find item invidx {invIdx} for Item.IncreaseStackNum");
                    return false;
                }

                ++stackNum;
                item.StackNum = (int)stackNum;

                rc.SaveChanges();
                return true;
            }
        }

        public ItemInfo GetInfo()
        {
            return new ItemInfo
            {
                CurCarID = curCarID,
                State = state,
                Slot = slot,
                StackNum = stackNum,
                LastCarID = lastCarID,
                AssistA = assistA,
                AssistB = assistB,
                AssistC = assistC,
                AssistD = assistD,
                AssistE = assistE,
                AssistF = assistF,
                AssistG = assistG,
                AssistH = assistH,
                AssistI = assistI,
                AssistJ = assistJ,
                Box = box,
                Belonging = belonging,
                Upgrade = upgrade,
                UpgradePoint = upgradePoint,
                ExpireTick = expireTick,
                ItemHealth = itemHealth,
                TableIdx = (uint) tblIdx,
                InvenIdx = invIdx,
                Random = 0
            };
        }
    }
}

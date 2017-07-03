using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Rice.Server.Core;
using Rice.Server.Structures;
using Rice.Server.Structures.Resources;
using Models = Rice.Server.Database.Models;

namespace Rice.Game
{
    public class Character
    {
        public User User;
        public ulong UID;
        public ulong CID;

        public string Name;
        public long Mito;
        public int Avatar;
        public int Level;
        public long Experience;
        public int City;
        public Vector4 Position;
        public int PosState;
        public int CurrentCarID;
        public int GarageLevel;
        public long TID;

        public List<Item> Inventory;
        public List<Vehicle> Garage;

        private List<ItemModInfo> PendingItemMods;

        public Area Area;
        public Quest ActiveQuest;
        public ushort CarSerial;

        private Character(Models.Character dbCharacter)
        {
            CID = (ulong) dbCharacter.ID;
            UID = (ulong) dbCharacter.UID;
            Name = dbCharacter.Name;
            Mito = dbCharacter.Mito;
            Avatar = dbCharacter.Avatar;
            Level = dbCharacter.Level;
            Experience = dbCharacter.Experience;
            City = dbCharacter.City;
            Position = new Vector4(dbCharacter.PosX, dbCharacter.PosY, dbCharacter.PosZ);
            PosState = dbCharacter.PosState;
            CurrentCarID = dbCharacter.CurrentCarID;
            GarageLevel = dbCharacter.GarageLevel;
            TID = dbCharacter.TID;
            Inventory = dbCharacter.Items.Select(Item.FromDB).ToList();
            Garage = dbCharacter.Vehicles.Select(Vehicle.FromDB).ToList();
            PendingItemMods = new List<ItemModInfo>();
            Log.WriteDebug($"char init, {Inventory.Count} items in inv, {Garage.Count} vehicles");
        }

        public ExpInfo GetExpInfo()
        {
            return ExpInfo.FromLevelExp(Level, Experience);
        }

        public bool SelectCar(int carId)
        {
            CurrentCarID = carId;
            using (var rc = Database.GetContext())
            {
                var character = rc.Characters.Find((long)CID);
                character.CurrentCarID = carId;
                rc.SaveChanges();
                return true;
            }
        }

        public bool GrantExperience(long exp)
        {
            Experience += exp;
            while (Experience > RiceServer.ExpSumTable[Level])
                Level++;

            using (var rc = Database.GetContext())
            {
                var ch = rc.Characters.Find((long) CID);
                if (ch == null)
                {
                    Log.WriteError($"Could not find char {CID} for GrantMito");
                    return false;
                }

                ch.Experience = Experience;
                rc.SaveChanges();
            }
            return true;
        }

        public bool GrantMito(ulong mito)
        {
            Mito += (long)mito;

            using (var rc = Database.GetContext())
            {
                var ch = rc.Characters.Find((long)CID);
                if (ch == null)
                {
                    Log.WriteError($"Could not find char {CID} for GrantMito");
                    return false;
                }

                ch.Mito = Mito;
                rc.SaveChanges();
            }
            return true;
        }

        public bool SpendMito(ulong mito)
        {
            long mitoAfter = Mito - (long)mito;
            if (mitoAfter < 0)
                return false;

            Mito = mitoAfter;
            
            using (var rc = Database.GetContext())
            {
                var ch = rc.Characters.Find((long)CID);
                if (ch == null)
                {
                    Log.WriteError($"Could not find char {CID} for SpendMito");
                    return false;
                }

                ch.Mito = Mito;
                rc.SaveChanges();
            }
            return true;
        }

        public bool GrantItem(string ID, uint StackNum, out Item resultItem)
        {
            Log.WriteLine($"Attempting to grant CID {CID} item {ID} (count {StackNum})");
            
            // TODO: Merge stacks
            var item = Item.CreateEntry(CID, ID, StackNum);
            resultItem = item;

            if (item == null)
                return false;
            
            Inventory.Add(item); // should be auto-added to inv in db at CreateEntry
            addItemMod(item);
            return true;
        }

        public bool EquipItem(int invIdx, short destSlot, int carId)
        {
            var invItem = Inventory.SingleOrDefault(i => i.invIdx == invIdx);
            if (invItem == null)
            {
                Log.WriteError($"Attempted to equip non-existant item at invIdx {invIdx}");
                return false;
            }

            if (Garage.SingleOrDefault(v => v.CarID == carId) == null)
            {
                Log.WriteError($"Couldn't find car id {carId} to equip item at invIdx {invIdx}");
                return false;
            }

            var itemCat = invItem.itemEntry.Category;
            if (!ItemTable.SlotMap.ContainsKey(itemCat) ||
                !ItemTable.SlotMap[itemCat].Contains(destSlot))
            {
                Log.WriteError($"Attempted to equip invIdx {invIdx} to {destSlot} which is invalid for category {itemCat}");
                return false;
            }

            using (var rc = Database.GetContext())
            {
                var itemsInSlot = rc.Items.Count(i => i.CID == (long) CID && i.State == 1 && i.Slot == destSlot);
                if (itemsInSlot >= 3)
                {
                    Log.WriteError(
                        $"Attempted to equip invIdx {invIdx} to {destSlot} which already has {itemsInSlot} items");
                    return false;
                }

                var dbItem = rc.Items.SingleOrDefault(i => i.CID == (long) CID && i.InvIdx == invIdx);
                if (dbItem == null)
                {
                    Log.WriteError($"Failed to find item invIdx {invIdx} for Character.EquipItem");
                    return false;
                }

                dbItem.Slot = destSlot;
                invItem.slot = (ushort) destSlot;

                dbItem.CurCarID = carId;
                invItem.curCarID = (uint) carId;

                dbItem.State = 1;
                invItem.state = 1;

                rc.SaveChanges();
                addItemMod(invItem, true);
            }

            return true;
        }

        public bool UnEquipItem(int invIdx, int carId)
        {
            var invItem = Inventory.SingleOrDefault(i => i.invIdx == invIdx);
            if (invItem == null)
            {
                Log.WriteError($"Attempted to unequip non-existant item at invIdx {invIdx}");
                return false;
            }

            if (Garage.SingleOrDefault(v => v.CarID == carId) == null)
            {
                Log.WriteError($"Couldn't find car id {carId} to unequip item at invIdx {invIdx}");
                return false;
            }

            using (var rc = Database.GetContext())
            {
                var dbItem = rc.Items.SingleOrDefault(i => i.CID == (long)CID && i.InvIdx == invIdx && i.CurCarID == carId);
                if (dbItem == null)
                {
                    Log.WriteError($"Failed to find item invIdx {invIdx} carId {carId} for Character.UnEquipItem");
                    return false;
                }

                dbItem.Slot = 0;
                invItem.slot = 0;

                dbItem.CurCarID = 0;
                invItem.curCarID = 0;

                dbItem.State = 0;
                invItem.state = 0;

                rc.SaveChanges();
                addItemMod(invItem, true);
            }

            return true;
        }

        public bool GrantVehicle(int carSort, out Vehicle resultVehicle, out Item resultKeyItem, int color = 0, int grade = 1)
        {
            Log.WriteLine($"Attempting to grant CID {CID} car {carSort}");

            var vehicle = Vehicle.CreateEntry(CID, carSort, color, grade);
            resultVehicle = vehicle;
            if (vehicle == null)
            {
                resultKeyItem = null;
                return false;
            }

            var vehicleListEntry = VehicleTable.Vehicles[carSort];
            var tableKeyItem = ItemTable.Items.FirstOrDefault(i => i.Category == "car" && i.ID == vehicleListEntry.GetKeyId());

            if (tableKeyItem == null)
            {
                Log.WriteError("Failed to find key in item table");
                // TODO: roll back vehicle creation :|
                resultKeyItem = null;
                return false;
            }

            var keyItem = Item.CreateEntry(CID, tableKeyItem.ID, 1, carId: vehicle.CarID);
            resultKeyItem = keyItem;
            if (keyItem == null)
            {
                Log.WriteError("Failed to create key");
                return false;
            }

            Garage.Add(vehicle);
            Inventory.Add(keyItem);
            addItemMod(keyItem);
            return true;
        }

        public bool RemoveVehicle(int carId)
        {
            var vehicle = Garage.SingleOrDefault(v => v.CarID == carId);
            if (vehicle == null)
            {
                Log.WriteError($"Failed to find vehicle for RemoveVehicle({carId})");
                return false;
            }

            var key = Inventory.SingleOrDefault(i => i.curCarID == vehicle.CarID);
            if (key == null)
            {
                Log.WriteError($"Failed to find vehicle key for RemoveVehicle({carId})");
                return false;
            }

            using (var rc = Database.GetContext())
            {
                var dbVehicle = rc.Vehicles.SingleOrDefault(v => v.CID == (long) CID && v.CarID == vehicle.CarID);
                var dbKey = rc.Items.SingleOrDefault(i => i.CID == (long) CID && i.CurCarID == vehicle.CarID);

                if (dbVehicle == null || dbKey == null)
                {
                    Log.WriteError($"Failed to find db counterparts of vehicle/key for RemoveVehicle({carId})");
                    return false;
                }

                rc.Vehicles.Remove(dbVehicle);
                rc.Items.Remove(dbKey);
                rc.SaveChanges();

                Inventory.Remove(key);
                Garage.Remove(vehicle);

                key.stackNum = 0;
                addItemMod(key);
            }
            return true;
        }

        public bool DropItem(int invIdx, out Item resultItem, uint count = 1)
        {
            using (var rc = Database.GetContext())
            {
                var dbItem = rc.Items.SingleOrDefault(i => i.CID == (long)CID && i.InvIdx == invIdx && i.StackNum >= count);
                var invItem = Inventory.SingleOrDefault(i => i.invIdx == invIdx);

                if (dbItem == null || invItem == null || !invItem.itemEntry.IsSellable())
                {
                    Log.WriteError($"Failed to find droppable item invIdx {invIdx} count >= {count} owned by {CID}");
                    resultItem = null;
                    return false;
                }

                bool destroyItem = count == dbItem.StackNum && count == invItem.stackNum;
                if (destroyItem)
                {
                    rc.Items.Remove(dbItem);
                    Inventory.Remove(invItem);
                    invItem.stackNum = 0;
                }
                else
                {
                    dbItem.StackNum -= (int)count;
                    invItem.stackNum -= count;
                }
                rc.SaveChanges();
                resultItem = invItem;
                addItemMod(invItem);
                return true;
            }
        }

        private void addItemMod(Item item, bool moved = false)
        {
            var itemInfo = item.GetInfo();
            var modInfo = new ItemModInfo
            {
                Item = itemInfo,
                State = moved ? 3 : itemInfo.StackNum == 0 ? 2 : 0
            };
            PendingItemMods.Add(modInfo);
        }

        public void FlushModInfo(RiceClient client)
        {
            var mods = PendingItemMods.ToArray();
            PendingItemMods.Clear();

            //ItemModList
            var packet = new RicePacket(402);
            packet.Writer.Write(mods.Length);
            foreach (var itemMod in mods)
                packet.Writer.Write(itemMod);
            client.Send(packet);
        } 

        public CharInfo GetInfo()
        {
            return new CharInfo
            {
                Avatar = (ushort)Avatar,
                Name = Name,
                CID = CID,
                City = City,
                Position = Position,
                PosState = PosState,
                CurrentCarID = CurrentCarID,
                PType = (byte)'A',
                TeamJoinDate = DateTime.Now,
                TeamLeaveDate = DateTime.Now,
                TeamCloseDate = DateTime.Now,
                ExpInfo = GetExpInfo(),
                HancoinGarage = GarageLevel,
                Level = (ushort)Level,
                TeamId = TID,
                TeamName = "", // load this
                MitoMoney = Mito,
                Flags = -1
            };
        }

        public static Character Retrieve(string charname)
        {
            Character character;

            using (var rc = Database.GetContext())
                character = new Character(rc.Characters.SingleOrDefault(c => c.Name == charname));

            return character;
        }

        public static bool IsNameUsable(string name)
        {
            using (var rc = Database.GetContext())
                return rc.Characters.Count(c => c.Name == name) == 0;
        }

        public static bool Create(ulong uid, string name, ushort avatar)
        {
            using (var rc = Database.GetContext())
            {
                rc.Characters.Add(new Server.Database.Models.Character
                {
                    UID = (long) uid,
                    Name = name,
                    Avatar = avatar,
                    TID = -1
                });

                try
                {
                    rc.SaveChanges();
                }
                catch (DataException ex)
                {
                    Log.WriteError(ex.ToString());
                    return false;
                }
            }
            return true;
        }

        public static bool Delete(string name)
        {
            using (var rc = Database.GetContext())
            {
                var ch = rc.Characters.SingleOrDefault(c => c.Name == name);
                if (ch == null)
                {
                    Log.WriteError($"Tried to delete non-existing character named {name}");
                    return false;
                }

                rc.Characters.Remove(ch);

                try
                {
                    rc.SaveChanges();
                }
                catch (DataException ex)
                {
                    Log.WriteError(ex.ToString());
                    return false;
                }
                return true;
            }
        }

        public bool SaveCarPos(int channelId, Vector4 pos, int cityId, int posState)
        {
            using (var rc = Database.GetContext())
            {
                var ch = rc.Characters.Find((long) CID);
                if (ch == null)
                {
                    Log.WriteError($"Could not find char {CID} for SaveCarPos");
                    return false;
                }

                ch.PosX = pos.X;
                ch.PosY = pos.Y;
                ch.PosZ = pos.Z;
                ch.City = cityId;
                ch.PosState = posState;
                rc.SaveChanges();
            }
            return true;
        }

        public static List<Character> Retrieve(ulong uid)
        {
            using (var rc = Database.GetContext())
            {
                var user = rc.Users.Find((long)uid);
                return user.Characters.Select(ch => new Character(ch)).ToList();
            }
        }
    }
}

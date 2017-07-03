using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Rice.Server.Structures.Resources
{
    public static class ItemTable
    {
        public static List<GenericItemEntry> Items = new List<GenericItemEntry>();
        public static Dictionary<string, int[]> SlotMap = new Dictionary<string, int[]>();

        public static void Load(string itempath, string useitempath)
        {
            if (!File.Exists(itempath))
            {
                Log.WriteError("Could not find ItemTable at " + itempath);
                return;
            }

            if (!File.Exists(useitempath))
            {
                Log.WriteError("Could not find UseItemTable at " + useitempath);
                return;
            }

            string itemjson = File.ReadAllText(itempath);
            string useitemjson = File.ReadAllText(useitempath);
            Items.AddRange(JsonConvert.DeserializeObject<List<ItemTableEntry>>(itemjson));
            Items.AddRange(JsonConvert.DeserializeObject<List<UseItemTableEntry>>(useitemjson));

            int[] partSlots = {100, 101, 103, 104, 106, 107, 109, 110};
            int[] speedSlots = {100, 101};
            int[] accelSlots = {103, 104};
            int[] duraSlots = {106, 107};
            int[] boostSlots = {109, 110};

            SlotMap = new Dictionary<string, int[]>
            {
                {"op_F", partSlots},

                {"speed", speedSlots},
                {"op_S", speedSlots},
                {"dock_S", speedSlots},

                {"accel", accelSlots},
                {"op_A", accelSlots},
                {"dock_A", accelSlots},

                {"crash", duraSlots},
                {"op_C", duraSlots},
                {"dock_C", duraSlots},

                {"boost", boostSlots},
                {"op_B", boostSlots},
                {"dock_B", boostSlots},

                {"neo_weapon", new [] {112}},
                {"neo_coil", new [] {113}},
                {"neo_fuse", new [] {114}},
                {"neo_armor", new [] {115}}
            };
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class GenericItemEntry
    {
        [JsonProperty("id")] public string ID;

        [JsonProperty("category")] public string Category;

        [JsonProperty("name")] public string Name;

        [JsonProperty("description")] public string Description;

        [JsonProperty("function")] public string Function;

        [JsonProperty("nextstate")] public string NextState;

        [JsonProperty("buyvalue")] public string BuyValue;
        [JsonProperty("sellvalue")] public string SellValue;

        [JsonProperty("expirationtime")] public string ExpirationTime;

        [JsonProperty("auctionable")] public string Auctionable;

        [JsonProperty("partsshop")] public string PartsShop;

        [JsonProperty("sendable")] public string Sendable;

        virtual public bool IsStackable()
        {
            return false;
        }

        virtual public uint GetMaxStack()
        {
            return 1;
        }

        public bool IsSellable() => SellValue.ToLower() != "n/a";
    }

    public class ItemTableEntry : GenericItemEntry
    {
        [JsonProperty("setid")] public string SetID;

        [JsonProperty("setname")] public string SetName;

        [JsonProperty("grade")] public string Grade;

        [JsonProperty("minlevel")] public string MinLevel;

        [JsonProperty("basepoints")] public string BasePoints;

        [JsonProperty("basepointmodifier")] public string BasePointModifier;

        [JsonProperty("basepointvariable")] public string BasePointVariable;

        [JsonProperty("partassist")] public string PartAssist;

        [JsonProperty("lube")] public string Lube;

        [JsonProperty("neostats")] public string NeoStats;
    }

    public class UseItemTableEntry : GenericItemEntry
    {
        [JsonProperty("maxstack")] public string MaxStack;

        [JsonProperty("stat")] public string StatModifier;

        [JsonProperty("cooldown")] public string CooldownTime;

        [JsonProperty("duration")] public string Duration;

        override public bool IsStackable()
        {
            return Category != "car";
        }

        override public uint GetMaxStack()
        {
            if (MaxStack == "n/a" || MaxStack == "0")
                return 99;
            return Convert.ToUInt32(MaxStack);
        }
    }
}
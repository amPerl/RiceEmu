using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Rice.Server.Structures.Resources
{
    public static class AssistTable
    {
        public static List<AssistTableEntry> Assists = new List<AssistTableEntry>();

        public static void Load(string assistclientpath)
        {
            if (!File.Exists(assistclientpath))
            {
                Log.WriteError("Could not find AssistClient at " + assistclientpath);
                return;
            }

            string assistClientJson = File.ReadAllText(assistclientpath);
            Assists = JsonConvert.DeserializeObject<List<AssistTableEntry>>(assistClientJson);
        }

        public static int GetIndex(string id)
        {
            int idx = Assists.FindIndex(i => i.ID.ToLower() == id.ToLower());
            return idx < 0 ? -1 : (idx + 1);
        }
    }
    
    [JsonObject(MemberSerialization.OptIn)]
    public class AssistTableEntry
    {
        [JsonProperty("id")] public string ID { get; set; }
        [JsonProperty("function")] public string Function { get; set; }

        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("description")] public string Description { get; set; }

        // double or n/a
        [JsonProperty("stat")] public string Stat { get; set; }
        // km/h, point, %, n/a etc
        [JsonProperty("unit")] public string Unit { get; set; }
    }
}
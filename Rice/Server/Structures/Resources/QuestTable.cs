using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Rice.Server.Structures.Resources
{
    public static class QuestTable
    {
        public static List<QuestTableEntry> Quests;

        public static void Load(string path)
        {
            if (!File.Exists(path))
            {
                Log.WriteError("Could not find QuestTable at " + path);
                return;
            }

            string json = File.ReadAllText(path);
            Quests = JsonConvert.DeserializeObject<List<QuestTableEntry>>(json);
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class QuestTableEntry
    {
        [JsonProperty("id")] public string ID;

        [JsonProperty("missiontype")] public string MissionType;

        [JsonProperty("tableindex")] public int TableIndex;

        [JsonProperty("title")] public string Title;

        [JsonProperty("introprompt")] public string IntroPrompt;
        [JsonProperty("station")] public string StartStation;

        [JsonProperty("completestation")] public string CompleteStation;
        [JsonProperty("completeprompt")] public string CompletePrompt;

        [JsonProperty("objective1")] public string Objective1;
        [JsonProperty("objective2")] public string Objective2;
        [JsonProperty("objective3")] public string Objective3;
        [JsonProperty("objective4")] public string Objective4;
        [JsonProperty("objective5")] public string Objective5;

        [JsonProperty("exp")] public int Experience;
        [JsonProperty("mito")] public int Mito;

        [JsonProperty("reward1")] public string RewardItem1;
        [JsonProperty("reward2")] public string RewardItem2;
        [JsonProperty("reward3")] public string RewardItem3;

        public override string ToString()
        {
            return Title;
        }

        public int GetObjectiveCount()
        {
            int count = 0;
            count += Objective1 != "0" ? 1 : 0;
            count += Objective2 != "0" ? 1 : 0;
            count += Objective3 != "0" ? 1 : 0;
            count += Objective4 != "0" ? 1 : 0;
            count += Objective5 != "0" ? 1 : 0;
            return count;
        }

        public string[] GetRewards()
        {
            var rewards = new List<string>();
            if (RewardItem1 != "0")
                rewards.Add(RewardItem1);
            if (RewardItem2 != "0")
                rewards.Add(RewardItem2);
            if (RewardItem3 != "0")
                rewards.Add(RewardItem3);
            return rewards.ToArray();
        }
    }
}
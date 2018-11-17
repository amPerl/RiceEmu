using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Rice
{
    public class Config
    {
        const string configPath = "server.json";

        public bool DebugMode = true;
        public string PublicIP = "127.0.0.1";

        public ushort AuthPort = 11005;
        public ushort LobbyPort = 11011;
        public ushort GamePort = 11021;
        public ushort AreaPort = 11031;
        public ushort RankingPort = 11078;

        public static Config Load(string path = "config.json")
        {
            Config config;

            if (File.Exists(configPath))
            {
                Log.WriteLine("Config file exists, loading.");

                string json = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<Config>(json);
            }
            else
            {
                Log.WriteLine("Config file could not be found, using defaults.");
                config = new Config();
            }

            return config;
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(configPath, json);
        }
    }
}

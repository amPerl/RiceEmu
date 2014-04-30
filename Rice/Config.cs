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

        public int AuthPort = 11005;
        public int LobbyPort = 11011;
        public int GamePort = 11021;
        public int AreaPort = 11031;
        public int RankingPort = 11041;

        public string DatabaseHost = "localhost";
        public int DatabasePort = 3306;
        public string DatabaseName = "RiceDB";
        public string DatabaseUser = "Rice";
        public string DatabasePassword = "ChangeMe";

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
                Log.WriteLine("Config file could not be found, creating.");

                config = new Config();
                config.Save();
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

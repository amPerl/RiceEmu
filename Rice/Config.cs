using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rice
{
    public class Config
    {
        public static int AuthPort = 11005;
        public static int LobbyPort = 11011;
        public static int GamePort = 11021;
        public static int AreaPort = 11031;
        public static int RankingPort = 11041;

        public static void Load(string path = "config.json")
        {
            Log.WriteLine("Loading config.");

            // TODO: Actually load config.
        }
    }
}

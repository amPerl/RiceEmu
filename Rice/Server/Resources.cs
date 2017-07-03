using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rice.Server.Structures.Resources;

namespace Rice.Server
{
    public static class Resources
    {
        public static void Initialize(Config config)
        {
            QuestTable.Load("res/QuestClient.json");
            ItemTable.Load("res/ItemClient.json", "res/UseItemClient.json");
            VehicleTable.Load("res/VehicleUpgrade.json", "res/VehicleList.json");
            AssistTable.Load("res/AssistClient.json");

            Log.WriteLine("Loaded resources.");
        }
    }
}

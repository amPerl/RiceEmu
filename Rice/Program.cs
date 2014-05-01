using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rice.Server.Core;

namespace Rice
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Rice";
            Console.Clear();

            Config config = Config.Load();

            Database.Initialize(config);
            RiceServer.Initialize(config);

            Database.Start();
            RiceServer.Start();

            Console.ReadLine();
        }
    }
}

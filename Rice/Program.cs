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

            Config config = Config.Load();
            RiceServer.Initialize(config);

            RiceServer.Start();

            Console.ReadLine();
        }
    }
}

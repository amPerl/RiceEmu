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
            Config.Load();
            RiceServer.Initialize();
            RiceServer.Start();
            Console.ReadLine();
        }
    }
}

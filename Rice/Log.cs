using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rice
{
    public static class Log
    {
        public static void WriteLine(string format, params object[] args)
        {
            string message = String.Format(format, args);

            Console.WriteLine("[{0}] {1}", DateTime.Now.ToLongTimeString(), message);

            File.AppendAllText("ServerLog.txt", String.Format("[{0}] {1}{2}", DateTime.Now, message, Environment.NewLine));
        }
    }
}

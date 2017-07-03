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
            string message = string.Format(format, args);

            Console.WriteLine("[{0}] {1}", DateTime.Now.ToLongTimeString(), message);
        }

        public static void WriteError(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            WriteLine(format, args);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void WriteDebug(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteLine($"DEBUG: {format}", args);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}

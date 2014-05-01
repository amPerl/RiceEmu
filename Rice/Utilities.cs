using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rice
{
    public class Utilities
    {
        public static string MD5(string str)
        {
            byte[] strBuf = Encoding.ASCII.GetBytes(str);
            byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(strBuf);

            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}

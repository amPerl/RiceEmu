using System;
using System.IO;
using System.Text;

namespace Rice.Server
{
    public class PacketWriter : BinaryWriter
    {
        public PacketWriter(MemoryStream stream)
            : base(stream, Encoding.Unicode)
        {
        }

        public byte[] GetBuffer()
        {
            return (this.BaseStream as MemoryStream).ToArray();
        }

        public void WriteUnicode(string str, bool lengthPrefix = true)
        {
            byte[] buf = Encoding.Unicode.GetBytes(str + "\0");

            if (lengthPrefix)
                Write((ushort)str.Length);

            Write(buf);
        }

        public void WriteUnicodeStatic(string str, int maxLength)
        {
            if (str.Length > maxLength) 
                str = str.Substring(0, maxLength);
            
            byte[] stringBuf = Encoding.Unicode.GetBytes(str);

            byte[] buf = new byte[maxLength * 2];
            Array.Copy(stringBuf, 0, buf, 0, stringBuf.Length);

            Write(buf);
        }

        public void WriteASCIIStatic(string str, int maxLength)
        {
            if (str.Length > maxLength)
                str = str.Substring(0, maxLength);

            byte[] stringBuf = Encoding.ASCII.GetBytes(str);

            byte[] buf = new byte[maxLength];
            Array.Copy(stringBuf, 0, buf, 0, stringBuf.Length);

            Write(buf);
        }
    }

    public class PacketReader : BinaryReader
    {
        public PacketReader(MemoryStream stream)
            : base(stream, Encoding.Unicode)
        {
        }

        public string ReadUnicode() 
        {
            StringBuilder sb = new StringBuilder();
            short val;
            do
            {
                val = ReadInt16();
                sb.Append((char)val);
            }
            while (val > 0);
            return sb.ToString();
        }

        public string ReadUnicodeStatic(int maxLength)
        {
            byte[] buf = ReadBytes(maxLength * 2);
            string str = Encoding.Unicode.GetString(buf);

            if (str.Contains("\0"))
                str = str.Substring(0, str.IndexOf('\0'));

            return str;
        }

        public string ReadUnicodePrefixed()
        {
            ushort length = ReadUInt16();
            return ReadUnicodeStatic(length);
        }

        public string ReadASCII()
        {
            StringBuilder sb = new StringBuilder();
            byte val;
            do
            {
                val = ReadByte();
                sb.Append((char)val);
            }
            while (val > 0);
            return sb.ToString();
        }

        public string ReadASCIIStatic(int maxLength)
        {
            byte[] buf = ReadBytes(maxLength);
            string str = Encoding.ASCII.GetString(buf);

            if (str.Contains("\0"))
                str = str.Substring(0, str.IndexOf('\0'));

            return str;
        }
    }
}
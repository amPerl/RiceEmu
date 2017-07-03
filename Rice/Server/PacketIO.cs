using System;
using System.IO;
using System.Text;
using Rice.Server.Structures;

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
            return (BaseStream as MemoryStream).ToArray();
        }

        public void WriteUnicode(string str, bool lengthPrefix = true)
        {
            if (str == null)
                str = "";

            byte[] buf = Encoding.Unicode.GetBytes(str + "\0");

            if (lengthPrefix)
                Write((ushort)str.Length);

            Write(buf);
        }

        public void WriteUnicodeStatic(string str, int maxLength, bool nullTerminated = false)
        {
            if (str == null)
                str = "";

            if (str.Length > maxLength) 
                str = str.Substring(0, maxLength);

            if (nullTerminated)
            {
                if (str.Length > maxLength - 1)
                    str = str.Substring(0, maxLength - 1);
                str += '\0';
            }

            byte[] stringBuf = Encoding.Unicode.GetBytes(str);

            byte[] buf = new byte[maxLength * 2];
            Array.Copy(stringBuf, 0, buf, 0, stringBuf.Length);

            Write(buf);
        }

        public void WriteASCIIStatic(string str, int maxLength)
        {
            if (str == null)
                str = "";

            if (str.Length > maxLength)
                str = str.Substring(0, maxLength);

            byte[] stringBuf = Encoding.ASCII.GetBytes(str);

            byte[] buf = new byte[maxLength];
            Array.Copy(stringBuf, 0, buf, 0, stringBuf.Length);

            Write(buf);
        }

        public void Write(ISerializable structure)
        {
            structure.Serialize(this);
        }

        public void Write(DateTime date)
        {
            Write(Convert.ToUInt32((date - new DateTime(1970, 1,1)).TotalSeconds));
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
                if (val > 0)
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
        
        public DateTime ReadDate()
        {
            return new DateTime(1970, 1, 1).AddSeconds(ReadUInt32());
        }

        public Vector4 ReadVector4()
        {
            float x = ReadSingle();
            float y = ReadSingle();
            float z = ReadSingle();
            float rotation = ReadSingle();
            return new Vector4(x, y, z, rotation);
        }
    }
}
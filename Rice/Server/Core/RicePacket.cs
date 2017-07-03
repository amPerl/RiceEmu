using System;
using System.IO;

namespace Rice.Server.Core
{
    public class RicePacket
    {
        public RiceClient Sender;

        public PacketWriter Writer;
        public PacketReader Reader;

        public byte[] Buffer;
        public ushort ID;

        public RicePacket(ushort id)
        {
            Writer = new PacketWriter(new MemoryStream());
            ID = id;

            Writer.Write(id);
        }

        public RicePacket(RiceClient sender, ushort id, byte[] buffer)
        {
            Sender = sender;
            Buffer = buffer;
            ID = id;
            Reader = new PacketReader(new MemoryStream(Buffer));
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    internal sealed class RicePacketAttribute : Attribute
    {
        private readonly RiceServer.ServerType handlers;
        private readonly ushort id;

        public RicePacketAttribute(ushort id, RiceServer.ServerType handlers)
        {
            this.id = id;
            this.handlers = handlers;
        }

        public RiceServer.ServerType Handlers { get { return handlers; } }
        public ushort ID { get { return id; } }

        public bool CheckedIn { get; set; }
    }
}
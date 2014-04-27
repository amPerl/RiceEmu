using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rice.Server.Structures
{
    public interface ISerializable
    {
        void Serialize(PacketWriter bw);
    }
}

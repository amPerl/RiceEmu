using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rice.Server.Structures
{
    public struct CarInfo : ISerializable
    {
        public CarUnit CarUnit;
        public uint Color;
        public uint Color2;
        public float MitronCapacity;
        public float MitronEfficiency;
        public bool AuctionOn;
        public bool SBBOn;

        public void Serialize(PacketWriter writer)
        {
            CarUnit.Serialize(writer);
            writer.Write(Color);
            writer.Write(Color2);
            writer.Write(MitronCapacity);
            writer.Write(MitronEfficiency);
            writer.Write(AuctionOn);
            writer.Write(SBBOn);
        }
    }

    public struct CarUnit : ISerializable
    {
        public uint CarID;
        public uint CarType;
        public uint BaseColor;
        public uint Grade;
        public uint SlotType;
        public uint AuctionCnt;
        public float Mitron;
        public float Kmh;

        public void Serialize(PacketWriter writer)
        {
            writer.Write(CarID);
            writer.Write(CarType);
            writer.Write(BaseColor);
            writer.Write(Grade);
            writer.Write(SlotType);
            writer.Write(AuctionCnt);
            writer.Write(Mitron);
            writer.Write(Kmh);
        }
    }
}

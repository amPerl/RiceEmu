using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rice.Server.Core;

namespace Rice.Server.Packets.Game
{
    public static class Character
    {
        [RicePacket(123, RiceServer.ServerType.Game)]
        public static void LoadCharacter(RicePacket packet)
        {
            string characterName = packet.Reader.ReadUnicode();

            packet.Sender.Player = new Rice.Game.Player(Rice.Game.User.Empty); // TODO: load user info on checkin
            packet.Sender.Player.ActiveCharacter = characterName; // TODO: verify this

            var ack = new RicePacket(124);

            var ackStruct = new Structures.LoadCharAck
            {
                CharInfo = new Structures.CharInfo
                {
                    Avatar = 1,
                    Name = characterName,
                    Cid = 1,
                    City = 1,
                    CurCarId = 1,
                    ExpInfo = new Structures.ExpInfo
                    {
                        BaseExp = 0,
                        CurExp = 0,
                        NextExp = 0
                    },
                    HancoinGarage = 1,
                    Level = 80,
                    TeamId = 1,
                    TeamName = "Staff",
                    MitoMoney = 123456789
                },
                nCarSize = 1,
                CarInfo = new List<Structures.CarInfo>()
                {
                    new Structures.CarInfo
                    {
                        AuctionOn = false,
                        CarUnit = new Structures.CarUnit
                        {
                            AuctionCnt = 0,
                            BaseColor = 0,
                            CarID = 1,
                            CarType = 24,
                            Grade = 9,
                            Mitron = 0.00f,
                            Kmh = 200,
                            SlotType = 0
                        },
                        Color = 0,
                        MitronCapacity = 0.00f,
                        MitronEfficiency = 0.00f
                    }
                }
            };
            ack.Writer.Write(ackStruct);

            packet.Sender.Send(ack);
            Log.WriteLine("Sent LoadCharAck");
        }
    }
}

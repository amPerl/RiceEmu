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

            var character = Rice.Game.Character.Retrieve(characterName); // TODO: verify this
            var user = Rice.Game.User.Retrieve(character.UID);

            packet.Sender.Player = new Rice.Game.Player(user);
            packet.Sender.Player.Characters = Rice.Game.Character.Retrieve(user.UID);
            packet.Sender.Player.ActiveCharacter = character;

            var ack = new RicePacket(124);

            var ackStruct = new Structures.LoadCharAck
            {
                CharInfo = character.GetInfo(),
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

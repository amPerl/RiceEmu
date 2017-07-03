using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rice.Game;
using Rice.Server.Core;

namespace Rice.Server.Packets.Area
{
    class Area
    {
        [RicePacket(562, RiceServer.ServerType.Area)]
        public static void EnterArea(RicePacket packet)
        {
            //TODO: Associate instance with gameserver player based on provided serial, verify, handle
            var serial = packet.Reader.ReadUInt16();
            var name = packet.Reader.ReadUnicodeStatic(21);
            var area = packet.Reader.ReadInt32();
            var group = packet.Reader.ReadInt32();
            var localtime = packet.Reader.ReadInt32();

            if (packet.Sender.Player == null)
            {
                var serverSerial = RiceServer.GetSerial(serial);

                if (serverSerial == null || serverSerial.GetOwner().ActiveCharacter.Name != name)
                {
                    Log.WriteLine("Serial non-existent or invalid for current user.");
                    packet.Sender.Error("water u even doin");
                    return;
                }

                packet.Sender.Player = serverSerial.GetOwner();
                packet.Sender.Player.ActiveCharacter.CarSerial = serial;
                packet.Sender.Player.AreaClient = packet.Sender;
            }

            RiceServer.GetArea(area).AddPlayer(packet.Sender.Player);

            var ack = new RicePacket(563);
            ack.Writer.Write(area);
            ack.Writer.Write(1); //Result
            ack.Writer.Write(localtime);
            ack.Writer.Write(Environment.TickCount);
            ack.Writer.Write(new byte[6]); // The rest of this is null
            packet.Sender.Send(ack);
        }

        [RicePacket(0x21D, RiceServer.ServerType.Area, CheckedIn = true)]
        public static void MoveVehicle(RicePacket packet)
        {
            packet.Reader.ReadUInt16(); // Skip serial, we'll use our local copy of it.
            byte[] movement = packet.Reader.ReadBytes(112);

            packet.Sender.Player.ActiveCharacter.Area.BroadcastMovement(packet.Sender.Player, movement);
        }

        [RicePacket(0x2AA, RiceServer.ServerType.Area)]
        public static void AreaStatus(RicePacket packet)
        {
            var ack = new RicePacket(0x2AB);

            // from ZoneServer
            for (int i = 0; i < 100; i++)
            {
                var area = RiceServer.GetArea(i);
                ack.Writer.Write(area != null ? area.GetPlayerCount() : 0);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rice.Server.Core;

namespace Rice.Server.Packets.Game
{
    public static class Misc
    {
        [RicePacket(784, RiceServer.ServerType.Game)]
        public static void GetDateTime(RicePacket packet)
        {
            /*
            var ack = new RicePacket(785);
            var timestamp = (Int32)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            ack.Writer.Write(packet.Reader.ReadUInt32());
            ack.Writer.Write(packet.Reader.ReadUInt32());
            ack.Writer.Write(packet.Reader.ReadSingle());
            ack.Writer.Write(Environment.TickCount);
            ack.Writer.Write(timestamp);
            ack.Writer.Write(1);
            ack.Writer.Write((short)DateTime.Now.Year);
            ack.Writer.Write((short)DateTime.Now.Month);
            ack.Writer.Write((short)DateTime.Now.Day);
            ack.Writer.Write((short)DateTime.Now.DayOfWeek);
            ack.Writer.Write((byte)DateTime.Now.Hour);
            ack.Writer.Write((byte)DateTime.Now.Minute);
            ack.Writer.Write((byte)DateTime.Now.Second);
            ack.Writer.Write((byte)0);
            packet.Sender.Send(ack);
            */
        }
        [RicePacket(120, RiceServer.ServerType.Game)]
        public static void CheckInGame(RicePacket packet)
        {
            Log.WriteLine("CheckInGame");
            var version = packet.Reader.ReadUInt32();
            var ticket = packet.Reader.ReadUInt32();
            var username = packet.Reader.ReadUnicodeStatic(32);
            // Followed by int m_IsPcBang, 21 bytes of weird shit we don't know about, and yet another 21 byte chunk of weird shit we don't know about.

            var serverTicket = RiceServer.GetTicket(ticket);

            if (serverTicket == null || !serverTicket.ValidateOrigin(packet.Sender, username))
            {
                Log.WriteLine("Ticket is non-existent or invalid for current user.");
                Log.WriteLine("ticket: {0}, packet sender: {1}, username: {2}", serverTicket.Identifier, packet.Sender.GetRemoteIP(), username);
                packet.Sender.Error("water u even doin");
                return;
            }

            packet.Sender.Player = serverTicket.GetOwner();
            packet.Sender.Player.GameClient = packet.Sender;

            var ack = new RicePacket(121);
            ack.Writer.Write(1L);
            packet.Sender.Send(ack);
        }

        [RicePacket(125, RiceServer.ServerType.Game)]
        public static void JoinChannel(RicePacket packet)
        {
            var serial = RiceServer.CreateSerial(packet.Sender);

            var ack = new RicePacket(126);
            ack.Writer.WriteUnicodeStatic("speeding", 10);
            ack.Writer.WriteUnicodeStatic(packet.Sender.Player.ActiveCharacter.Name, 16);
            ack.Writer.Write(serial.Identifier);
            ack.Writer.Write((ushort)123); // Age
            packet.Sender.Send(ack);
        }

        [RicePacket(4, RiceServer.ServerType.Game)]
        public static void Latency(RicePacket packet)
        {
            // TODO: log SkidRush and check this

            float time = packet.Reader.ReadSingle();
            //Log.WriteLine("Latency: {0}", time);

            var ack = new RicePacket(4); // no idea if this is right, pls check Cmd_Latency on client
            ack.Writer.Write(time);
            packet.Sender.Send(ack);
        }
        
        [RicePacket(3916, RiceServer.ServerType.Game)]
        public static void LatencyRelated(RicePacket packet)
        {
            // TODO: log SkidRush and check this
            // possibly Cmd_PartyPing (client's Cmd_PartyPing handler is 3915)

            float time = packet.Reader.ReadSingle();
            //Log.WriteLine("Latency??: {0}", BitConverter.ToString(packet.Buffer));
        }

        [RicePacket(3917, RiceServer.ServerType.Game)]
        public static void UnknownSync(RicePacket packet)
        {
            // hide sync packets for now
        }
    }
}

using Rice.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rice.Server.Packets.Game
{
    public class Party
    {
        [RicePacket(228, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void PartyPreCheck(RicePacket packet)
        {
            int serial = packet.Reader.ReadInt32();
            string invitee = packet.Reader.ReadUnicodeStatic(21);
            Log.WriteLine("PartyPreCheck: {0}", BitConverter.ToString(packet.Buffer));

            var ack = new RicePacket(229);
            ack.Writer.Write(0); // Serial, generate and store for invite?
            ack.Writer.Write(0L); // Cid
            ack.Writer.WriteUnicodeStatic(invitee, 21); // char name
            ack.Writer.Write(-1); // PartyId (should be -1 if not in party)
            ack.Writer.WriteUnicodeStatic("", 30); // PartyName
            packet.Sender.Send(ack);
        }

        [RicePacket(240, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void PartyInvite(RicePacket packet)
        {
            uint serial = packet.Reader.ReadUInt32();
            uint ticket = packet.Reader.ReadUInt32();

            //xistrpartymemberinfo
            long charCid = packet.Reader.ReadInt64();
            string charName = packet.Reader.ReadUnicodeStatic(21);
            ushort charAvatar = packet.Reader.ReadUInt16();
            ushort charLevel = packet.Reader.ReadUInt16();

            string partyName = packet.Reader.ReadUnicodeStatic(30);
            string inviteMsg = packet.Reader.ReadUnicodeStatic(100);
            //server seems to create a party and relay this packet
            Log.WriteLine("PartyInvite: {0}", BitConverter.ToString(packet.Buffer));
        }

        [RicePacket(241, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void PartyReject(RicePacket packet)
        {
            Log.WriteLine("PartyReject: {0}", BitConverter.ToString(packet.Buffer));
        }

        [RicePacket(242, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void PartyJoin(RicePacket packet)
        {
            Log.WriteLine("PartyJoin: {0}", BitConverter.ToString(packet.Buffer));
        }

        [RicePacket(244, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void PartyLeave(RicePacket packet)
        {
            Log.WriteLine("PartyLeave: {0}", BitConverter.ToString(packet.Buffer));
        }

        [RicePacket(245, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void PartyBanish(RicePacket packet)
        {
            Log.WriteLine("PartyBanish: {0}", BitConverter.ToString(packet.Buffer));
        }

        [RicePacket(246, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void PartyApply(RicePacket packet)
        {
            Log.WriteLine("PartyApply: {0}", BitConverter.ToString(packet.Buffer));
        }

        [RicePacket(247, RiceServer.ServerType.Game, CheckedIn = true)]
        public static void PartyAccept(RicePacket packet)
        {
            Log.WriteLine("PartyAccept: {0}", BitConverter.ToString(packet.Buffer));
        }
    }
}

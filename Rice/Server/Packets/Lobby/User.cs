using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rice.Server.Core;

namespace Rice.Server.Packets.Lobby
{
    public static class User
    {
        [RicePacket(60, RiceServer.ServerType.Lobby)]
        public static void UserInfo(RicePacket packet)
        {
            Log.WriteLine("UserInfo request.");

            var settings = new RicePacket(30);
            settings.Writer.Write(Static.GameSettings);
            packet.Sender.Send(settings);

            var ack = new RicePacket(61);

            ack.Writer.Write(0); // Permissions
            ack.Writer.Write(2); // Character Count

            ack.Writer.WriteUnicodeStatic("userName", 18); // Username
            ack.Writer.Write((long)0);
            ack.Writer.Write((long)0);
            ack.Writer.Write((long)0);
            ack.Writer.Write(0);

            // Generate 2 characters
            for (int i = 1; i <= 2; i++)
            {
                ack.Writer.WriteUnicodeStatic("char" + i, 21); // Name
                ack.Writer.Write((ulong)i); // ID
                ack.Writer.Write(i); // Avatar
                ack.Writer.Write(i * 20); // Level
                ack.Writer.Write(1);
                ack.Writer.Write(0x50);
                ack.Writer.Write(0);
                ack.Writer.Write(int.MaxValue / 2); // Creation Date
                ack.Writer.Write((ulong)1); // Crew ID
                ack.Writer.Write(0L); // Crew Image
                ack.Writer.WriteUnicodeStatic("Staff, maybe", 12); // Crew Name
                ack.Writer.Write((short)0);
                ack.Writer.Write((short)-1);
                ack.Writer.Write((short)0);
            }

            packet.Sender.Send(ack);

            Log.WriteLine("Sent character list.");
        }

        [RicePacket(80, RiceServer.ServerType.Lobby)]
        public static void CheckCharacterName(RicePacket packet)
        {
            string characterName = packet.Reader.ReadUnicode();
            Log.WriteLine("VerifyCharName: {0}", characterName);

            // TODO: Verify

            var ack = new RicePacket(81);
            ack.Writer.WriteUnicodeStatic(characterName, 21);
            ack.Writer.Write(1); // Availability. 1 = Available, 0 = Unavailable.
            packet.Sender.Send(ack);
        }

        [RicePacket(82, RiceServer.ServerType.Lobby)]
        public static void CreateCharacter(RicePacket packet)
        {
            string characterName = packet.Reader.ReadUnicode();
            Log.WriteLine("CreateChar: {0}", characterName);

            // TODO: Verify, Handle
        }

        [RicePacket(50, RiceServer.ServerType.Lobby)]
        public static void DeleteCharacter(RicePacket packet)
        {
            string characterName = packet.Reader.ReadUnicode();
            Log.WriteLine("DeleteChar: {0}", characterName);

            // TODO: Verify

            var ack = new RicePacket(84);
            ack.Writer.WriteUnicodeStatic(characterName, 21);
            packet.Sender.Send(ack);
        }
    }
}

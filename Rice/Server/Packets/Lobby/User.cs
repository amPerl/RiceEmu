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

            packet.Sender.Player.Characters = Rice.Game.Character.Retrieve(packet.Sender.Player.User.UID);

            var ack = new RicePacket(61);

            ack.Writer.Write(0); // Permissions
            ack.Writer.Write(packet.Sender.Player.Characters.Count); // Character Count

            ack.Writer.WriteUnicodeStatic(packet.Sender.Player.User.Name, 18); // Username
            ack.Writer.Write((long)0);
            ack.Writer.Write((long)0);
            ack.Writer.Write((long)0);
            ack.Writer.Write(0);

            foreach (var character in packet.Sender.Player.Characters)
            {
                ack.Writer.WriteUnicodeStatic(character.Name, 21); // Name
                ack.Writer.Write(character.CID); // ID
                ack.Writer.Write(character.Avatar); // Avatar
                ack.Writer.Write(character.Level); // Level
                ack.Writer.Write(1); // Car ID
                ack.Writer.Write(0x50); // Car type
                ack.Writer.Write(0); // Car color
                ack.Writer.Write(DateTime.Now - TimeSpan.FromDays(365)); // Creation Date
                ack.Writer.Write(character.TID); // Crew ID
                ack.Writer.Write(0L); // Crew Image
                ack.Writer.WriteUnicodeStatic("", 13); // Crew Name
                ack.Writer.Write((short)0); // Crew rank
                ack.Writer.Write((short)0); // Guild (0 = OMD, 1 = ROO)
            }

            packet.Sender.Send(ack);

            Log.WriteLine("Sent character list.");
        }

        [RicePacket(80, RiceServer.ServerType.Lobby)]
        public static void CheckCharacterName(RicePacket packet)
        {
            string characterName = packet.Reader.ReadUnicode();

            var ack = new RicePacket(81);
            ack.Writer.WriteUnicodeStatic(characterName, 21);
            ack.Writer.Write(Rice.Game.Character.IsNameUsable(characterName) ? 1 : 0); // Availability. 1 = Available, 0 = Unavailable.
            packet.Sender.Send(ack);
        }

        [RicePacket(82, RiceServer.ServerType.Lobby)]
        public static void CreateCharacter(RicePacket packet)
        {
            string characterName = packet.Reader.ReadUnicodeStatic(21);
            ushort avatar = packet.Reader.ReadUInt16();
            // We disregard the next 8 bytes (ulong CarType, ulong Color)

            if(packet.Sender.Player.Characters.Count() < 3 && Rice.Game.Character.IsNameUsable(characterName))
            {
                if (Rice.Game.Character.Create(packet.Sender.Player.User.UID, characterName, avatar))
                {
                    Rice.Game.Character character = Rice.Game.Character.Retrieve(characterName);
                    if (character != null)
                    {
                          packet.Sender.Player.Characters.Add(character);
                          var ack = new RicePacket(83);
                          ack.Writer.WriteUnicodeStatic(characterName, 21);
                          ack.Writer.Write(character.CID); //High hopes there, NPLUTO
                          ack.Writer.Write(character.CurrentCarID); //???
                          packet.Sender.Send(ack);
                    }
                }
            }
            // TODO: Verify, Handle
        }

        [RicePacket(50, RiceServer.ServerType.Lobby)]
        public static void DeleteCharacter(RicePacket packet)
        {
            string characterName = packet.Reader.ReadUnicode();
            foreach(var character in packet.Sender.Player.Characters)
            {
                if (character.Name == characterName)
                {
                    if (Rice.Game.Character.Delete(characterName))
                    {
                        packet.Sender.Player.Characters.Remove(character);
                        var ack = new RicePacket(84);
                        ack.Writer.WriteUnicodeStatic(characterName, 21);
                        packet.Sender.Send(ack);
                        break;
                    }
                    break;
                }
            }
        }
    }
}

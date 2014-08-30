using Rice.Server.Structures;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rice.Game
{
    public class Character
    {
        public User User;
        public ulong UID;
        public ulong CID;

        public string Name;
        public long Mito;
        public int Avatar;
        public int Level;
        public int City;
        public int CurrentCarID;
        public int GarageLevel;
        public long TID;

        private Character()
        {
        }

        public CharInfo GetInfo()
        {
            return new CharInfo
            {
                Avatar = (ushort)Avatar,
                Name = Name,
                CID = CID,
                City = City,
                CurrentCarID = CurrentCarID,
                ExpInfo = new ExpInfo // load this
                {
                    BaseExp = 0,
                    CurExp = 0,
                    NextExp = 0
                },
                HancoinGarage = GarageLevel,
                Level = (ushort)Level,
                TeamId = TID,
                TeamName = "Staff", // load this
                MitoMoney = Mito
            };
        }

        public static Character Retrieve(string charname)
        {
            DbConnection dbconn = Database.GetConnection();

            DbCommand command = dbconn.CreateTextCommand("SELECT * FROM Characters WHERE Name = @char");

            command.AddParameter("@char", charname);

            Character character = null;

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    character = new Character();
                    character.CID = Convert.ToUInt64(reader["CID"]);
                    character.UID = Convert.ToUInt64(reader["UID"]);
                    character.Name = reader["Name"] as string;
                    character.Mito = Convert.ToInt64(reader["Mito"]);
                    character.Avatar = Convert.ToInt32(reader["Avatar"]);
                    character.Level = Convert.ToInt32(reader["Level"]);
                    character.City = Convert.ToInt32(reader["City"]);
                    character.CurrentCarID = Convert.ToInt32(reader["CurrentCarID"]);
                    character.GarageLevel = Convert.ToInt32(reader["GarageLevel"]);
                    character.TID = Convert.ToInt64(reader["TID"]);
                }
            }

            return character;
        }

        public static List<Character> Retrieve(ulong uid)
        {
            DbConnection dbconn = Database.GetConnection();

            DbCommand command = dbconn.CreateTextCommand("SELECT * FROM Characters WHERE UID = @uid");

            command.AddParameter("@uid", uid);

            List<Character> chars = new List<Character>();

            using (DbDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var character = new Character();
                    character.CID = Convert.ToUInt64(reader["CID"]);
                    character.UID = Convert.ToUInt64(reader["UID"]);
                    character.Name = reader["Name"] as string;
                    character.Mito = Convert.ToInt64(reader["Mito"]);
                    character.Avatar = Convert.ToInt32(reader["Avatar"]);
                    character.Level = Convert.ToInt32(reader["Level"]);
                    character.City = Convert.ToInt32(reader["City"]);
                    character.CurrentCarID = Convert.ToInt32(reader["CurrentCarID"]);
                    character.GarageLevel = Convert.ToInt32(reader["GarageLevel"]);
                    character.TID = Convert.ToInt64(reader["TID"]);
                    chars.Add(character);
                }
            }

            return chars;
        }
    }
}

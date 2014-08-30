using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rice.Game
{
    public enum UserStatus : byte
    {
        Invalid = 0,
        Normal = 1,
        Banned = 2,
        Muted = 3
    }

    public class User
    {
        public ulong UID;
        public string Name;
        public string PasswordHash;
        public UserStatus Status;
        public string CreateIP;

        private User()
        {
        }

        public static User Empty
        {
            get
            {
                return new User() { Status = UserStatus.Normal };
            }
        }

        public static User Retrieve(string username, string passwordHash)
        {
            DbConnection dbconn = Database.GetConnection();

            DbCommand command = dbconn.CreateTextCommand("SELECT * FROM Users WHERE Username = @user AND PasswordHash = @pwhash");

            command.AddParameter("@user", username);
            command.AddParameter("@pwhash", passwordHash);

            Log.WriteLine("Username: {0}, PasswordHash: {1}", username, passwordHash);

            User user = null;

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    user = new User();
                    user.UID = Convert.ToUInt64(reader["UID"]);
                    user.Name = reader["Username"] as string;
                    user.PasswordHash = reader["PasswordHash"] as string;
                    user.Status = (UserStatus)Convert.ToByte(reader["Status"]);
                    user.CreateIP = reader["CreateIP"] as string;
                }
            }

            return user;
        }

        public static User Retrieve(string username)
        {
            DbConnection dbconn = Database.GetConnection();

            DbCommand command = dbconn.CreateTextCommand("SELECT * FROM Users WHERE Username = @user");

            command.AddParameter("@user", username);

            User user = null;

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    user = new User();
                    user.UID = Convert.ToUInt64(reader["UID"]);
                    user.Name = reader["Username"] as string;
                    user.PasswordHash = reader["PasswordHash"] as string;
                    user.Status = (UserStatus)Convert.ToByte(reader["Status"]);
                    user.CreateIP = reader["CreateIP"] as string;
                }
            }

            return user;
        }

        public static User Retrieve(ulong uid)
        {
            DbConnection dbconn = Database.GetConnection();

            DbCommand command = dbconn.CreateTextCommand("SELECT * FROM Users WHERE UID = @uid");

            command.AddParameter("@uid", uid);

            User user = null;

            using (DbDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    user = new User();
                    user.UID = Convert.ToUInt64(reader["UID"]);
                    user.Name = reader["Username"] as string;
                    user.PasswordHash = reader["PasswordHash"] as string;
                    user.Status = (UserStatus)Convert.ToByte(reader["Status"]);
                    user.CreateIP = reader["CreateIP"] as string;
                }
            }

            return user;
        }
    }
}

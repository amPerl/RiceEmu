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
        public string Username;
        public string PasswordHash;
        public UserStatus Status;
        public string CreateIP;

        private User()
        {
        }

        public static User Retrieve(string username, string passwordHash)
        {
            DbConnection dbconn = Database.GetConnection();

            DbCommand command = dbconn.CreateTextCommand("SELECT * FROM Users WHERE Username = @user AND PasswordHash = @pwhash");

            command.AddParameter("@user", username);
            command.AddParameter("@pwhash", passwordHash);

            Log.WriteLine("Username: {0}, PasswordHash: {1}", username, passwordHash);

            DbDataReader reader = command.ExecuteReader();
            User user = null;

            if (reader.Read())
            {
                user = new User();
                user.UID = Convert.ToUInt64(reader["UID"]);
                user.Username = reader["Username"] as string;
                user.PasswordHash = reader["PasswordHash"] as string;
                user.Status = (UserStatus)Convert.ToByte(reader["Status"]);
                user.CreateIP = reader["CreateIP"] as string;
            }

            return user;
        }
    }
}

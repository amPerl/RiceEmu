using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rice.Server.Database;
using Models = Rice.Server.Database.Models;

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
        public uint Credits;

        private User(Models.User dbUser = null)
        {
            if (dbUser != null)
            {
                this.UID = (ulong)dbUser.ID;
                this.Name = dbUser.Name;
                this.PasswordHash = dbUser.PasswordHash;
                this.Status = (UserStatus)dbUser.Status;
                this.CreateIP = dbUser.CreateIP;
                this.Credits = (uint)dbUser.Credits;
            }
        }

        public static User Empty => new User { Status = UserStatus.Normal };

        public static User Retrieve(string username, string passwordHash)
        {
            Log.WriteLine("Username: {0}, PasswordHash: {1}", username, passwordHash);

            User user;
            
            using (var rc = Database.GetContext())
                user = new User(rc.Users.FirstOrDefault(u => u.Name == username && u.PasswordHash == passwordHash));

            return user;
        }

        public static User Retrieve(string username)
        {
            User user;

            using (var rc = Database.GetContext())
                user = new User(rc.Users.FirstOrDefault(u => u.Name == username));

            return user;
        }

        public static User Retrieve(ulong uid)
        {
            User user;

            using (var rc = Database.GetContext())
                user = new User(rc.Users.Find((long)uid));

            return user;
        }
    }
}

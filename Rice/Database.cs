using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using Rice.Server.Database.Models;

namespace Rice
{
    public static class DatabaseExtensions
    {
        public static DbCommand CreateTextCommand(this DbConnection dbconn, string query)
        {
            DbCommand command = dbconn.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = query;
            return command;
        }

        public static void AddParameter(this DbCommand command, string name, object value)
        {
            DbParameter param = command.CreateParameter();
            param.ParameterName = name;
            param.Value = value;
            command.Parameters.Add(param);
        }
    }

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class RiceContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Character> Characters { get; set; }
        public virtual DbSet<QuestState> QuestStates { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Vehicle> Vehicles { get; set; }

        public RiceContext(DbConnection connection, bool ownsConnection = false) : base(connection, ownsConnection) { }
    }

    public static class Database
    {
        static DbConnection conn;

        public static void Initialize(Config config)
        {
            conn = new MySqlConnection(String.Format(
                "Persist Security Info=True; Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
                config.DatabaseHost,
                config.DatabasePort,
                config.DatabaseName,
                config.DatabaseUser,
                config.DatabasePassword
            ));
        }

        public static void Start()
        {
            Log.WriteLine("Connecting to database.");

            try
            {
                using (var rc = new RiceContext(conn))
                {
                    if (rc.Database.CreateIfNotExists())
                    {
                        Log.WriteLine("Database did not exist, created.");
                        
                        // Create some test data to work with
                        var testUser = new User
                        {
                            Name = "admin",
                            PasswordHash = Utilities.MD5("admin"),
                            CreateIP = "127.0.0.1",
                            Status = 1,
                            Credits = 1000
                        };

                        var testChar = new Character
                        {
                            Name = "RiceAdmin",
                            Mito = 12345678910,
                            Avatar = 2,
                            Level = 123,
                            Experience = 123,
                            City = 1,
                            TID = -1
                        };

                        testUser.Characters = new List<Character> {testChar};

                        var testVehicle = new Vehicle
                        {
                            CarID = 1,
                            AuctionCount = 0,
                            CarType = 88,
                            Color = 0,
                            Grade = 9,
                            Kms = 200,
                            Mitron = 5550f
                        };

                        testChar.Vehicles = new List<Vehicle> {testVehicle};
                        testChar.CurrentCarID = testVehicle.CarID;

                        rc.Users.Add(testUser);
                        rc.SaveChanges();
                    }
                }
                conn.Open();
                Log.WriteLine("Connected to database.");
            }
            catch (MySqlException ex)
            {
                Log.WriteError($"Connection failed: {ex.Message}");
            }
        }

        private static DbConnection GetConnection()
        {
            if (conn.State == ConnectionState.Broken)
                conn.Close();
            if (conn.State == ConnectionState.Closed)
                Start();
            return conn;
        }

        public static RiceContext GetContext() => new RiceContext(GetConnection());
    }
}

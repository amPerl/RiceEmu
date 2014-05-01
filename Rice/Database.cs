using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

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

    public static class Database
    {
        static DbConnection conn;

        public static void Initialize(Config config)
        {
            conn = new MySqlConnection(
                String.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};",
                config.DatabaseHost,
                config.DatabasePort,
                config.DatabaseName,
                config.DatabaseUser,
                config.DatabasePassword));
        }

        public static void Start()
        {
            Log.WriteLine("Connecting to database.");

            try
            {
                conn.Open();
                Log.WriteLine("Connected to database.");
            }
            catch (Exception ex)
            {
                Log.WriteError("Connection failed: {0}", ex.Message);
            }
        }

        public static DbConnection GetConnection()
        {
            return conn;
        }
    }
}

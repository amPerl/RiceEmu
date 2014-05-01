using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Rice.Game;

namespace Rice.Server.Core
{
    public static class RiceServer
    {
        [Flags]
        public enum ServerType
        {
            Auth = 1,
            Lobby = 2,
            Game = 4,
            Area = 8,
            Ranking = 16
        }

        public static RiceListener Auth, Lobby, Game, Area, Ranking;
        public static Config Config;
        public static Random RNG;

        static List<Player> players;

        public static void Initialize(Config config)
        {
            Log.WriteLine("Initializing RiceServer.");

            RiceServer.Config = config;

            Auth = new RiceListener(Config.AuthPort);
            Lobby = new RiceListener(Config.LobbyPort);
            Game = new RiceListener(Config.GamePort);
            Area = new RiceListener(Config.AreaPort, false);
            Ranking = new RiceListener(Config.RankingPort);

            players = new List<Player>();
            RNG = new Random();

            loadParsers();
        }

        private static void loadParsers()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    foreach (var attrib in method.GetCustomAttributes<RicePacketAttribute>())
                    {
                        var id = attrib.ID;
                        var parser = (Action<RicePacket>)method.CreateDelegate(typeof(Action<RicePacket>));

                        if (attrib.Handlers.HasFlag(ServerType.Auth))
                            Auth.SetParser(id, parser);

                        if (attrib.Handlers.HasFlag(ServerType.Lobby))
                            Lobby.SetParser(id, parser);

                        if (attrib.Handlers.HasFlag(ServerType.Game))
                            Game.SetParser(id, parser);

                        if (attrib.Handlers.HasFlag(ServerType.Area))
                            Area.SetParser(id, parser);

                        if (attrib.Handlers.HasFlag(ServerType.Ranking))
                            Ranking.SetParser(id, parser);
                    }
                }
            }

        }

        public static void Start()
        {
            Log.WriteLine("Starting RiceServer.");

            Auth.Start();
            Lobby.Start();
            Game.Start();
            Area.Start();
            Ranking.Start();

            Log.WriteLine("RiceServer started.");
        }

        public static Player[] GetPlayers()
        {
            return players.ToArray();
        }

        public static void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public static void RemovePlayer(Player player)
        {
            players.Add(player);
        }

        public static uint CreateTicket()
        {
            Player[] players = GetPlayers();

            bool exists = false;
            uint ticket = 0;

            do
            {
                exists = false;
                ticket = (uint)RNG.Next();

                foreach (Player p in players)
                {
                    if (p.Ticket == ticket)
                    {
                        exists = true;
                        break;
                    }
                }
            }
            while (exists);

            return ticket;
        }
    }
}
using System;
using System.Linq;
using System.Net;
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

        static List<Serial> validSerials = new List<Serial>();
        static List<Ticket> validTickets = new List<Ticket>();

        private static List<Area> areas; 
        private static List<Player> players;

        public static long[] ExpTable, ExpSumTable;

        static ushort serialCounter = 1;

        public static void Initialize(Config config)
        {
            Log.WriteLine("Initializing RiceServer.");

            Config = config;

            Auth = new RiceListener("Auth", Config.AuthPort);
            Lobby = new RiceListener("Lobby", Config.LobbyPort);
            Game = new RiceListener("Game", Config.GamePort);
            Area = new RiceListener("Area", Config.AreaPort, false);
            Ranking = new RiceListener("Ranking", Config.RankingPort);

            players = new List<Player>();
            areas = new List<Area>();
            RNG = new Random();

            ExpTable = new long[150];
            ExpSumTable = new long[ExpTable.Length];
            long sum = 0;
            for (int i = 1; i < ExpTable.Length; i++)
            {
                double missionTime = Math.Pow(i, 2.25) * 1.5;
                double realGrind = Math.Pow(i, 20) / Math.Pow(10, 33);
                long exp = (long) (100 + missionTime + realGrind);
                ExpTable[i] = exp;
                sum += exp;
                ExpSumTable[i] = sum;
            }

            loadParsers();
        }

        private static void loadParsers()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    foreach (var boxedAttrib in method.GetCustomAttributes(typeof(RicePacketAttribute), false))
                    {
                        var attrib = boxedAttrib as RicePacketAttribute;

                        var id = attrib.ID;
                        var parser = (Action<RicePacket>)Delegate.CreateDelegate(typeof(Action<RicePacket>), method);

                        if (attrib.Handlers.HasFlag(ServerType.Auth))
                            Auth.SetParser(id, parser, attrib.CheckedIn);

                        if (attrib.Handlers.HasFlag(ServerType.Lobby))
                            Lobby.SetParser(id, parser, attrib.CheckedIn);

                        if (attrib.Handlers.HasFlag(ServerType.Game))
                            Game.SetParser(id, parser, attrib.CheckedIn);

                        if (attrib.Handlers.HasFlag(ServerType.Area))
                            Area.SetParser(id, parser, attrib.CheckedIn);

                        if (attrib.Handlers.HasFlag(ServerType.Ranking))
                            Ranking.SetParser(id, parser, attrib.CheckedIn);
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
            players.Remove(player);
        }

        public static Serial CreateSerial(RiceClient client)
        {
            var serial = new Serial(client) {Identifier = serialCounter++};
            validSerials.Add(serial);
            return serial;
        }

        public static Ticket CreateTicket(RiceClient client)
        {
            var ticket = new Ticket(client);

            do
            {
                uint ticketId = (uint)RNG.Next();

                if (validTickets.FirstOrDefault(t => t.Identifier == ticketId) != null) continue;

                ticket.Identifier = ticketId;
                validTickets.Add(ticket);
                return ticket;
            }
            while (true);
        }

#if DEBUG
        public static Ticket CreateDebugTicket(RiceClient client, uint ticketId)
        {
            Ticket ticket = new Ticket(client)
            {
                Identifier = ticketId
            };
            validTickets.Add(ticket);
            return ticket;
        }
#endif

        public static Ticket GetTicket(uint ticketId)
        {
            return validTickets.FirstOrDefault(t => t.Identifier == ticketId);
        }

        public static void InvalidateTicket(uint ticketId)
        {
            var ticket = GetTicket(ticketId);
            if (ticket == null) return;

            validTickets.Remove(ticket);
        }

        public static Serial GetSerial(ushort serialId)
        {
            return validSerials.FirstOrDefault(s => s.Identifier == serialId);
        }

        public static void InvalidateSerial(ushort serialId)
        {
            var serial = GetSerial(serialId);
            if (serial == null) return;

            validSerials.Remove(serial);
        }

        public static Area GetArea(int id)
        {
            var area = areas.FirstOrDefault(a => a.ID == id);
            if (area != null) return area;

            area = new Area(id);
            areas.Add(area);
            return area;
        }

        public static Area[] GetAreas()
        {
            return areas.ToArray();
        }
    }
}
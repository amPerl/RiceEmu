using System;
using System.Reflection;

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
            Area = 8
        }

        public static RiceListener Auth, Lobby, Game, Area;

        public static void Initialize()
        {
            Log.WriteLine("Initializing RiceServer.");

            Auth = new RiceListener(Config.AuthPort);
            Lobby = new RiceListener(Config.LobbyPort);
            Game = new RiceListener(Config.GamePort);
            Area = new RiceListener(Config.AreaPort, false);

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
        }
    }
}
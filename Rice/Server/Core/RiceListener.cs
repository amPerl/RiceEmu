using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Rice.Server.Core
{
    public class RiceListener
    {
#if DEBUG
        private static Dictionary<ushort, string> debugNameDatabase;
#endif

        private Dictionary<ushort, Action<RicePacket>> parsers;
        private List<ushort> checkInParsers; 
        private List<RiceClient> clients;

        private int port;
        private TcpListener listener;
        private bool exchangeRequired;

        public string Name;

        public RiceListener(string name, int port, bool exchangeRequired = true)
        {
#if DEBUG
            if (debugNameDatabase == null)
            {
                debugNameDatabase = new Dictionary<ushort, string>();

                string[] src = File.ReadAllLines("Packets.txt");
                foreach (var line in src)
                {
                    if (line.Length <= 3) continue;
                    string[] lineSplit = line.Split(':');

                    ushort id = ushort.Parse(lineSplit[0]);

                    debugNameDatabase[id] = lineSplit[1].Trim();
                }
            }
#endif
            Name = name;
            parsers = new Dictionary<ushort, Action<RicePacket>>();
            checkInParsers = new List<ushort>();
            clients = new List<RiceClient>();

            this.port = port;
            this.listener = new TcpListener(IPAddress.Any, port);
            this.exchangeRequired = exchangeRequired;
        }

        public void Start()
        {
            listener.Start();
            listener.BeginAcceptTcpClient(onAccept, this.listener);

            //Log.WriteLine("Started listening on {0}", port);
        }

        private void onAccept(IAsyncResult result)
        {
            var tcpClient = listener.EndAcceptTcpClient(result);
            var riceClient = new RiceClient(tcpClient, this, exchangeRequired);

            Log.WriteLine("Accepted client from {0} on {1}", tcpClient.Client.RemoteEndPoint, port);

            clients.Add(riceClient);
            listener.BeginAcceptTcpClient(onAccept, this.listener);
        }

        public void SetParser(ushort id, Action<RicePacket> parser, bool needsCheckIn)
        {
            //Log.WriteLine("Added parser for packet {0} on {1}.", id, port);
            parsers[id] = parser;
            if (needsCheckIn)
                checkInParsers.Add(id);
        }

        public void Parse(RicePacket packet)
        {
            //Log.WriteLine("{0} parsing {1}.", port, packet.ID);

            if (parsers.ContainsKey(packet.ID))
            {
                if (checkInParsers.Contains(packet.ID) && packet.Sender.Player == null)
                {
                    Log.WriteLine("Autokilling {0} sender on {1}: CheckIn requirement not met.");
                    packet.Sender.Kill("Forbidden");
                    return;
                }

                parsers[packet.ID](packet);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
#if DEBUG
                if (debugNameDatabase.ContainsKey(packet.ID))
                    Log.WriteLine("Received {2} (id {0}, {0:X}) on {1}.", packet.ID, port, debugNameDatabase[packet.ID]);
                else
                    Log.WriteLine("Received unknown packet (id {0}, {0:X}) on {1}.", packet.ID, port);
#else
                Log.WriteLine("Received unknown packet (id {0}, {0:X}) on {1}.", packet.ID, port);
#endif
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        public RiceClient[] GetClients()
        {
            return clients.ToArray();
        }

        public void Broadcast(RicePacket packet, RiceClient exclude = null)
        {
            foreach (var client in GetClients())
            {
                if (exclude == null || client != exclude)
                    client.Send(packet);
            }
        }
    }
}
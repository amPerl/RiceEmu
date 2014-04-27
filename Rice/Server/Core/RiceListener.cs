using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Rice.Server.Core
{
    public class RiceListener
    {
        private Dictionary<ushort, Action<RicePacket>> parsers;
        private List<RiceClient> clients;

        private int port;
        private TcpListener listener;
        private bool exchangeRequired;

        public RiceListener(int port, bool exchangeRequired = true)
        {
            parsers = new Dictionary<ushort, Action<RicePacket>>();
            clients = new List<RiceClient>();
            this.port = port;
            this.listener = new TcpListener(IPAddress.Any, port);
            this.exchangeRequired = exchangeRequired;
        }

        public void Start()
        {
            listener.Start();
            listener.BeginAcceptTcpClient(onAccept, this.listener);

            Log.WriteLine("Started listening on {0}", port);
        }

        private void onAccept(IAsyncResult result)
        {
            var tcpClient = listener.EndAcceptTcpClient(result);
            var riceClient = new RiceClient(tcpClient, this, exchangeRequired);

            Log.WriteLine("Accepted client from {0}", tcpClient.Client.RemoteEndPoint);

            clients.Add(riceClient);
            listener.BeginAcceptTcpClient(onAccept, this.listener);
        }

        public void SetParser(ushort id, Action<RicePacket> parser)
        {
            Log.WriteLine("Added parser for packet {0} on {1}.", id, port);
            parsers[id] = parser;
        }

        public void Parse(RicePacket packet)
        {
            Log.WriteLine("{0} parsing {1}.", port, packet.ID);

            if (parsers.ContainsKey(packet.ID))
                parsers[packet.ID](packet);
            else
            {
                Log.WriteLine("Received unknown packet (id {0}) on {1}.", packet.ID, port);
            }
        }

        public RiceClient[] GetClients()
        {
            return clients.ToArray();
        }
    }
}
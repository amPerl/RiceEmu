using System;
using System.Net.Sockets;

namespace Rice.Server.Core
{
    public class RiceClient
    {
        private TcpClient tcp;
        private NetworkStream ns;
        private RiceListener parent;
        private bool exchangeRequired;

        private byte[] buffer;
        private int bytesToRead;
        private ushort packetLength, packetID;

        public RiceClient(TcpClient tcp, RiceListener parent, bool exchangeRequired)
        {
            this.tcp = tcp;
            this.parent = parent;
            this.exchangeRequired = exchangeRequired;

            ns = tcp.GetStream();

            try
            {
                if (exchangeRequired)
                {
                    buffer = new byte[56];
                    bytesToRead = buffer.Length;
                    ns.BeginRead(buffer, 0, 56, onExchange, null);
                }
                else
                {
                    buffer = new byte[4];
                    bytesToRead = buffer.Length;
                    ns.BeginRead(buffer, 0, 4, onHeader, null);
                }
            }
            catch { Kill(); }
        }

        private void onExchange(IAsyncResult result)
        {
            try
            {
                bytesToRead -= ns.EndRead(result);
                if (bytesToRead > 0)
                {
                    ns.BeginRead(buffer, buffer.Length - bytesToRead, bytesToRead, onExchange, null);
                    return;
                }

                ns.Write(new byte[56], 0, 56); // The client will disable crypto.
                Log.WriteLine("Returned exchange.");

                buffer = new byte[4];
                bytesToRead = buffer.Length;
                ns.BeginRead(buffer, 0, 4, onHeader, null);
            }
            catch { Kill(); }
        }

        private void onHeader(IAsyncResult result)
        {
            try
            {
                bytesToRead -= ns.EndRead(result);
                if (bytesToRead > 0)
                {
                    ns.BeginRead(buffer, buffer.Length - bytesToRead, bytesToRead, onHeader, null);
                    return;
                }

                packetLength = BitConverter.ToUInt16(buffer, 0);
                packetID = BitConverter.ToUInt16(buffer, 2);

                Log.WriteLine("Received length: {0}", packetLength);

                bytesToRead = packetLength - 4;
                buffer = new byte[bytesToRead];
                ns.BeginRead(buffer, 0, bytesToRead, onData, null);
            }
            catch { Kill(); }
        }

        private void onData(IAsyncResult result)
        {
            try
            {
                bytesToRead -= ns.EndRead(result);
                if (bytesToRead > 0)
                {
                    ns.BeginRead(buffer, buffer.Length - bytesToRead, bytesToRead, onData, null);
                    return;
                }

                var packet = new RicePacket(this, packetID, buffer);
                parent.Parse(packet);

                buffer = new byte[4];
                bytesToRead = buffer.Length;
                ns.BeginRead(buffer, 0, 4, onHeader, null);
            }
            catch { Kill(); }
        }

        public void Send(RicePacket packet)
        {
            byte[] buffer = packet.Writer.GetBuffer();

            int bufferLength = buffer.Length;
            ushort length = (ushort)(bufferLength + 2); // Length includes itself

            try
            {
                ns.Write(BitConverter.GetBytes(length), 0, 2);
                ns.Write(buffer, 0, bufferLength);
            }
            catch { Kill(); }
        }

        public void Kill()
        {
            if (tcp.Connected)
            {
                tcp.Close();
            }

            Log.WriteLine("Killed off client {0}.", tcp.Client.RemoteEndPoint);
        }
    }
}
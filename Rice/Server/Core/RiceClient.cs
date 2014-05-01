using System;
using System.IO;
using System.Net.Sockets;
using Rice.Game;

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

        public bool Alive;
        public Player Player;

        public RiceClient(TcpClient tcp, RiceListener parent, bool exchangeRequired)
        {
            this.tcp = tcp;
            this.parent = parent;
            this.exchangeRequired = exchangeRequired;

            ns = tcp.GetStream();
            Alive = true;

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
            catch (Exception ex) { Kill(ex); }
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

                ns.Write(new byte[56], 0, 56);

                buffer = new byte[4];
                bytesToRead = buffer.Length;
                ns.BeginRead(buffer, 0, 4, onHeader, null);
            }
            catch (Exception ex) { Kill(ex); }
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

                bytesToRead = packetLength - 4;
                buffer = new byte[bytesToRead];
                ns.BeginRead(buffer, 0, bytesToRead, onData, null);
            }
            catch (Exception ex) { Kill(ex); }
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
            catch (Exception ex) { Kill(ex); }
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
            catch (Exception ex) { Kill(ex); }
        }

        public void Error(string format, params object[] args)
        {
            var err = new RicePacket(1);
            err.Writer.WriteUnicode(String.Format(format, args));
            Send(err);
        }

        public void Kill(Exception ex)
        {
            if (ex is SocketException || ex is IOException)
            {
                Kill(); 
                return;
            }

            Kill(ex.Message + ": " + ex.StackTrace);
        }

        public void Kill(string reason = "")
        {
            if (!Alive) return;
            Alive = false;

            Log.WriteLine("Killing off client. {0}", reason);
            tcp.Close();
        }
    }
}
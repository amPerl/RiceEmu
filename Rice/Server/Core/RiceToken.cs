using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Rice.Game;
using Rice.Server.Structures;

namespace Rice.Server.Core
{
    public abstract class RiceToken
    {
        private readonly Player originPlayer;
        private readonly string originIP;

        protected RiceToken(RiceClient client)
        {
            originPlayer = client.Player;
            originIP = client.GetRemoteIP();
        }

        public bool ValidateOrigin(RiceClient client, string username = null)
        {
            if (originIP != client.GetRemoteIP())
            {
                Log.WriteError("Failed to validate origin IP of token. Origin: {0}, Client: {1}", originIP, client.GetRemoteIP());
                return false;
            }

            if (username != null)
            {
                if (originPlayer.User.Name.ToLower() == username.ToLower())
                    return true;

                Log.WriteError("Failed to validate origin Username of token. Origin: {0}, Client: {1}", originPlayer.User.Name, username);
                return false;
            }
            return true;
        }

        public Player GetOwner()
        {
            return originPlayer;
        }
    }

    public class Ticket : RiceToken, ISerializable
    {
        public uint Identifier;
        public Ticket(RiceClient client) : base(client) { }

        public void Serialize(PacketWriter pw)
        {
            pw.Write(Identifier);
        }
    }

    public class Serial : RiceToken, ISerializable
    {
        public ushort Identifier;
        public Serial(RiceClient client) : base(client) { }

        public void Serialize(PacketWriter pw)
        {
            pw.Write(Identifier);
        }
    }
}

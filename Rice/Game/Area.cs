using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rice.Server.Core;

namespace Rice.Game
{
    public class Area
    {
        public int ID;
        private List<Player> players;

        public Area(int id)
        {
            ID = id;
            players = new List<Player>();
        }

        public void AddPlayer(Player player)
        {
            if (players.Contains(player))
            {
                Log.WriteLine("Weird: Player already in area.");
                RemovePlayer(player);
            }
            players.Add(player);
            player.ActiveCharacter.Area = this;
            Log.WriteLine("Added player {0} to area {1}.", player.ActiveCharacter.Name, ID);
        }

        public void RemovePlayer(Player player)
        {
            players.Remove(player);
            player.ActiveCharacter.Area = null;
            Log.WriteLine("Removed player {0} from area {1}.", player.ActiveCharacter.Name, ID);
        }

        public Player[] GetPlayers()
        {
            return players.ToArray();
        }

        public int GetPlayerCount()
        {
            return players.Count;
        }

        public void BroadcastMovement(Player player, byte[] movement) // byte[] temporarily
        {
            var move = new RicePacket(0x21D); // 114 total length
            move.Writer.Write(player.ActiveCharacter.Serial);
            move.Writer.Write(movement);

            foreach (Player areaPlayer in players)
            {
                if (areaPlayer != player)
                    areaPlayer.AreaClient.Send(move);
            }
        }

        public void Broadcast(RicePacket packet, RiceClient exclude = null)
        {
            foreach (var player in GetPlayers())
            {
                var client = player.AreaClient;

                if (exclude == null || client != exclude)
                    client.Send(packet);
            }
        }
    }
}

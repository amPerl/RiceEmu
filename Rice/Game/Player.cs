using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rice.Server.Core;

namespace Rice.Game
{
    public class Player
    {
        public uint Ticket;

        public User User;
        public List<Character> Characters;
        public Character ActiveCharacter;

        private RiceClient authClient, lobbyClient, areaClient, gameClient;
        public RiceClient AuthClient { get { return authClient; } set { authClient = value; authClient.Player = this; } }
        public RiceClient LobbyClient { get { return lobbyClient; } set { lobbyClient = value; lobbyClient.Player = this; } }
        public RiceClient AreaClient { get { return areaClient; } set { areaClient = value; areaClient.Player = this; } }
        public RiceClient GameClient { get { return gameClient; } set { gameClient = value; gameClient.Player = this; } }

        public Player(User user)
        {
            User = user;
            Characters = new List<Character>();
        }
    }
}

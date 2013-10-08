using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class PlayerCircle
    {
        private PlayerState[] players;   // circular list, higher numbers to the left;
        private int currentPlayerIndex;

        public PlayerCircle(int playerCount, IPlayerAction[] players, IGameLog gameLog, Random random, CardGameSubset gameSubset)
        {
            this.players = new PlayerState[playerCount];
            for (int playerIndex = 0; playerIndex < this.players.Length; ++playerIndex)
            {
                this.players[playerIndex] = new PlayerState(players[playerIndex], playerIndex, gameLog, random, gameSubset);
            }

            this.currentPlayerIndex = 0;
        }

        public PlayerState CurrentPlayer
        {
            get
            {
                return this.players[this.currentPlayerIndex];
            }
        }

        public PlayerState PlayerLeft
        {
            get
            {
                return this[1];
            }
        }

        public PlayerState PlayerRight
        {
            get
            {
                return this[-1];
            }
        }

        public bool BeginningOfRound
        {
            get
            {
                return this.currentPlayerIndex == 0;
            }
        }

        public int PlayerCount
        {
            get
            {
                return this.players.Length;
            }
        }

        public void PassTurnLeft()
        {
            this.currentPlayerIndex++;
            if (this.currentPlayerIndex >= this.players.Count())
            {
                this.currentPlayerIndex = 0;
            }
        }

        public PlayerState this[int index]
        {
            get
            {
                int playerIndex = (this.currentPlayerIndex + index) % this.players.Length;
                return players[playerIndex];
            }
        }

        public void AllPlayersDrawInitialCards()
        {
            foreach (PlayerState playerState in this.players)
            {                
                playerState.DrawUntilCountInHand(5);
            }
        }

        public IEnumerable<PlayerState> AllPlayers
        {
            get
            {
                for (int playerIndex = 0; playerIndex < this.PlayerCount; ++playerIndex)
                {
                    PlayerState nextPlayer = this.players[playerIndex];

                    yield return nextPlayer;
                }
            }
        }

        public IEnumerable<PlayerState> OtherPlayers
        {
            get
            {
                // 1 is first player to the left
                for (int playerIndex = 1; playerIndex < this.PlayerCount; ++playerIndex)
                {
                    PlayerState nextPlayer = this[playerIndex];

                    yield return nextPlayer;
                }
            }
        }
    }
}

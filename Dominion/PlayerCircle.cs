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

        public PlayerCircle(int playerCount, IPlayerAction[] playerActions, int[] playerPosition, Game game)
        {
            this.players = new PlayerState[playerCount];
            for (int playerIndex = 0; playerIndex < this.players.Length; ++playerIndex)
            {
                int playPosition = playerPosition[playerIndex];
                IPlayerAction playerAction = playerActions[playerIndex];
                this.players[playPosition] = new PlayerState(playerAction, playerIndex, game);
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
            if (this.currentPlayerIndex >= this.players.Length)
            {
                this.currentPlayerIndex = 0;
            }
        }

        public PlayerState this[int index]
        {
            get
            {
                if (index < 0)
                    index += this.players.Length;

                int playerIndex = (this.currentPlayerIndex + index) % this.players.Length;
                return players[playerIndex];
            }
        }

        public void AllPlayersDrawInitialCards(GameConfig gameConfig)
        {
            foreach (PlayerState playerState in this.players)
            {                
                IEnumerable<CardCountPair> startingHand = gameConfig.StartingHand(playerState.PlayerIndex);

                if (startingHand == null)
                {
                    playerState.DrawUntilCountInHand(5);                    
                }
                else
                {
                    playerState.DrawCardsIntoHand(startingHand);
                }
            }            
        }

        public PlayerState[] OriginalPlayerOrder
        {
            get
            {
                return this.players;
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

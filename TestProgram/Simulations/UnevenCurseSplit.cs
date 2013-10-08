using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Simulations
{
    static public class UnevenCurseSplit
    {
        public static void Run()
        {         
            int gameCount = 100000;

            System.Console.WriteLine("Out of {0} games", gameCount);
            System.Console.WriteLine();

            ComparePlayers(gameCount, null);
                        
            System.Console.WriteLine("");
            System.Console.WriteLine("When the players have have a Silver/Silver opening, followed by Witch/Silver, Witch/Silver hands ...");
            System.Console.WriteLine("");
            
            ComparePlayers(gameCount,
                new CardCountPair[] {
                    new CardCountPair(Card.Type<CardTypes.Copper>(), 7),
                    new CardCountPair(Card.Type<CardTypes.Estate>(), 3),
                    new CardCountPair(Card.Type<CardTypes.Silver>(), 4),
                    new CardCountPair(Card.Type<CardTypes.Witch>(), 2),
                    new CardCountPair(Card.Type<CardTypes.Curse>(), 1)
                });

            System.Console.WriteLine("");
            System.Console.WriteLine("When the players each have a Silver/Silver opening, followed by Witch/Witch ...");
            System.Console.WriteLine("");

                        
            ComparePlayers(gameCount,
                new CardCountPair[] {
                    new CardCountPair(Card.Type<CardTypes.Copper>(), 7),
                    new CardCountPair(Card.Type<CardTypes.Estate>(), 3),
                    new CardCountPair(Card.Type<CardTypes.Silver>(), 2),
                    new CardCountPair(Card.Type<CardTypes.Witch>(), 2)                
                });
        }

        private static void ComparePlayers(int gameCount, CardCountPair[] startingCards)
        {
            var gameLogFactory = new GameLogFactory();

            Program.ComparePlayers(
                Strategies.BigMoneyWithCard<CardTypes.Witch>.Player(1, cardCount:2),
                Strategies.BigMoneyWithCard<CardTypes.Witch>.Player(2, cardCount:2),
                firstPlayerAdvantage: true,
                numberOfGames: gameCount,
                createGameLog: new Program.CreateGameLog(gameLogFactory.CreateGameLog),
                startingDeckPerPlayer: GameConfig.GetUniformStartingDecks(2, startingCards));

            System.Console.WriteLine("Curses Split was Uneven {0}%", gameLogFactory.UnEvenSplitPercent);
            System.Console.WriteLine();
            System.Console.WriteLine("When the curses were not split, Player 1 won the curse split {0}/{1}", gameLogFactory.Player1WinPercent, gameLogFactory.Player2WinPercent);
        }
    }

    class GameLogFactory
    {
        private object theLock = new object();
        private int player1Win = 0;
        private int player2Win = 0;
        private int totalGameCount = 0;        

        public float Player1WinPercent
        {
            get
            {
                return 100 * ((float)player1Win) / (player1Win + player2Win);
            }
        }

        public float Player2WinPercent
        {
            get
            {
                return 100 - Player1WinPercent;
            }
        }

        public float UnEvenSplitPercent
        {
            get
            {
                return 100 * ((float)(player1Win + player2Win)) / totalGameCount;
            }
        }

        public IGameLog CreateGameLog()
        {
            return new GameLog(this);
        }

        class GameLog
            : Dominion.EmptyGameLog
        {
            private bool[] was25split = new bool[2];
            private GameState gameState;
            private readonly GameLogFactory factory;

            public GameLog(GameLogFactory factory)            
            {
                this.factory = factory;
            }

            public override void StartGame(GameState gameState)
            {
                this.gameState = gameState;
            }

            public override void EndTurn(PlayerState playerState)
            {
                if (playerState.TurnNumber == 2)
                {
                    if (playerState.AllOwnedCards.Where(card => card.Is< CardTypes.Witch>()).Any())
                    {
                        this.was25split[((PlayerAction)playerState.Actions).playerIndex - 1] = true;
                    }
                }
            }

            public override void EndGame(GameState gameState)
            {
                lock (this.factory.theLock)
                {
                    if (gameState.GetPile<CardTypes.Curse>().Any)
                        return;

                    int player1Count = CountCurses(gameState, 0);
                    int player2Count = CountCurses(gameState, 1);

                    if (player1Count > player2Count)
                        this.factory.player1Win++;

                    if (player2Count > player1Count)
                        this.factory.player2Win++;

                    this.factory.totalGameCount++;
                }

                if (gameState.WinningPlayers.Count() > 1)
                {
                    int playerIndex = PlayerAction.PlayIndexfor(gameState.WinningPlayers[0].Actions);
                    //winningPlayerWas25++;
                }
            }

            private static int CountCurses(GameState gameState, int playerNumber)
            {
                return gameState.players[playerNumber].AllOwnedCards.Where(card => card.isCurse).Count();
            }
        }
    }
}

using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominion.Strategy;


namespace Program.Simulations
{
    static class OddsOfScryingPoolAndCrossroadsAllDraw
    {
        /*
         *  This code answers the following question.   It's player 1's turn.   No provinces have been bought yet - but he's down
         *  scrying pools (lost the split 4/6) and has a more non actions in his deck than the opponent.   Opponenent can 
         *  also double province on his turn.  If player1 starts a double province buy war, he will probably Not chain first and lose.
         *  So intead, though he can afford 2 provinces, opts for 1 province, 2 estates and 2 crossroads.
         *  This leaves the deck composition as described below, with scrying pool and festival pile already empty.  
         *  There are remaining 6 estates ending, the plan is on the next turn to chain enough of the deck to buy all 6 estates
         *  and 3 pile out the game.   What is the probability of this happening?  (need to draw all 5 festivals and 2 additional coin)
         * */
        public static void Run()
        {
            var player1 = ScryingPoolAndCrossroads.Player();
            var player2 = Strategies.BigMoney.Player();

            var builder = new GameConfigBuilder();
            builder.CardSplit = StartingCardSplit.Random;

            builder.SetStartingDeckPerPlayer(
                new Dominion.CardCountPair[][]
            {
                new Dominion.CardCountPair[]        // Player1
                {
                    new Dominion.CardCountPair(Cards.Estate, 5),
                    new Dominion.CardCountPair(Cards.Province, 1),
                    new Dominion.CardCountPair(Cards.Festival, 5),
                    new Dominion.CardCountPair(Cards.Silver, 2),
                    new Dominion.CardCountPair(Cards.Copper, 2),
                    new Dominion.CardCountPair(Cards.CrossRoads, 3),
                    new Dominion.CardCountPair(Cards.ScryingPool, 4),
                    new Dominion.CardCountPair(Cards.WanderingMinstrel, 1),
                    new Dominion.CardCountPair(Cards.Remake, 1),
                },
                new Dominion.CardCountPair[]        // Player2
                {
                    new Dominion.CardCountPair(Cards.Estate, 3),
                    new Dominion.CardCountPair(Cards.Copper, 7),
                }
            });

            PlayerAction.SetKingdomCards(builder, player1, player2);
            GameConfig gameConfig = builder.ToGameConfig();

            var playerActions = new PlayerAction[] { player1, player2 };

            int countWin = 0;           
            int countGame = 1000;
            for (int i = 0; i < countGame; ++i)
            {
                using (var indentedTextOutput = TestOutput.GetGameLogWriterForIteration(playerActions, i))
                {
                    var gameLog = new HumanReadableGameLog(indentedTextOutput);
                    using (Game game = new Game(new Random(i), gameConfig, gameLog))
                    {
                        GameState gameState = new GameState(playerActions, new int[] { 0, 1 }, game);
                        
                        PlayerState currentPlayer = gameState.players[0];

                        gameLog.BeginRound(currentPlayer);
                        gameState.PlayTurn(currentPlayer);                        
                        // 11 = 3 starting estates plus all 8 estates in the pile
                        if (currentPlayer.AllOwnedCards.CountOf(Cards.Estate) == 11)
                        {
                            countWin++;
                            System.Console.WriteLine("Won Game {0}", i);
                        }
                    }
                }
            }

            System.Console.WriteLine("{1}% win for {0}", player1.PlayerName, (double)countWin / countGame * 100);
        }

        public class ScryingPoolAndCrossroads
            : Strategy
        {
            public static PlayerAction Player()
            {
                return new PlayerAction("ScryingPoolAndCrossroads",
                            purchaseOrder: PurchaseOrder(),
                            actionOrder: ActionOrder(),
                            chooseDefaultActionOnNone: false);
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                            CardAcceptance.For(Cards.Estate),
                            CardAcceptance.For(Cards.Remake));
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(Cards.WanderingMinstrel),
                    CardAcceptance.For(Cards.ScryingPool),
                    CardAcceptance.For(Cards.Festival),
                    CardAcceptance.For(Cards.CrossRoads));
            }          
        }
    }
}

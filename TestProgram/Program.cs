using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    class Program
    {       
        struct CardAcceptance
        {
            internal Card card;
            internal GameStatePredicate match;

            public CardAcceptance(Card card)
            {
                this.card = card;
                this.match = gameState => true;
            }

            public CardAcceptance(Card card, GameStatePredicate match)
            {
                this.card = card;
                this.match = match;
            }

            public static CardAcceptance For<T>()
                where T : Card, new()
            {
                return new CardAcceptance(new T());
            }

            public static CardAcceptance For<T>(GameStatePredicate match)
                where T : Card, new()
            {
                return new CardAcceptance(new T(), match);
            }
        }

        class CardPickByPriority
        {
            CardAcceptance[] cardAcceptances;

            public CardPickByPriority(params CardAcceptance[] cardAcceptances)
            {
                this.cardAcceptances = cardAcceptances;
            }       

            public Type GetMatchingCard(GameState gameState, CardPredicate cardPredicate)
            {
                foreach (CardAcceptance acceptance in this.cardAcceptances)
                {
                    if (cardPredicate(acceptance.card) &&
                        acceptance.match(gameState))
                    {
                        return acceptance.card.GetType();
                    }
                }

                return null;
            }
        }
       

        class PlayerAction
            : DefaultPlayerAction
        {            
            internal int playerIndex;
            CardPickByPriority purchaseOrder;
            CardPickByPriority actionOrder;
            CardPickByPriority trashOrder;
            CardPickByPriority treasurePlayOrder;

            public PlayerAction(int playerIndex,
                CardPickByPriority purchaseOrder,
                CardPickByPriority actionOrder,
                CardPickByPriority trashOrder,
                CardPickByPriority treasurePlayOrder)
            {
                this.playerIndex = playerIndex;                
                this.purchaseOrder = purchaseOrder;
                this.actionOrder = actionOrder;
                this.trashOrder = trashOrder;
                this.treasurePlayOrder = treasurePlayOrder;
            }

            public override Type GetCardFromSupplyToBuy(GameState gameState)
            {
                var currentPlayer = gameState.players.CurrentPlayer;  
                return this.purchaseOrder.GetMatchingCard(
                    gameState,
                    card => currentPlayer.AvailableCoins >= card.CurrentCoinCost(currentPlayer));
            }

            public override Type GetTreasureFromHandToPlay(GameState gameState)
            {
                var currentPlayer = gameState.players.CurrentPlayer;
                return this.treasurePlayOrder.GetMatchingCard(
                    gameState,
                    card => currentPlayer.Hand.HasCard(card.GetType()));                
            }

            public override Type GetActionFromHandToPlay(GameState gameState, bool isOptional)
            {
                var currentPlayer = gameState.players.CurrentPlayer;
                return this.actionOrder.GetMatchingCard(
                    gameState,
                    card => currentPlayer.Hand.HasCard(card.GetType()));
            }

            public override Type GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard)
            {
                var currentPlayer = gameState.players.CurrentPlayer;                
                return this.trashOrder.GetMatchingCard(
                    gameState,
                    card => currentPlayer.Hand.HasCard(card.GetType()) && acceptableCard(card));
            }

            public override string PlayerName
            {
                get
                {
                    return "Player" + playerIndex;
                }
            }
        }
      
        static void Main(string[] args)
        {
            int numberOfGames = 1000;

            int[] winnerCount = new int[2];

            for (int gameCount = 0; gameCount < numberOfGames; ++gameCount)
            {
                using (IGameLog gameLog = new DefaultLog(gameCount == 0 ? "..\\..\\Results\\GameLog.txt" : null))
                {
                    var player1 = new PlayerAction(1, DeathCartPurchaseOrder(), DeathCartActions(), DefaultTrashOrder(), DefaultTreasurePlayOrder());
                    var player2 = new PlayerAction(2, BetterPurchaseOrder(), EmptyPickOrder(), EmptyPickOrder(), DefaultTreasurePlayOrder());

                    var gameConfig = new GameConfig(new CardTypes.DeathCart());

                    GameState gameState = new GameState(
                        gameLog,
                        new PlayerAction[] { player1, player2 },
                        gameConfig);

                    gameState.PlayGameToEnd();

                    PlayerState[] winners = gameState.WinningPlayers;

                    if (winners.Length == 1)
                    {
                        int winningPlayerIndex = ((PlayerAction)winners[0].Actions).playerIndex - 1;
                        winnerCount[winningPlayerIndex]++;
                    }
                }        
            }

            for (int index = 0; index < winnerCount.Length; ++index)
            {
                System.Console.WriteLine("Player {0} won: {1} percent of the time.", index, winnerCount[index] / (double)numberOfGames * 100);
            }                
        }

        static CardPickByPriority DeathCartPurchaseOrder()
        {
            return new CardPickByPriority(                       
                        CardAcceptance.For<CardTypes.Province>(gameState => gameState.players.CurrentPlayer.AllOwnedCards.Where( card => card is CardTypes.Gold).Count() > 2),
                        CardAcceptance.For<CardTypes.Gold>(),
                        CardAcceptance.For<CardTypes.DeathCart>(gameState => gameState.players.CurrentPlayer.AllOwnedCards.Where(card => card is CardTypes.DeathCart).Count() < 1),
                        CardAcceptance.For<CardTypes.Silver>());
        }

        static CardPickByPriority BetterPurchaseOrder()
        {
            return new CardPickByPriority(
                       CardAcceptance.For<CardTypes.Province>(gameState => gameState.players.CurrentPlayer.AllOwnedCards.Where(card => card is CardTypes.Gold).Count() > 2),
                       CardAcceptance.For<CardTypes.Gold>(),
                       CardAcceptance.For<CardTypes.Silver>());            
        }

        static CardPickByPriority SimplePurchaseOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For<CardTypes.Province>(),
                CardAcceptance.For<CardTypes.Gold>(),
                CardAcceptance.For<CardTypes.Duchy>(),
                CardAcceptance.For<CardTypes.Silver>(),
                CardAcceptance.For<CardTypes.Estate>());                        
        }        

        static CardPickByPriority DeathCartActions()
        {
            return new CardPickByPriority( 
                CardAcceptance.For<CardTypes.DeathCart>());
        }

        static CardPickByPriority EmptyPickOrder()
        {
            return new CardPickByPriority();               
        }

        static CardPickByPriority DefaultTreasurePlayOrder()
        {
            return new CardPickByPriority(                
                CardAcceptance.For<CardTypes.Gold>(),                
                CardAcceptance.For<CardTypes.Silver>(),
                CardAcceptance.For<CardTypes.Copper>());
        }

        static CardPickByPriority DefaultTrashOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For<CardTypes.Estate>(),
                CardAcceptance.For<CardTypes.Ruin>());
        }
    }
}

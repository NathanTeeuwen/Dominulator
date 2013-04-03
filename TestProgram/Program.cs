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
        delegate bool GameStateToBool(GameState gameState);

        struct CardAcceptance
        {
            internal Card card;
            internal GameStateToBool match;

            public CardAcceptance(Card card)
            {
                this.card = card;
                this.match = gameState => true;
            }

            public CardAcceptance(Card card, GameStateToBool match)
            {
                this.card = card;
                this.match = match;
            }
        }
        
        class PurchaseByPrecedenceAndCost
        {
            CardAcceptance[] cardTypes;

            public PurchaseByPrecedenceAndCost(CardAcceptance[] cardTypes)
            {
                this.cardTypes = cardTypes;
            }

            public Type GetCardFromSupplyToBuy(GameState gameState, PlayerState playerState)
            {
                foreach (CardAcceptance acceptance in this.cardTypes)
                {
                    if (playerState.AvailableCoins >= acceptance.card.CurrentCoinCost(playerState) &&
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
            int playerIndex;
            PlayerState playerState;
            PurchaseByPrecedenceAndCost purchaseOrder;

            public PlayerAction(int playerIndex, PlayerState playerState, PurchaseByPrecedenceAndCost purchaseOrder)
            {
                this.playerIndex = playerIndex;
                this.playerState = playerState;
                this.purchaseOrder = purchaseOrder;
            }

            public override Type GetCardFromSupplyToBuy(GameState gameState)
            {
                return this.purchaseOrder.GetCardFromSupplyToBuy(gameState, this.playerState);
            }

            public override Type GetTreasureFromHandToPlay(GameState gameState)
            {
                if (this.playerState.Hand.HasCard<CardTypes.Gold>())
                    return typeof(CardTypes.Gold);
                if (this.playerState.Hand.HasCard<CardTypes.Silver>())
                    return typeof(CardTypes.Silver);
                if (this.playerState.Hand.HasCard<CardTypes.Copper>())
                    return typeof(CardTypes.Copper);

                return null;
            }

            public override string PlayerName
            {
                get
                {
                    return "Player" + playerIndex;
                }
            }
        }

        class PlayerActionFactory
            : IPlayerActionFactory
        {
            int playerCount = 0;
            PurchaseByPrecedenceAndCost purchaseOrderSimple;
            PurchaseByPrecedenceAndCost purchaseOrderBetter;

            public PlayerActionFactory()
            {
                this.purchaseOrderSimple = new PurchaseByPrecedenceAndCost(
                    new CardAcceptance[]
                    {
                        new CardAcceptance(new CardTypes.Province()),
                        new CardAcceptance(new CardTypes.Gold()),
                        new CardAcceptance(new CardTypes.Duchy()),
                        new CardAcceptance(new CardTypes.Silver()),
                        new CardAcceptance(new CardTypes.Estate())
                    });

                this.purchaseOrderBetter = new PurchaseByPrecedenceAndCost(
                    new CardAcceptance[]
                    {
                        new CardAcceptance(new CardTypes.Province(), 
                            gameState => (gameState.players.CurrentPlayer.AllOwnedCards.Where( card => card is CardTypes.Gold).Count() > 2) ),
                        new CardAcceptance(new CardTypes.Gold()),                       
                        new CardAcceptance(new CardTypes.Silver()),                        
                    });
            }

            public IPlayerAction NewPlayerAction(PlayerState playerState)
            {
                return new PlayerAction(++this.playerCount, playerState, 
                    this.playerCount == 1 ? this.purchaseOrderBetter : purchaseOrderSimple );
            }
        }

        static void Main(string[] args)
        {
            using (IGameLog gameLog = new DefaultLog())
            {
                GameState currentState = new GameState(
                    gameLog,
                    new PlayerActionFactory(), 
                    new Card[] { }, 
                    playerCount: 2, 
                    usePlatinumAndColony: false);
                currentState.PlayGameToEnd();
            }
        }
    }
}

using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{    
    public class TreasureMapDoctor
        : Strategy
    {            
        public static PlayerAction Player()
        {
            return new MyPlayerAction();
        }

        class MyPlayerAction
            : PlayerAction
        {
            public MyPlayerAction()
                : base("TreasureMapDoctor",
                    purchaseOrder: PurchaseOrder(),                        
                    actionOrder: ActionOrder(),
                    trashOrder: TrashAndDiscardOrder(),
                    discardOrder: TrashAndDiscardOrder())
            {
            }
               
            override public int GetCoinAmountToOverpayForCard(GameState gameState, Card card)
            {                    
                return gameState.Self.AvailableCoins;
            }

            public override Card NameACard(GameState gameState)
            {
                return GetCardTypeToTrash(gameState);
            }

            public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
            {
                if (gameState.CurrentContext.CurrentCard == Cards.Doctor)
                {
                    if (gameState.Self.CardsBeingRevealed.HasCard(Cards.Silver))
                        return PlayerActionChoice.Discard;
                    return PlayerActionChoice.Trash;
                }
                else
                    return base.ChooseBetween(gameState, acceptableChoice);
            }

            public override Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState)
            {
                return gameState.Self.CardsBeingRevealed.FirstOrDefault();
            }

            static Card GetCardTypeToTrash(GameState gameState)
            {
                PlayerState self = gameState.Self;

                if (self.CardsInDeck.Count <= 3 &&
                    CountInDeck(Cards.Estate, gameState) > 0)
                {
                    return Cards.Estate;
                }

                int countCopper = CountMightDraw(Cards.Copper, gameState, 3);
                int countEstate = CountMightDraw(Cards.Estate, gameState, 3);

                if (DefaultStrategies.ShouldBuyProvinces(gameState))
                    countEstate = 0;

                if (countCopper + countEstate == 0)
                    return Cards.Estate;

                return countCopper > countEstate ? (Card) Cards.Copper : Cards.Estate;
            }
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Doctor, gameState => CountAllOwned(Cards.Doctor, gameState) < 1 && gameState.Self.AvailableCoins >= 5),
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 5),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.TreasureMap, gameState => CountAllOwned(Cards.Gold, gameState) == 0),
                        //CardAcceptance.For(Cards.Doctor, gameState => CountAllOwned(Cards.Doctor, gameState) == 0),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        CardAcceptance.For(Cards.Silver));                           
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.TreasureMap, gameState => CountInHand(Cards.TreasureMap, gameState) == 2 || CountAllOwned(Cards.Gold, gameState) > 0),
                        CardAcceptance.For(Cards.Doctor));
        }

        private static CardPickByPriority TrashAndDiscardOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.Copper),
                        CardAcceptance.For(Cards.Silver),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy),
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.TreasureMap));
        }
    }
}

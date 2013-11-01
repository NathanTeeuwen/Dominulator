using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{    
    public class IronworksGreathallRemodelHuntingGrounds
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
                : base("IronworksGreathallRemodelHuntingGrounds",                            
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder())
            {
            }                
        }

        private static ICardPicker PurchaseOrder()
        {
            var highPriority = new CardPickByPriority(
                    CardAcceptance.For(Cards.GreatHall, gameState => CardBeingPlayedIs(Cards.IronWorks, gameState)),
                    CardAcceptance.For(Cards.Province),
                    CardAcceptance.For(Cards.HuntingGrounds, gameState => CountAllOwned(Cards.Gold, gameState) >= 1 ),
                    CardAcceptance.For(Cards.Gold),
                    CardAcceptance.For(Cards.Pillage, gameState => CountAllOwned(Cards.Gold, gameState) == 0),                     
                    CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4));

            var buildOrder = new CardPickByBuildOrder(
                CardAcceptance.For(Cards.IronWorks),
                CardAcceptance.For(Cards.Silver));

            var lowPriority = new CardPickByPriority(                       
                    CardAcceptance.For(Cards.IronWorks, gameState => CountAllOwned(Cards.IronWorks, gameState) < 2),
                    CardAcceptance.For(Cards.Remodel),
                    CardAcceptance.For(Cards.Silver));

            return new CardPickConcatenator(highPriority, buildOrder, lowPriority);                       
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.GreatHall),                           
                        CardAcceptance.For(Cards.IronWorks,
                            gameState => CountOfPile(Cards.GreatHall, gameState) > 0 ||
                            CountAllOwned(Cards.Remodel, gameState) * 6 < gameState.Self.AllOwnedCards.Count ||
                            gameState.Self.Hand.CountWhere(c => c.isAction && c != Cards.IronWorks) == 0),
                        CardAcceptance.For(Cards.Pillage),
                        CardAcceptance.For(Cards.Remodel),
                        CardAcceptance.For(Cards.HuntingGrounds));
        }

        private static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.HuntingGrounds),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Remodel),
                        CardAcceptance.For(Cards.IronWorks),                           
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.Copper));
        }            
    }
}

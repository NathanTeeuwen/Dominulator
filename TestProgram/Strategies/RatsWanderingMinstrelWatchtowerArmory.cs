using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program;

namespace Strategies
{    
    // for forum post:  http://forum.dominionstrategy.com/index.php?topic=9554.0
    public class RatsWanderingMinstrelWatchtowerArmory
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
                : base("RatsWanderingMinstrelWatchtowerArmory",                            
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(),
                        discardOrder: DiscardOrder(),
                        trashOrder: TrashOrder())
            {
            }                
        }

        private static ICardPicker PurchaseOrder()
        {
            var highPriority = new CardPickByPriority(
                    CardAcceptance.For(Cards.Province, gameState => gameState.Self.AvailableBuys >= 4 || CountAllOwned(Cards.Province, gameState) > 0),
                    CardAcceptance.For(Cards.Duchy, gameState => CountAllOwned(Cards.Province, gameState) >= 3),
                    CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 1));
                    //CardAcceptance.For(Cards.Jester, gameState => CountAllOwned(Cards.Jester, gameState) == 0));  // jester actually hurts in a non mirror match

            /*//  Intuitive guess as to the right build order, not correct it seeems.
            var buildOrder = new CardPickByBuildOrder(
                Cards.Armory,
                Cards.Silver,
                Cards.WanderingMinstrel,
                Cards.Watchtower,
                Cards.Rats,
                Cards.Watchtower);*/
                

            var buildOrder = new CardPickByBuildOrder(
                CardAcceptance.For(Cards.Armory),
                CardAcceptance.For(Cards.Watchtower),
                CardAcceptance.For(Cards.Rats),
                CardAcceptance.For(Cards.Watchtower));                                

            var lowPriority = new CardPickByPriority(
                    CardAcceptance.For(Cards.Bridge, ShouldBuyBridge),                       
                    CardAcceptance.For(Cards.WanderingMinstrel),                       
                    CardAcceptance.For(Cards.Watchtower, gameState => CountAllOwned(Cards.Watchtower, gameState) < 3));

            return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
        }

        private static bool ShouldBuyBridge(GameState gameState)
        {
            return CountAllOwned(Cards.WanderingMinstrel, gameState) > CountAllOwned(Cards.Bridge, gameState) + CountAllOwned(Cards.Watchtower, gameState) + 1;
        }

        private static bool ShouldPlayArmory(GameState gameState)
        {
            return CountAllOwned(Cards.Bridge, gameState) < 5;
        }

        private static bool CanPlayTerminalWhileChaining(GameState gameState)
        {
            return gameState.Self.AvailableActions >= 1;
        }

        private static bool CanPlay2TerminalsWhileChaining(GameState gameState)
        {
            return gameState.Self.AvailableActions >= 2;
        }

        private static bool WillRatsComboWork(GameState gameState)
        {
            return HasCardInHand(Cards.Watchtower, gameState) &&
                    HasCardInHand(Cards.Rats, gameState) &&
                    HasCardFromInHand(TrashOrderWithoutRats(), gameState);
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Armory, gameState => CanPlay2TerminalsWhileChaining(gameState) && ShouldPlayArmory(gameState)),
                        CardAcceptance.For(Cards.Jester, gameState => CanPlay2TerminalsWhileChaining(gameState)),
                        CardAcceptance.For(Cards.Bridge, gameState => CanPlay2TerminalsWhileChaining(gameState)),
                        CardAcceptance.For(Cards.Watchtower, gameState => CanPlayTerminalWhileChaining(gameState) && !WillRatsComboWork(gameState) && gameState.Self.Hand.Count <= 5),
                        CardAcceptance.For(Cards.Rats, gameState => WillRatsComboWork(gameState)),
                        CardAcceptance.For(Cards.WanderingMinstrel),
                        CardAcceptance.For(Cards.Necropolis),
                        CardAcceptance.For(Cards.Jester),
                        CardAcceptance.For(Cards.Armory, ShouldPlayArmory),
                        CardAcceptance.For(Cards.Bridge),                           
                        CardAcceptance.For(Cards.Watchtower));
        }

        // dicard order used to tune which actions to put back with wandering minstrell 
        private static CardPickByPriority DiscardOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For(Cards.Armory),
                CardAcceptance.For(Cards.Bridge),
                CardAcceptance.For(Cards.Necropolis),
                CardAcceptance.For(Cards.Watchtower),
                CardAcceptance.For(Cards.WanderingMinstrel),
                CardAcceptance.For(Cards.Rats));
        }

        private static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Rats, gameState => CountAllOwned(Cards.Rats, gameState) > 0),
                        CardAcceptance.For(Cards.Curse),
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.OvergrownEstate),                          
                        CardAcceptance.For(Cards.Hovel),
                        CardAcceptance.For(Cards.Necropolis),
                        CardAcceptance.For(Cards.Copper));
        }

        private static CardPickByPriority TrashOrderWithoutRats()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Curse),
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.OvergrownEstate),
                        CardAcceptance.For(Cards.Hovel),
                        CardAcceptance.For(Cards.Necropolis),                           
                        CardAcceptance.For(Cards.Copper)
                        );
        }
    }
}
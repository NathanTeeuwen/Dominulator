using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public static partial class Strategies
    {
        // for forum post:  http://forum.dominionstrategy.com/index.php?topic=9554.0
        public static class RatsWanderingMinstrelWatchtowerArmory
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("RatsWanderingMinstrelWatchtowerArmory",
                            playerNumber,
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
                     CardAcceptance.For(CardTypes.Province.card, gameState => gameState.Self.AvailableBuys >= 4 || CountAllOwned(CardTypes.Province.card, gameState) > 0),
                     CardAcceptance.For(CardTypes.Duchy.card, gameState => CountAllOwned(CardTypes.Province.card, gameState) >= 3),
                     CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 1));
                     //CardAcceptance.For(CardTypes.Jester.card, gameState => CountAllOwned(CardTypes.Jester.card, gameState) == 0));  // jester actually hurts in a non mirror match

                /*//  Intuitive guess as to the right build order, not correct it seeems.
                var buildOrder = new CardPickByBuildOrder(
                    CardTypes.Armory.card,
                    CardTypes.Silver.card,
                    CardTypes.WanderingMinstrell.card,
                    CardTypes.Watchtower.card,
                    CardTypes.Rats.card,
                    CardTypes.Watchtower.card);*/
                

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(CardTypes.Armory.card),
                    CardAcceptance.For(CardTypes.Watchtower.card),
                    CardAcceptance.For(CardTypes.Rats.card),
                    CardAcceptance.For(CardTypes.Watchtower.card));                                

                var lowPriority = new CardPickByPriority(
                       CardAcceptance.For(CardTypes.Bridge.card, ShouldBuyBridge),                       
                       CardAcceptance.For(CardTypes.WanderingMinstrell.card),                       
                       CardAcceptance.For(CardTypes.Watchtower.card, gameState => CountAllOwned(CardTypes.Watchtower.card, gameState) < 3));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static bool ShouldBuyBridge(GameState gameState)
            {
                return CountAllOwned(CardTypes.WanderingMinstrell.card, gameState) > CountAllOwned(CardTypes.Bridge.card, gameState) + CountAllOwned(CardTypes.Watchtower.card, gameState) + 1;
            }

            private static bool ShouldPlayArmory(GameState gameState)
            {
                return CountAllOwned(CardTypes.Bridge.card, gameState) < 5;
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
                return HasCardInHand(CardTypes.Watchtower.card, gameState) &&
                       HasCardInHand(CardTypes.Rats.card, gameState) &&
                       HasCardFromInHand(TrashOrderWithoutRats(), gameState);
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Armory.card, gameState => CanPlay2TerminalsWhileChaining(gameState) && ShouldPlayArmory(gameState)),
                           CardAcceptance.For(CardTypes.Jester.card, gameState => CanPlay2TerminalsWhileChaining(gameState)),
                           CardAcceptance.For(CardTypes.Bridge.card, gameState => CanPlay2TerminalsWhileChaining(gameState)),
                           CardAcceptance.For(CardTypes.Watchtower.card, gameState => CanPlayTerminalWhileChaining(gameState) && !WillRatsComboWork(gameState) && gameState.Self.Hand.Count <= 5),
                           CardAcceptance.For(CardTypes.Rats.card, gameState => WillRatsComboWork(gameState)),
                           CardAcceptance.For(CardTypes.WanderingMinstrell.card),
                           CardAcceptance.For(CardTypes.Necropolis.card),
                           CardAcceptance.For(CardTypes.Jester.card),
                           CardAcceptance.For(CardTypes.Armory.card, ShouldPlayArmory),
                           CardAcceptance.For(CardTypes.Bridge.card),                           
                           CardAcceptance.For(CardTypes.Watchtower.card));
            }

            // dicard order used to tune which actions to put back with wandering minstrell 
            private static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(CardTypes.Armory.card),
                    CardAcceptance.For(CardTypes.Bridge.card),
                    CardAcceptance.For(CardTypes.Necropolis.card),
                    CardAcceptance.For(CardTypes.Watchtower.card),
                    CardAcceptance.For(CardTypes.WanderingMinstrell.card),
                    CardAcceptance.For(CardTypes.Rats.card));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Rats.card, gameState => CountAllOwned(CardTypes.Rats.card, gameState) > 0),
                           CardAcceptance.For(CardTypes.Curse.card),
                           CardAcceptance.For(CardTypes.Estate.card),
                           CardAcceptance.For(CardTypes.OvergrownEstate.card),                          
                           CardAcceptance.For(CardTypes.Hovel.card),
                           CardAcceptance.For(CardTypes.Necropolis.card),
                           CardAcceptance.For(CardTypes.Copper.card));
            }

            private static CardPickByPriority TrashOrderWithoutRats()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Curse.card),
                           CardAcceptance.For(CardTypes.Estate.card),
                           CardAcceptance.For(CardTypes.OvergrownEstate.card),
                           CardAcceptance.For(CardTypes.Hovel.card),
                           CardAcceptance.For(CardTypes.Necropolis.card),                           
                           CardAcceptance.For(CardTypes.Copper.card)
                           );
            }
        }
    }
}
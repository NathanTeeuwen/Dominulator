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

                override public Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
                {
                    return Card.Type<CardTypes.Watchtower>();
                }                
            }

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                     CardAcceptance.For<CardTypes.Province>(gameState => gameState.players.CurrentPlayer.AvailableBuys >= 4 || CountAllOwned<CardTypes.Province>(gameState) > 0),
                     CardAcceptance.For<CardTypes.Duchy>(gameState => CountAllOwned<CardTypes.Province>(gameState) >= 3),
                     CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 1));
                     //CardAcceptance.For<CardTypes.Jester>(gameState => CountAllOwned<CardTypes.Jester>(gameState) == 0));  // jester actually hurts in a non mirror match

                /*//  Intuitive guess as to the right build order, not correct it seeems.
                var buildOrder = new CardPickByBuildOrder(
                    Card.Type<CardTypes.Armory>(),
                    Card.Type<CardTypes.Silver>(),
                    Card.Type<CardTypes.WanderingMinstrell>(),
                    Card.Type<CardTypes.Watchtower>(),
                    Card.Type<CardTypes.Rats>(),
                    Card.Type<CardTypes.Watchtower>());*/
                

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For<CardTypes.Armory>(),
                    CardAcceptance.For<CardTypes.Watchtower>(),
                    CardAcceptance.For<CardTypes.Rats>(),
                    CardAcceptance.For<CardTypes.Watchtower>());                                

                var lowPriority = new CardPickByPriority(
                       CardAcceptance.For<CardTypes.Bridge>(ShouldBuyBridge),                       
                       CardAcceptance.For<CardTypes.WanderingMinstrell>(),                       
                       CardAcceptance.For<CardTypes.Watchtower>(gameState => CountAllOwned<CardTypes.Watchtower>(gameState) < 3));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static bool ShouldBuyBridge(GameState gameState)
            {
                return CountAllOwned<CardTypes.WanderingMinstrell>(gameState) > CountAllOwned<CardTypes.Bridge>(gameState) + CountAllOwned<CardTypes.Watchtower>(gameState) + 1;
            }

            private static bool ShouldPlayArmory(GameState gameState)
            {
                return CountAllOwned<CardTypes.Bridge>(gameState) < 5;
            }

            private static bool CanPlayTerminalWhileChaining(GameState gameState)
            {
                return gameState.players.CurrentPlayer.AvailableActions >= 1;
            }

            private static bool CanPlay2TerminalsWhileChaining(GameState gameState)
            {
                return gameState.players.CurrentPlayer.AvailableActions >= 2;
            }

            private static bool WillRatsComboWork(GameState gameState)
            {
                return HasCardInHand<CardTypes.Watchtower>(gameState) &&
                       HasCardInHand<CardTypes.Rats>(gameState) &&
                       HasCardFromInHand(TrashOrderWithoutRats(), gameState);
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Armory>(gameState => CanPlay2TerminalsWhileChaining(gameState) && ShouldPlayArmory(gameState)),
                           CardAcceptance.For<CardTypes.Jester>(gameState => CanPlay2TerminalsWhileChaining(gameState)),
                           CardAcceptance.For<CardTypes.Bridge>(gameState => CanPlay2TerminalsWhileChaining(gameState)),
                           CardAcceptance.For<CardTypes.Watchtower>(gameState => CanPlayTerminalWhileChaining(gameState) && !WillRatsComboWork(gameState) && gameState.players.CurrentPlayer.Hand.Count() <= 5),
                           CardAcceptance.For<CardTypes.Rats>(gameState => WillRatsComboWork(gameState)),
                           CardAcceptance.For<CardTypes.WanderingMinstrell>(),
                           CardAcceptance.For<CardTypes.Necropolis>(),
                           CardAcceptance.For<CardTypes.Jester>(),
                           CardAcceptance.For<CardTypes.Armory>(ShouldPlayArmory),
                           CardAcceptance.For<CardTypes.Bridge>(),                           
                           CardAcceptance.For<CardTypes.Watchtower>());
            }

            // dicard order used to tune which actions to put back with wandering minstrell 
            private static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Armory>(),
                    CardAcceptance.For<CardTypes.Bridge>(),
                    CardAcceptance.For<CardTypes.Necropolis>(),
                    CardAcceptance.For<CardTypes.Watchtower>(),
                    CardAcceptance.For<CardTypes.WanderingMinstrell>(),
                    CardAcceptance.For<CardTypes.Rats>());
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Rats>(gameState => CountAllOwned<CardTypes.Rats>(gameState) > 0),
                           CardAcceptance.For<CardTypes.Curse>(),
                           CardAcceptance.For<CardTypes.Estate>(),
                           CardAcceptance.For<CardTypes.OvergrownEstate>(),                          
                           CardAcceptance.For<CardTypes.Hovel>(),
                           CardAcceptance.For<CardTypes.Necropolis>(),
                           CardAcceptance.For<CardTypes.Copper>());
            }

            private static CardPickByPriority TrashOrderWithoutRats()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Curse>(),
                           CardAcceptance.For<CardTypes.Estate>(),
                           CardAcceptance.For<CardTypes.OvergrownEstate>(),
                           CardAcceptance.For<CardTypes.Hovel>(),
                           CardAcceptance.For<CardTypes.Necropolis>(),                           
                           CardAcceptance.For<CardTypes.Copper>()
                           );
            }
        }
    }
}
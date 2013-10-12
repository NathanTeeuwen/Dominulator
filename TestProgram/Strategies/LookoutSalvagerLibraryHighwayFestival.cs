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
        public static class LookoutSalvagerLibraryHighwayFestival
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
                    : base("LookoutSalvagerLibraryHighwayFestival",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder())
                {
                }                
            }

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                     CardAcceptance.For(Cards.Province, ShouldBuyProvinces),
                     CardAcceptance.For(Cards.Duchy, gameState => CountAllOwned(Cards.Province, gameState) >= 3),
                     CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 1),
                     CardAcceptance.For(Cards.Salvager, gameState => CountAllOwned(Cards.Copper, gameState) >= 6 && CountAllOwned(Cards.Salvager, gameState) == 0),
                     CardAcceptance.For(Cards.Lookout, gameState => CountAllOwned(Cards.Copper, gameState) >= 6 && CountAllOwned(Cards.Lookout, gameState) == 0),
                     CardAcceptance.For(Cards.Silver, gameState => CountAllOwned(Cards.Silver, gameState) + CountAllOwned(Cards.Festival, gameState) < 2)
                     );
                
                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(Cards.Festival),
                    CardAcceptance.For(Cards.Library),
                    CardAcceptance.For(Cards.Festival),
                    CardAcceptance.For(Cards.Highway),
                    CardAcceptance.For(Cards.Highway),
                    CardAcceptance.For(Cards.Festival),
                    CardAcceptance.For(Cards.Festival),
                    CardAcceptance.For(Cards.Library),
                    CardAcceptance.For(Cards.Festival)
                    );

                var lowPriority = new CardPickByPriority(
                    CardAcceptance.For(Cards.Highway));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static bool ShouldBuyProvinces(GameState gameState)
            {
                return CostOfCard(Cards.Province, gameState) <= 4 || CountAllOwned(Cards.Province, gameState) > 0;
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Lookout, Default.ShouldPlayLookout(ShouldBuyProvinces)),
                           CardAcceptance.For(Cards.Highway),
                           CardAcceptance.For(Cards.Festival),
                           CardAcceptance.For(Cards.Salvager, Default.ShouldPlaySalvager(TrashOrder())),
                           CardAcceptance.For(Cards.Necropolis),
                           CardAcceptance.For(Cards.Library)
                           );
            }            

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(                           
                           CardAcceptance.For(Cards.Curse),
                           CardAcceptance.For(Cards.Lookout, gameState => CountAllOwned(Cards.Copper, gameState) <= 4),                                               
                           CardAcceptance.For(Cards.Estate),
                           CardAcceptance.For(Cards.OvergrownEstate),
                           CardAcceptance.For(Cards.Hovel),
                           CardAcceptance.For(Cards.Necropolis),
                           CardAcceptance.For(Cards.Silver, gameState => gameState.Self.ExpectedCoinValueAtEndOfTurn == 4 && CardBeingPlayedIs(Cards.Salvager, gameState)),
                           CardAcceptance.For(Cards.Copper),
                           CardAcceptance.For(Cards.Silver));
            }            
        }
    }
}
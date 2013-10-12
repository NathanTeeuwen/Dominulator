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
                     CardAcceptance.For(CardTypes.Province.card, ShouldBuyProvinces),
                     CardAcceptance.For(CardTypes.Duchy.card, gameState => CountAllOwned(CardTypes.Province.card, gameState) >= 3),
                     CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 1),
                     CardAcceptance.For(CardTypes.Salvager.card, gameState => CountAllOwned(CardTypes.Copper.card, gameState) >= 6 && CountAllOwned(CardTypes.Salvager.card, gameState) == 0),
                     CardAcceptance.For(CardTypes.Lookout.card, gameState => CountAllOwned(CardTypes.Copper.card, gameState) >= 6 && CountAllOwned(CardTypes.Lookout.card, gameState) == 0),
                     CardAcceptance.For(CardTypes.Silver.card, gameState => CountAllOwned(CardTypes.Silver.card, gameState) + CountAllOwned(CardTypes.Festival.card, gameState) < 2)
                     );
                
                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(CardTypes.Festival.card),
                    CardAcceptance.For(CardTypes.Library.card),
                    CardAcceptance.For(CardTypes.Festival.card),
                    CardAcceptance.For(CardTypes.Highway.card),
                    CardAcceptance.For(CardTypes.Highway.card),
                    CardAcceptance.For(CardTypes.Festival.card),
                    CardAcceptance.For(CardTypes.Festival.card),
                    CardAcceptance.For(CardTypes.Library.card),
                    CardAcceptance.For(CardTypes.Festival.card)
                    );

                var lowPriority = new CardPickByPriority(
                    CardAcceptance.For(CardTypes.Highway.card));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static bool ShouldBuyProvinces(GameState gameState)
            {
                return CostOfCard(CardTypes.Province.card, gameState) <= 4 || CountAllOwned(CardTypes.Province.card, gameState) > 0;
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Lookout.card, Default.ShouldPlayLookout(ShouldBuyProvinces)),
                           CardAcceptance.For(CardTypes.Highway.card),
                           CardAcceptance.For(CardTypes.Festival.card),
                           CardAcceptance.For(CardTypes.Salvager.card, Default.ShouldPlaySalvager(TrashOrder())),
                           CardAcceptance.For(CardTypes.Necropolis.card),
                           CardAcceptance.For(CardTypes.Library.card)
                           );
            }            

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(                           
                           CardAcceptance.For(CardTypes.Curse.card),
                           CardAcceptance.For(CardTypes.Lookout.card, gameState => CountAllOwned(CardTypes.Copper.card, gameState) <= 4),                                               
                           CardAcceptance.For(CardTypes.Estate.card),
                           CardAcceptance.For(CardTypes.OvergrownEstate.card),
                           CardAcceptance.For(CardTypes.Hovel.card),
                           CardAcceptance.For(CardTypes.Necropolis.card),
                           CardAcceptance.For(CardTypes.Silver.card, gameState => gameState.Self.ExpectedCoinValueAtEndOfTurn == 4 && CardBeingPlayedIs(CardTypes.Salvager.card, gameState)),
                           CardAcceptance.For(CardTypes.Copper.card),
                           CardAcceptance.For(CardTypes.Silver.card));
            }            
        }
    }
}
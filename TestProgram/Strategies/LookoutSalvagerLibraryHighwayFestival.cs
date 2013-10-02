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
                     CardAcceptance.For<CardTypes.Province>(ShouldBuyProvinces),
                     CardAcceptance.For<CardTypes.Duchy>(gameState => CountAllOwned<CardTypes.Province>(gameState) >= 3),
                     CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 1),
                     CardAcceptance.For<CardTypes.Salvager>(gameState => CountAllOwned<CardTypes.Copper>(gameState) >= 6 && CountAllOwned<CardTypes.Salvager>(gameState) == 0),
                     CardAcceptance.For<CardTypes.Lookout>(gameState => CountAllOwned<CardTypes.Copper>(gameState) >= 6 && CountAllOwned<CardTypes.Lookout>(gameState) == 0),
                     CardAcceptance.For<CardTypes.Silver>(gameState => CountAllOwned<CardTypes.Silver>(gameState) + CountAllOwned<CardTypes.Festival>(gameState) < 2)
                     );
                
                var buildOrder = new CardPickByBuildOrder(                    
                    new CardTypes.Festival(),
                    new CardTypes.Library(),
                    new CardTypes.Festival(),
                    new CardTypes.Highway(),
                    new CardTypes.Highway(),
                    new CardTypes.Festival(),                    
                    new CardTypes.Festival(),
                    new CardTypes.Library(),
                    new CardTypes.Festival()
                    );

                var lowPriority = new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Highway>());

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static bool ShouldBuyProvinces(GameState gameState)
            {
                return CostOfCard<CardTypes.Province>(gameState) <= 4 || CountAllOwned<CardTypes.Province>(gameState) > 0;
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Lookout>(Default.ShouldPlayLookout(ShouldBuyProvinces)),
                           CardAcceptance.For<CardTypes.Highway>(),
                           CardAcceptance.For<CardTypes.Festival>(),
                           CardAcceptance.For<CardTypes.Salvager>(Default.ShouldPlaySalvager(TrashOrder())),
                           CardAcceptance.For<CardTypes.Library>()
                           );
            }            

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(                           
                           CardAcceptance.For<CardTypes.Curse>(),
                           CardAcceptance.For<CardTypes.Lookout>(gameState => CountAllOwned<CardTypes.Copper>(gameState) <= 4),                                               
                           CardAcceptance.For<CardTypes.Estate>(),
                           CardAcceptance.For<CardTypes.Silver>(gameState => gameState.players.CurrentPlayer.ExpectedCoinValueAtEndOfTurn == 4 && CardBeingPlayedIs<CardTypes.Salvager>(gameState)),
                           CardAcceptance.For<CardTypes.Copper>(),
                           CardAcceptance.For<CardTypes.Silver>());
            }            
        }
    }
}
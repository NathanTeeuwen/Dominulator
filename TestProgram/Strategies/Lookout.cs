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
        public static class Lookout
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            treasurePlayOrder: Default.TreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashAndDiscardOrder(),
                            discardOrder: TrashAndDiscardOrder());
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(ShouldBuyProvinces),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 2),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 2),
                           CardAcceptance.For<CardTypes.Lookout>(gameState => CountAllOwned<CardTypes.Lookout>(gameState) < 1),                           
                           CardAcceptance.For<CardTypes.Silver>());
            }            

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Lookout>(ShouldPlayLookout));
            }

            private static bool ShouldBuyProvinces(GameState gameState)
            {
                return CountAllOwned<CardTypes.Gold>(gameState) > 2;
            }

            private static bool ShouldPlayLookout(GameState gameState)
            {
                int cardCountToTrash = CountInDeck<CardTypes.Copper>(gameState);

                if (!ShouldBuyProvinces(gameState))
                {
                    cardCountToTrash += CountInDeck<CardTypes.Estate>(gameState);                    
                }

                cardCountToTrash += CountInDeck<CardTypes.Lookout>(gameState);                    

                int totalCardsOwned = gameState.players.CurrentPlayer.CardsInDeck.Count();

                return ((double)cardCountToTrash) / totalCardsOwned > 0.4;
            }

            private static CardPickByPriority TrashAndDiscardOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Estate>(),
                           CardAcceptance.For<CardTypes.Copper>(),
                           CardAcceptance.For<CardTypes.Lookout>(),
                           CardAcceptance.For<CardTypes.Silver>(),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Province>(),
                           CardAcceptance.For<CardTypes.Duchy>(),
                           CardAcceptance.For<CardTypes.Estate>());
            }        
        }
    }
}

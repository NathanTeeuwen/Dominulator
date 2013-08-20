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
        public static class ButcherPlazaWatchtower
        {              
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("ButcherPlazaWatchtower",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder())
                {
                }
                
                public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {
                    return PlayerActionChoice.TopDeck;
                }                
            }           

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                         CardAcceptance.For<CardTypes.Province>(gameState => gameState.players.CurrentPlayer.AllOwnedCards.Where(card => card is CardTypes.Gold).Count() > 2),
                          CardAcceptance.For<CardTypes.Duchy>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 4),
                          CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 2),
                          CardAcceptance.For<CardTypes.Butcher>(gameState => CountAllOwned<CardTypes.Butcher>(gameState) < 2),
                          CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 3));

                var buildOrder = new CardPickByBuildOrder(                    
                    new CardTypes.Silver(),
                    new CardTypes.Watchtower());

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Plaza>(),
                           CardAcceptance.For<CardTypes.Watchtower>());

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);                
            }            

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Plaza>(),
                           CardAcceptance.For<CardTypes.Butcher>(),
                           CardAcceptance.For<CardTypes.Watchtower>());
            }                        

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Estate>(),
                           CardAcceptance.For<CardTypes.Copper>());
            }        
        }
    }
}

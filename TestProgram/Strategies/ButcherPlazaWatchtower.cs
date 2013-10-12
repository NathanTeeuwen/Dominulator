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
                            trashOrder: TrashOrder(),
                            discardOrder: DiscardOrder(),
                            chooseDefaultActionOnNone:false)
                {
                }                
            }           

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                         CardAcceptance.For(CardTypes.Province.card, CardAcceptance.AlwaysMatch, CardAcceptance.OverPayMaxAmount),
                          CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 2),                          
                          CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 2),
                          CardAcceptance.For(CardTypes.Butcher.card, gameState => CountAllOwned(CardTypes.Butcher.card, gameState) < 2),                          
                          CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 3));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Watchtower.card),
                    CardAcceptance.For(CardTypes.Plaza.card),
                    CardAcceptance.For(CardTypes.Plaza.card),
                    CardAcceptance.For(CardTypes.Watchtower.card)                    
                    );

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Plaza.card),
                           CardAcceptance.For(CardTypes.Watchtower.card));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);                
            }            

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Necropolis.card),
                           CardAcceptance.For(CardTypes.Plaza.card),
                           CardAcceptance.For(CardTypes.Watchtower.card, gameState => gameState.Self.AvailableActions == 1),
                           //CardAcceptance.For(CardTypes.Watchtower.card, gameState => CountInHand(CardTypes.Estate.card, gameState) + CountInHand(CardTypes.Copper.card, gameState) == 0 && gameState.players.CurrentPlayer.AvailableActions > 1),
                           CardAcceptance.For(CardTypes.Butcher.card),
                           CardAcceptance.For(CardTypes.Watchtower.card));
            }                        

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Estate.card),
                           CardAcceptance.For(CardTypes.OvergrownEstate.card),
                           CardAcceptance.For(CardTypes.Hovel.card),
                           CardAcceptance.For(CardTypes.Copper.card),
                           CardAcceptance.For(CardTypes.Silver.card),
                           CardAcceptance.For(CardTypes.Butcher.card),
                           CardAcceptance.For(CardTypes.Watchtower.card),
                           CardAcceptance.For(CardTypes.Duchy.card),
                           CardAcceptance.For(CardTypes.Province.card));
            }

            private static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(                           
                           CardAcceptance.For(CardTypes.Copper.card),
                           CardAcceptance.For(CardTypes.Silver.card));
            }        
        }
    }
}

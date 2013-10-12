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
        public static class GovernorJunkdealer
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
                    : base("GovernorJunkdealer",
                        playerNumber,
                        purchaseOrder: PurchaseOrder(),                        
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder())
                {
                }

                public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {
                    return PlayerActionChoice.GainCard;
                }
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 2),
                           CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) < 4),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),
                           CardAcceptance.For(Cards.Conspirator, gameState => CountAllOwned(Cards.Conspirator, gameState) < 1),
                           CardAcceptance.For(Cards.JunkDealer, gameState => CountAllOwned(Cards.JunkDealer, gameState) < 1),
                           CardAcceptance.For(Cards.Governor, gameState => CountAllOwned(Cards.Governor, gameState) < 2),
                           CardAcceptance.For(Cards.Gold),
                           CardAcceptance.For(Cards.Governor),
                           CardAcceptance.For(Cards.Silver)
                           );
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Governor),
                           CardAcceptance.For(Cards.JunkDealer, gameState => HasCardFromInHand(TrashOrder(), gameState)),
                           CardAcceptance.For(Cards.Necropolis),
                           CardAcceptance.For(Cards.Conspirator)
                           );
            }
          
            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(                  
                           CardAcceptance.For(Cards.OvergrownEstate),
                           CardAcceptance.For(Cards.Necropolis),
                           CardAcceptance.For(Cards.Hovel),
                           CardAcceptance.For(Cards.Estate),                           
                           CardAcceptance.For(Cards.Copper));
            }
        }
    }
}

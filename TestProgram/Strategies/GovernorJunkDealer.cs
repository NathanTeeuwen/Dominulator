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
                           CardAcceptance.For<CardTypes.Province>(gameState => CountAllOwned<CardTypes.Gold>(gameState) > 2),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => gameState.GetPile<CardTypes.Province>().Count() < 4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() < 2),
                           CardAcceptance.For<CardTypes.Conspirator>(gameState => CountAllOwned<CardTypes.Conspirator>(gameState) < 1),
                           CardAcceptance.For<CardTypes.JunkDealer>(gameState => CountAllOwned<CardTypes.JunkDealer>(gameState) < 1),
                           CardAcceptance.For<CardTypes.Governor>(gameState => CountAllOwned<CardTypes.Governor>(gameState) < 2),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Governor>(),
                           CardAcceptance.For<CardTypes.Silver>()
                           );
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Governor>(),
                           CardAcceptance.For<CardTypes.JunkDealer>(gameState => HasCardFromInHand(TrashOrder(), gameState)),
                           CardAcceptance.For<CardTypes.Necropolis>(),
                           CardAcceptance.For<CardTypes.Conspirator>()
                           );
            }
          
            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(                  
                           CardAcceptance.For<CardTypes.OvergrownEstate>(),
                           CardAcceptance.For<CardTypes.Necropolis>(),
                           CardAcceptance.For<CardTypes.Hovel>(),
                           CardAcceptance.For<CardTypes.Estate>(),                           
                           CardAcceptance.For<CardTypes.Copper>());
            }
        }
    }
}

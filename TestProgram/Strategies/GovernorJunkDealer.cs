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
                           CardAcceptance.For(CardTypes.Province.card, gameState => CountAllOwned(CardTypes.Gold.card, gameState) > 2),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 4),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 2),
                           CardAcceptance.For(CardTypes.Conspirator.card, gameState => CountAllOwned(CardTypes.Conspirator.card, gameState) < 1),
                           CardAcceptance.For(CardTypes.JunkDealer.card, gameState => CountAllOwned(CardTypes.JunkDealer.card, gameState) < 1),
                           CardAcceptance.For(CardTypes.Governor.card, gameState => CountAllOwned(CardTypes.Governor.card, gameState) < 2),
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(CardTypes.Governor.card),
                           CardAcceptance.For(CardTypes.Silver.card)
                           );
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Governor.card),
                           CardAcceptance.For(CardTypes.JunkDealer.card, gameState => HasCardFromInHand(TrashOrder(), gameState)),
                           CardAcceptance.For(CardTypes.Necropolis.card),
                           CardAcceptance.For(CardTypes.Conspirator.card)
                           );
            }
          
            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(                  
                           CardAcceptance.For(CardTypes.OvergrownEstate.card),
                           CardAcceptance.For(CardTypes.Necropolis.card),
                           CardAcceptance.For(CardTypes.Hovel.card),
                           CardAcceptance.For(CardTypes.Estate.card),                           
                           CardAcceptance.For(CardTypes.Copper.card));
            }
        }
    }
}

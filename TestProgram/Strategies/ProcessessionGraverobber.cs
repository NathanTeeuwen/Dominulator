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
        public static class ProcessionGraverobber
        {
            // big money smithy player
            private static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "ProcessionGraverobber",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder());
            }

            private static ICardPicker PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Province),
                           CardAcceptance.For(Cards.Graverobber),
                           CardAcceptance.For(Cards.Militia, 1),
                           CardAcceptance.For(Cards.Feast),
                           CardAcceptance.For(Cards.Procession),
                           CardAcceptance.For(Cards.Silver, 1),
                           CardAcceptance.For(Cards.Village));
            }

            private static ICardPicker ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Procession),
                           CardAcceptance.For(Cards.Graverobber, gameState => CardBeingPlayedIs(Cards.Procession, gameState)),
                           CardAcceptance.For(Cards.Feast, gameState => CardBeingPlayedIs(Cards.Procession, gameState)),
                           CardAcceptance.For(Cards.Village),
                           CardAcceptance.For(Cards.Graverobber),
                           CardAcceptance.For(Cards.Militia, 1),
                           CardAcceptance.For(Cards.Feast),                           
                           CardAcceptance.For(Cards.Silver, 1));
            }

            private static ICardPicker TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Graverobber),
                           CardAcceptance.For(Cards.Feast),
                           CardAcceptance.For(Cards.Village),
                           CardAcceptance.For(Cards.Militia));
            }
        }
    }
}
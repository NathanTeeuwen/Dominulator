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
            private static PlayerAction Player()
            {
                return new MyPlayerAction();
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction()
                    : base("ProcessionGraverobber",                            
                            purchaseOrder: PurchaseOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder())
                {
                }

                public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {
                    if (CardBeingPlayedIs(Cards.Graverobber, gameState))
                    {                                               
                        // always prefer trashing cards that might get convertd to victory points
                        if (gameState.Self.Hand.AnyWhere(card => CardTypes.Graverobber.CardValidToTrash(card) && 
                                                                   card.CurrentCoinCost(gameState.Self) >= 5 &&
                                                                   HasCardIn(card, this.trashOrder, gameState)))
                            return PlayerActionChoice.Trash;

                        // otherwise prefer to gain a card from trash if you are trying to gain it
                        if (gameState.trash.HasCard(c => CardTypes.Graverobber.CardValidToGainFromTrash(c, gameState.Self) &&
                                                         HasCardIn(c, this.purchaseOrder, gameState)))
                        {
                            return PlayerActionChoice.GainCard;
                        }

                        // otherwise, you really want to do nothing


                        // otherwise gain whatever u can from the trash
                        return PlayerActionChoice.GainCard;
                    }

                    return base.ChooseBetween(gameState, acceptableChoice);
                }
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
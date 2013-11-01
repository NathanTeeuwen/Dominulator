using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program;

namespace Strategies
{    
    public class ProcessionGraverobber
        : Strategy
    {            
        public static PlayerAction Player()
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
                        trashOrder: TrashOrder(),
                        chooseDefaultActionOnNone:false)
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
                else if (CardBeingPlayedIs(Cards.Squire, gameState))
                {
                    if (gameState.Self.AvailableActions == 0)
                        return PlayerActionChoice.PlusAction;
                    else
                        return PlayerActionChoice.PlusBuy;
                }

                return base.ChooseBetween(gameState, acceptableChoice);
            }
        }           

        private static ICardPicker PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 6),
                        CardAcceptance.For(Cards.Militia, 1),
                        CardAcceptance.For(Cards.Procession, 1),
                        CardAcceptance.For(Cards.Graverobber),
                        CardAcceptance.For(Cards.Feast, 1, gameState => CountAllOwned(Cards.Procession, gameState) > 0),
                        CardAcceptance.For(Cards.Procession),
                        CardAcceptance.For(Cards.Village)); 
                        //CardAcceptance.For(Cards.Squire));
        }

        private static ICardPicker ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Procession),
                        CardAcceptance.For(Cards.Squire, gameState => CardBeingPlayedIs(Cards.Procession, gameState)),
                        CardAcceptance.For(Cards.Village, gameState => CardBeingPlayedIs(Cards.Procession, gameState) && gameState.Self.AvailableActions == 0),
                        CardAcceptance.For(Cards.Feast, gameState => CardBeingPlayedIs(Cards.Procession, gameState)),
                        CardAcceptance.For(Cards.Graverobber, gameState => CardBeingPlayedIs(Cards.Procession, gameState) && BenefitFromGraverobber(gameState)),
                        CardAcceptance.For(Cards.Village),
                        CardAcceptance.For(Cards.Squire),
                        CardAcceptance.For(Cards.Graverobber, BenefitFromGraverobber),
                        CardAcceptance.For(Cards.Militia),
                        CardAcceptance.For(Cards.Feast));
        }

        private static bool BenefitFromGraverobber(GameState gameState)
        {
            return gameState.trash.HasCard(c => CardTypes.Graverobber.CardValidToGainFromTrash(c, gameState.Self)) || 
                    gameState.Self.Hand.CountWhere( c => CardTypes.Graverobber.CardValidToTrash(c)) > 1;
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
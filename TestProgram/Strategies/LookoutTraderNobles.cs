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
        public static class LookoutTraderNobles
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
                    : base("LookoutTraderNobles",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(), 
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder())
                {
                }                

                override public PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {
                    if (CountInHand(CardTypes.Nobles.card, gameState) > 0)
                        return PlayerActionChoice.PlusAction;

                    if (ShouldBuyProvinces(gameState))
                        return PlayerActionChoice.PlusCard;

                    return gameState.Self.Hand.HasCard(card => card.isAction) ? PlayerActionChoice.PlusAction : PlayerActionChoice.PlusCard;
                }
            }

            private static ICardPicker PurchaseOrder()
            {
                return new CardPickByPriority(
                       CardAcceptance.For(CardTypes.Province.card, ShouldBuyProvinces),
                       CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 4),
                       CardAcceptance.For(CardTypes.Estate.card, ShouldBuyEstates),
                       CardAcceptance.For(CardTypes.Nobles.card),
                       CardAcceptance.For(CardTypes.Trader.card, gameState => CountAllOwned(CardTypes.Trader.card, gameState) < 1),
                       CardAcceptance.For(CardTypes.Lookout.card, gameState => CountAllOwned(CardTypes.Lookout.card, gameState) < 2 && !ShouldBuyProvinces(gameState)),                       
                       CardAcceptance.For(CardTypes.Silver.card));
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Lookout.card, gameState => CountInHand(CardTypes.Lookout.card, gameState) > 1 ),
                           CardAcceptance.For(CardTypes.Trader.card, gameState => CountAllOwned(CardTypes.Lookout.card, gameState) > 1 && CountInHand(CardTypes.Lookout.card, gameState) > 0 && ShouldTrashLookout(gameState)),
                           CardAcceptance.For(CardTypes.Lookout.card, Default.ShouldPlayLookout(Default.ShouldBuyProvinces)),
                           CardAcceptance.For(CardTypes.Nobles.card),
                           CardAcceptance.For(CardTypes.Trader.card, gameState => HasCardFromInHand(TrashOrder(), gameState)));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Curse.card),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountAllOwned(CardTypes.Silver.card, gameState) >= 4 && !ShouldBuyEstates(gameState)),
                           CardAcceptance.For(CardTypes.Copper.card, gameState => CardBeingPlayedIs(CardTypes.Lookout.card, gameState)),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => !ShouldBuyEstates(gameState)),
                           CardAcceptance.For(CardTypes.Lookout.card, gameState => CardBeingPlayedIs(CardTypes.Trader.card, gameState) && ShouldTrashLookout(gameState)),
                           CardAcceptance.For(CardTypes.Copper.card),
                           CardAcceptance.For(CardTypes.Lookout.card));
            }

            private static bool ShouldBuyEstates(GameState gameState)
            {
                return CountOfPile(CardTypes.Province.card, gameState) <= 2;
            }

            private static bool ShouldTrashLookout(GameState gameState)
            {
                return CountAllOwned(CardTypes.Copper.card, gameState) <= 4;
            }

            private static bool ShouldBuyProvinces(GameState gameState)
            {
                return CountAllOwned(CardTypes.Copper.card, gameState) <= 4;
            }
        }        
    }
}

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
                    if (CountInHand(Cards.Nobles, gameState) > 0)
                        return PlayerActionChoice.PlusAction;

                    if (ShouldBuyProvinces(gameState))
                        return PlayerActionChoice.PlusCard;

                    return gameState.Self.Hand.HasCard(card => card.isAction) ? PlayerActionChoice.PlusAction : PlayerActionChoice.PlusCard;
                }
            }

            private static ICardPicker PurchaseOrder()
            {
                return new CardPickByPriority(
                       CardAcceptance.For(Cards.Province, ShouldBuyProvinces),
                       CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                       CardAcceptance.For(Cards.Estate, ShouldBuyEstates),
                       CardAcceptance.For(Cards.Nobles),
                       CardAcceptance.For(Cards.Trader, gameState => CountAllOwned(Cards.Trader, gameState) < 1),
                       CardAcceptance.For(Cards.Lookout, gameState => CountAllOwned(Cards.Lookout, gameState) < 2 && !ShouldBuyProvinces(gameState)),                       
                       CardAcceptance.For(Cards.Silver));
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Lookout, gameState => CountInHand(Cards.Lookout, gameState) > 1 ),
                           CardAcceptance.For(Cards.Trader, gameState => CountAllOwned(Cards.Lookout, gameState) > 1 && CountInHand(Cards.Lookout, gameState) > 0 && ShouldTrashLookout(gameState)),
                           CardAcceptance.For(Cards.Lookout, Default.ShouldPlayLookout(Default.ShouldBuyProvinces)),
                           CardAcceptance.For(Cards.Nobles),
                           CardAcceptance.For(Cards.Trader, gameState => HasCardFromInHand(TrashOrder(), gameState)));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Curse),
                           CardAcceptance.For(Cards.Estate, gameState => CountAllOwned(Cards.Silver, gameState) >= 4 && !ShouldBuyEstates(gameState)),
                           CardAcceptance.For(Cards.Copper, gameState => CardBeingPlayedIs(Cards.Lookout, gameState)),
                           CardAcceptance.For(Cards.Estate, gameState => !ShouldBuyEstates(gameState)),
                           CardAcceptance.For(Cards.Lookout, gameState => CardBeingPlayedIs(Cards.Trader, gameState) && ShouldTrashLookout(gameState)),
                           CardAcceptance.For(Cards.Copper),
                           CardAcceptance.For(Cards.Lookout));
            }

            private static bool ShouldBuyEstates(GameState gameState)
            {
                return CountOfPile(Cards.Province, gameState) <= 2;
            }

            private static bool ShouldTrashLookout(GameState gameState)
            {
                return CountAllOwned(Cards.Copper, gameState) <= 4;
            }

            private static bool ShouldBuyProvinces(GameState gameState)
            {
                return CountAllOwned(Cards.Copper, gameState) <= 4;
            }
        }        
    }
}

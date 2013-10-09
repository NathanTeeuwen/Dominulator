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
                    if (CountInHand<CardTypes.Nobles>(gameState) > 0)
                        return PlayerActionChoice.PlusAction;

                    if (ShouldBuyProvinces(gameState))
                        return PlayerActionChoice.PlusCard;

                    return gameState.players.CurrentPlayer.Hand.HasCard(card => card.isAction) ? PlayerActionChoice.PlusAction : PlayerActionChoice.PlusCard;
                }
            }

            private static ICardPicker PurchaseOrder()
            {
                return new CardPickByPriority(
                       CardAcceptance.For<CardTypes.Province>(ShouldBuyProvinces),
                       CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 4),
                       CardAcceptance.For<CardTypes.Estate>(ShouldBuyEstates),
                       CardAcceptance.For<CardTypes.Nobles>(),
                       CardAcceptance.For<CardTypes.Trader>(gameState => CountAllOwned<CardTypes.Trader>(gameState) < 1),
                       CardAcceptance.For<CardTypes.Lookout>(gameState => CountAllOwned<CardTypes.Lookout>(gameState) < 2 && !ShouldBuyProvinces(gameState)),                       
                       CardAcceptance.For<CardTypes.Silver>());
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Lookout>(gameState => CountInHand<CardTypes.Lookout>(gameState) > 1 ),
                           CardAcceptance.For<CardTypes.Trader>(gameState => CountAllOwned<CardTypes.Lookout>(gameState) > 1 && CountInHand<CardTypes.Lookout>(gameState) > 0 && ShouldTrashLookout(gameState)),
                           CardAcceptance.For<CardTypes.Lookout>(Default.ShouldPlayLookout(Default.ShouldBuyProvinces)),
                           CardAcceptance.For<CardTypes.Nobles>(),
                           CardAcceptance.For<CardTypes.Trader>(gameState => HasCardFromInHand(TrashOrder(), gameState)));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Curse>(),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountAllOwned<CardTypes.Silver>(gameState) >= 4 && !ShouldBuyEstates(gameState)),
                           CardAcceptance.For<CardTypes.Copper>(gameState => gameState.players.CurrentPlayer.CurrentCardBeingPlayed != null && gameState.players.CurrentPlayer.CurrentCardBeingPlayed.Is<CardTypes.Lookout>()),
                           CardAcceptance.For<CardTypes.Estate>(gameState => !ShouldBuyEstates(gameState)),
                           CardAcceptance.For<CardTypes.Lookout>(gameState => gameState.players.CurrentPlayer.CurrentCardBeingPlayed != null && gameState.players.CurrentPlayer.CurrentCardBeingPlayed.Is<CardTypes.Trader>() && ShouldTrashLookout(gameState)),
                           CardAcceptance.For<CardTypes.Copper>(),
                           CardAcceptance.For<CardTypes.Lookout>());
            }

            private static bool ShouldBuyEstates(GameState gameState)
            {
                return CountOfPile<CardTypes.Province>(gameState) <= 2;
            }

            private static bool ShouldTrashLookout(GameState gameState)
            {
                return CountAllOwned<CardTypes.Copper>(gameState) <= 4;
            }

            private static bool ShouldBuyProvinces(GameState gameState)
            {
                return CountAllOwned<CardTypes.Copper>(gameState) <= 4;
            }
        }        
    }
}

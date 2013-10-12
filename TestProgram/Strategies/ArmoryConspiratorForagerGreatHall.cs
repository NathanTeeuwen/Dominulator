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
        // for forum post: http://forum.dominionstrategy.com/index.php?topic=9558.0
        public static class ArmoryConspiratorForagerGreatHall
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
                    : base("ArmoryConspiratorForagerGreatHall",
                        playerNumber,
                        purchaseOrder: PurchaseOrder(),                        
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder())
                {
                }              
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Armory.card, gameState => CountAllOwned(CardTypes.Armory.card, gameState) < 1),
                           CardAcceptance.For(CardTypes.Forager.card, gameState => CountAllOwned(CardTypes.Forager.card, gameState) < 1),
                    //CardAcceptance.For(CardTypes.Province.card, gameState => gameState.Self.AvailableBuys == 2 && gameState.Self.AvailableCoins >= 16 || CountAllOwned(CardTypes.Province.card, gameState) > 0),
                           CardAcceptance.For(CardTypes.Province.card),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 1),                           
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) == 0),
                           CardAcceptance.For(CardTypes.GreatHall.card, gameState => gameState.Self.AvailableBuys > 1 && gameState.Self.AvailableCoins == 6),
                           CardAcceptance.For(CardTypes.GreatHall.card, gameState => gameState.Self.Hand.HasCard(CardTypes.Hovel.card)),
                           CardAcceptance.For(CardTypes.GreatHall.card, gameState => CountAllOwned(CardTypes.GreatHall.card, gameState) < CountAllOwned(CardTypes.Conspirator.card, gameState)),
                           CardAcceptance.For(CardTypes.Conspirator.card),
                           CardAcceptance.For(CardTypes.GreatHall.card));
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Necropolis.card),
                           CardAcceptance.For(CardTypes.Armory.card, gameState => gameState.Self.AvailableActions > 0),                           
                           CardAcceptance.For(CardTypes.GreatHall.card),
                           CardAcceptance.For(CardTypes.Conspirator.card, gameState => gameState.Self.CountCardsPlayedThisTurn >= 2),
                           CardAcceptance.For(CardTypes.Forager.card, gameState => HasCardFromInHand(TrashOrder(), gameState)),                           
                           CardAcceptance.For(CardTypes.Conspirator.card),
                           CardAcceptance.For(CardTypes.Armory.card));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(                           
                           CardAcceptance.For(CardTypes.OvergrownEstate.card),
                           CardAcceptance.For(CardTypes.Estate.card),                           
                           CardAcceptance.For(CardTypes.Necropolis.card),
                           CardAcceptance.For(CardTypes.Copper.card),
                           CardAcceptance.For(CardTypes.Hovel.card),
                           CardAcceptance.For(CardTypes.Forager.card, gameState => CountAllOwned(CardTypes.Copper.card, gameState) <= 2 && CountAllOwned(CardTypes.Forager.card, gameState) > 1));
            }
        }
    }
}
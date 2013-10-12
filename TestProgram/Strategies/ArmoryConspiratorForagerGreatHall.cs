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
                           CardAcceptance.For(Cards.Armory, gameState => CountAllOwned(Cards.Armory, gameState) < 1),
                           CardAcceptance.For(Cards.Forager, gameState => CountAllOwned(Cards.Forager, gameState) < 1),
                    //CardAcceptance.For(Cards.Province, gameState => gameState.Self.AvailableBuys == 2 && gameState.Self.AvailableCoins >= 16 || CountAllOwned(Cards.Province, gameState) > 0),
                           CardAcceptance.For(Cards.Province),
                           CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 1),                           
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) == 0),
                           CardAcceptance.For(Cards.GreatHall, gameState => gameState.Self.AvailableBuys > 1 && gameState.Self.AvailableCoins == 6),
                           CardAcceptance.For(Cards.GreatHall, gameState => gameState.Self.Hand.HasCard(Cards.Hovel)),
                           CardAcceptance.For(Cards.GreatHall, gameState => CountAllOwned(Cards.GreatHall, gameState) < CountAllOwned(Cards.Conspirator, gameState)),
                           CardAcceptance.For(Cards.Conspirator),
                           CardAcceptance.For(Cards.GreatHall));
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Necropolis),
                           CardAcceptance.For(Cards.Armory, gameState => gameState.Self.AvailableActions > 0),                           
                           CardAcceptance.For(Cards.GreatHall),
                           CardAcceptance.For(Cards.Conspirator, gameState => gameState.Self.CountCardsPlayedThisTurn >= 2),
                           CardAcceptance.For(Cards.Forager, gameState => HasCardFromInHand(TrashOrder(), gameState)),                           
                           CardAcceptance.For(Cards.Conspirator),
                           CardAcceptance.For(Cards.Armory));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(                           
                           CardAcceptance.For(Cards.OvergrownEstate),
                           CardAcceptance.For(Cards.Estate),                           
                           CardAcceptance.For(Cards.Necropolis),
                           CardAcceptance.For(Cards.Copper),
                           CardAcceptance.For(Cards.Hovel),
                           CardAcceptance.For(Cards.Forager, gameState => CountAllOwned(Cards.Copper, gameState) <= 2 && CountAllOwned(Cards.Forager, gameState) > 1));
            }
        }
    }
}
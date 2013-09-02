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
                           CardAcceptance.For<CardTypes.Armory>(gameState => CountAllOwned<CardTypes.Armory>(gameState) < 1),
                           CardAcceptance.For<CardTypes.Forager>(gameState => CountAllOwned<CardTypes.Forager>(gameState) < 1),
                           CardAcceptance.For<CardTypes.Province>(),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) == 0),                           
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) == 0),
                           CardAcceptance.For<CardTypes.GreatHall>(gameState => gameState.players.CurrentPlayer.AvailableBuys > 1 && gameState.players.CurrentPlayer.AvailableCoins == 6),
                           CardAcceptance.For<CardTypes.GreatHall>(gameState => gameState.players.CurrentPlayer.Hand.HasCard<CardTypes.Hovel>()),
                           CardAcceptance.For<CardTypes.GreatHall>(gameState => CountAllOwned<CardTypes.GreatHall>(gameState) < CountAllOwned<CardTypes.Conspirator>(gameState)),
                           CardAcceptance.For<CardTypes.Conspirator>(),
                           CardAcceptance.For<CardTypes.GreatHall>());
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Necropolis>(),
                           CardAcceptance.For<CardTypes.Armory>(gameState => gameState.players.CurrentPlayer.AvailableActions > 0),                           
                           CardAcceptance.For<CardTypes.GreatHall>(),
                           CardAcceptance.For<CardTypes.Conspirator>(gameState => gameState.players.CurrentPlayer.CountCardsPlayedThisTurn >= 2),
                           CardAcceptance.For<CardTypes.Forager>(gameState => HasCardFromInHand(TrashOrder(), gameState)),
                           CardAcceptance.For<CardTypes.Conspirator>(),
                           CardAcceptance.For<CardTypes.Armory>());
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(                           
                           CardAcceptance.For<CardTypes.OvergrownEstate>(),
                           CardAcceptance.For<CardTypes.Estate>(),                           
                           CardAcceptance.For<CardTypes.Copper>(),
                           CardAcceptance.For<CardTypes.Hovel>());
            }
        }
    }
}

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
        public static class MountebankGovernorMaurader
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
                    : base("MountebankGovernorMaurader",
                        playerNumber,
                        purchaseOrder: PurchaseOrder(),                        
                        trashOrder: TrashOrder())
                {
                }

                public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {                    
                    if (HasCardInHand<CardTypes.Gold>(gameState))
                        return PlayerActionChoice.Trash;
                    else if (gameState.Self.ExpectedCoinValueAtEndOfTurn >= 6 && gameState.Self.ExpectedCoinValueAtEndOfTurn < 8)
                        return PlayerActionChoice.PlusCard;
                    else
                        return PlayerActionChoice.GainCard;
                }
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 2),
                           CardAcceptance.For<CardTypes.University>(gameState => CountAllOwned<CardTypes.University>(gameState) < 1),                          
                           CardAcceptance.For<CardTypes.Mountebank>(gameState => CountAllOwned<CardTypes.Mountebank>(gameState) < 1),                           
                           CardAcceptance.For<CardTypes.Gold>(),                           
                           CardAcceptance.For<CardTypes.Governor>(),                           
                           CardAcceptance.For<CardTypes.University>(gameState => CountAllOwned<CardTypes.University>(gameState) < 2),
                           //CardAcceptance.For<CardTypes.Potion>(gameState => CountAllOwned<CardTypes.Potion>(gameState) < 1),
                           CardAcceptance.For<CardTypes.Marauder>(gameState => CountAllOwned<CardTypes.Marauder>(gameState) < 1),                           
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 4),
                           CardAcceptance.For<CardTypes.Silver>());
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Hovel>());
            }
        }

        public static class GovernorMarketsquare
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
                    : base("GovernorMarketsquare",
                        playerNumber,
                        purchaseOrder: PurchaseOrder(),
                        trashOrder: TrashOrder(),
                        actionOrder:ActionOrder()
                        )
                {
                }

                public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {
                    if (HasCardInHand<CardTypes.Gold>(gameState) || HasCardInHand<CardTypes.MarketSquare>(gameState))
                        return PlayerActionChoice.Trash;
                    //else if (gameState.Self.ExpectedCoinValueAtEndOfTurn >= 6 && gameState.Self.ExpectedCoinValueAtEndOfTurn < 8)
                    //return PlayerActionChoice.PlusCard;
                    else
                        return PlayerActionChoice.PlusCard;
                }
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.University>(),
                    CardAcceptance.For<CardTypes.MarketSquare>(gameState => CountInHand<CardTypes.Governor>(gameState) == 0),
                    CardAcceptance.For<CardTypes.Governor>(),
                    CardAcceptance.For<CardTypes.Mountebank>());
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 2),                                                      
                           CardAcceptance.For<CardTypes.Gold>(),
                           //CardAcceptance.For<CardTypes.Mountebank>(gameState => CountAllOwned<CardTypes.Mountebank>(gameState) < 1),
                           CardAcceptance.For<CardTypes.Governor>(),
                           CardAcceptance.For<CardTypes.University>(gameState => CountAllOwned<CardTypes.University>(gameState) < 2),
                           CardAcceptance.For<CardTypes.Potion>(gameState => CountAllOwned<CardTypes.Potion>(gameState) < 1 && CountAllOwned<CardTypes.University>(gameState) == 0),
                           //CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 4),
                           CardAcceptance.For<CardTypes.MarketSquare>(),
                           CardAcceptance.For<CardTypes.Silver>());
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Gold>(),
                           //CardAcceptance.For<CardTypes.Potion>(),
                           CardAcceptance.For<CardTypes.Curse>(),
                           CardAcceptance.For<CardTypes.OvergrownEstate>(),
                           CardAcceptance.For<CardTypes.RuinedLibrary>(),
                           CardAcceptance.For<CardTypes.RuinedVillage>(),
                           CardAcceptance.For<CardTypes.RuinedMarket>(),
                           CardAcceptance.For<CardTypes.AbandonedMine>(),
                           CardAcceptance.For<CardTypes.Survivors>(),
                           CardAcceptance.For<CardTypes.Estate>(),
                           CardAcceptance.For<CardTypes.Hovel>(),
                           CardAcceptance.For<CardTypes.Copper>());
            }
        }
    }
}
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
                    if (HasCardInHand(CardTypes.Gold.card, gameState))
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
                           CardAcceptance.For(CardTypes.Province.card),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 4),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 2),
                           CardAcceptance.For(CardTypes.University.card, gameState => CountAllOwned(CardTypes.University.card, gameState) < 1),                          
                           CardAcceptance.For(CardTypes.Mountebank.card, gameState => CountAllOwned(CardTypes.Mountebank.card, gameState) < 1),                           
                           CardAcceptance.For(CardTypes.Gold.card),                           
                           CardAcceptance.For(CardTypes.Governor.card),                           
                           CardAcceptance.For(CardTypes.University.card, gameState => CountAllOwned(CardTypes.University.card, gameState) < 2),
                           //CardAcceptance.For(CardTypes.Potion.card, gameState => CountAllOwned(CardTypes.Potion.card, gameState) < 1),
                           CardAcceptance.For(CardTypes.Marauder.card, gameState => CountAllOwned(CardTypes.Marauder.card, gameState) < 1),                           
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 4),
                           CardAcceptance.For(CardTypes.Silver.card));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(CardTypes.Hovel.card));
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
                    if (HasCardInHand(CardTypes.Gold.card, gameState) || HasCardInHand(CardTypes.MarketSquare.card, gameState))
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
                    CardAcceptance.For(CardTypes.University.card),
                    CardAcceptance.For(CardTypes.MarketSquare.card, gameState => CountInHand(CardTypes.Governor.card, gameState) == 0),
                    CardAcceptance.For(CardTypes.Governor.card),
                    CardAcceptance.For(CardTypes.Mountebank.card));
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Province.card),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 4),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 2),                                                      
                           CardAcceptance.For(CardTypes.Gold.card),
                           //CardAcceptance.For(CardTypes.Mountebank.card, gameState => CountAllOwned(CardTypes.Mountebank.card, gameState) < 1),
                           CardAcceptance.For(CardTypes.Governor.card),
                           CardAcceptance.For(CardTypes.University.card, gameState => CountAllOwned(CardTypes.University.card, gameState) < 2),
                           CardAcceptance.For(CardTypes.Potion.card, gameState => CountAllOwned(CardTypes.Potion.card, gameState) < 1 && CountAllOwned(CardTypes.University.card, gameState) == 0),
                           //CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 4),
                           CardAcceptance.For(CardTypes.MarketSquare.card),
                           CardAcceptance.For(CardTypes.Silver.card));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Gold.card),
                           //CardAcceptance.For(CardTypes.Potion.card),
                           CardAcceptance.For(CardTypes.Curse.card),
                           CardAcceptance.For(CardTypes.OvergrownEstate.card),
                           CardAcceptance.For(CardTypes.RuinedLibrary.card),
                           CardAcceptance.For(CardTypes.RuinedVillage.card),
                           CardAcceptance.For(CardTypes.RuinedMarket.card),
                           CardAcceptance.For(CardTypes.AbandonedMine.card),
                           CardAcceptance.For(CardTypes.Survivors.card),
                           CardAcceptance.For(CardTypes.Estate.card),
                           CardAcceptance.For(CardTypes.Hovel.card),
                           CardAcceptance.For(CardTypes.Copper.card));
            }
        }
    }
}
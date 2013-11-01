using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program;

namespace Strategies
{    
    public class MountebankGovernorMaurader
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
                : base("MountebankGovernorMaurader",                        
                    purchaseOrder: PurchaseOrder(),                        
                    trashOrder: TrashOrder())
            {
            }

            public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
            {                    
                if (HasCardInHand(Cards.Gold, gameState))
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
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),
                        CardAcceptance.For(Cards.University, gameState => CountAllOwned(Cards.University, gameState) < 1),                          
                        CardAcceptance.For(Cards.Mountebank, gameState => CountAllOwned(Cards.Mountebank, gameState) < 1),                           
                        CardAcceptance.For(Cards.Gold),                           
                        CardAcceptance.For(Cards.Governor),                           
                        CardAcceptance.For(Cards.University, gameState => CountAllOwned(Cards.University, gameState) < 2),
                        //CardAcceptance.For(Cards.Potion, gameState => CountAllOwned(Cards.Potion, gameState) < 1),
                        CardAcceptance.For(Cards.Marauder, gameState => CountAllOwned(Cards.Marauder, gameState) < 1),                           
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        CardAcceptance.For(Cards.Silver));
        }

        private static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Hovel));
        }
    }

    public class GovernorMarketsquare
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
                : base("GovernorMarketsquare",                        
                    purchaseOrder: PurchaseOrder(),
                    trashOrder: TrashOrder(),
                    actionOrder:ActionOrder()
                    )
            {
            }

            public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
            {
                if (HasCardInHand(Cards.Gold, gameState) || HasCardInHand(Cards.MarketSquare, gameState))
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
                CardAcceptance.For(Cards.University),
                CardAcceptance.For(Cards.MarketSquare, gameState => CountInHand(Cards.Governor, gameState) == 0),
                CardAcceptance.For(Cards.Governor),
                CardAcceptance.For(Cards.Mountebank));
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),                                                      
                        CardAcceptance.For(Cards.Gold),
                        //CardAcceptance.For(Cards.Mountebank, gameState => CountAllOwned(Cards.Mountebank, gameState) < 1),
                        CardAcceptance.For(Cards.Governor),
                        CardAcceptance.For(Cards.University, gameState => CountAllOwned(Cards.University, gameState) < 2),
                        CardAcceptance.For(Cards.Potion, gameState => CountAllOwned(Cards.Potion, gameState) < 1 && CountAllOwned(Cards.University, gameState) == 0),
                        //CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        CardAcceptance.For(Cards.MarketSquare),
                        CardAcceptance.For(Cards.Silver));
        }

        private static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Gold),
                        //CardAcceptance.For(Cards.Potion),
                        CardAcceptance.For(Cards.Curse),
                        CardAcceptance.For(Cards.OvergrownEstate),
                        CardAcceptance.For(Cards.RuinedLibrary),
                        CardAcceptance.For(Cards.RuinedVillage),
                        CardAcceptance.For(Cards.RuinedMarket),
                        CardAcceptance.For(Cards.AbandonedMine),
                        CardAcceptance.For(Cards.Survivors),
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.Hovel),
                        CardAcceptance.For(Cards.Copper));
        }
    }    
}
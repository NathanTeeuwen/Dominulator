using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program;

//Author: SheCantSayNo

namespace Strategies
{    
    public class RebuildAdvanced
        : Strategy
    {
        public static PlayerAction Player()
        {
            return new MyPlayerAction();
        }

        public class MyPlayerAction
            : PlayerAction
        {
            public MyPlayerAction()
                : this("RebuildAdvanced", null, CardAcceptance.AlwaysMatch)                        
            {
            }

            public MyPlayerAction(string strategyName, Card withCard, GameStatePredicate withCardPurchaseCondition)
                : base(strategyName,                        
                    purchaseOrder: PurchaseOrder(withCard, withCardPurchaseCondition),
                    treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                    actionOrder: ActionOrder(withCard))
            {
            }               
        }

        private static CardPickByPriority PurchaseOrder(Card withCard, GameStatePredicate withCardPurchaseCondition)
        {
            return new CardPickByPriority(
                CardAcceptance.For(Cards.Province),
                           
                CardAcceptance.For(Cards.Rebuild, gameState => 
                    CountAllOwned(Cards.Rebuild, gameState) < 2),
                    
                //In non-mirrors, get a 3rd Rebuild
                CardAcceptance.For(Cards.Rebuild, gameState =>
                    CountAllOwned(Cards.Rebuild, gameState) < 3 && 
                    CountOfPile(Cards.Rebuild, gameState) > 7),

                CardAcceptance.For(Cards.Duchy),

                CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 1),

                CardAcceptance.For(Cards.Estate, gameState =>
                    CountOfPile(Cards.Province, gameState) == 2 &&
                    PlayersPointLead(gameState) > -8),
                           
                CardAcceptance.For(Cards.Gold),
                           
                CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),

                CardAcceptance.For(Cards.Rebuild, gameState => 
                    (CountAllOwned(Cards.Duchy, gameState) > 0 || PlayersPointLead(gameState) > 2) && 
                    (CountOfPile(Cards.Rebuild, gameState) > 2 || 
                    PlayersPointLead(gameState) > 3 || 
                    (CountOfPile(Cards.Rebuild, gameState) == 1 && PlayersPointLead(gameState) > 0))),

                CardAcceptance.For(Cards.Estate, gameState => 
                    CountOfPile(Cards.Duchy, gameState) >= 4 && 
                    CountAllOwned(Cards.Duchy, gameState) == 0 && 
                    CountAllOwned(Cards.Estate, gameState) == 0),

                CardAcceptance.For(Cards.Estate, gameState => 
                    CountOfPile(Cards.Duchy, gameState) == 0 && 
                    CountAllOwned(Cards.Duchy, gameState) == 0),

                new CardAcceptance(withCard, withCardPurchaseCondition),

                CardAcceptance.For(Cards.Silver));
        }

        private static CardPickByPriority ActionOrder(Card withCard)
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Rebuild, ShouldPlayRebuild),
                        new CardAcceptance(withCard)
                        );
        }            

        private static bool ShouldPlayRebuild(GameState gameState)
        {
            return !(gameState.players.CurrentPlayer.ExpectedCoinValueAtEndOfTurn >= 8 && CountOfPile(Cards.Province, gameState) == 1)
                    && !(CountOfPile(Cards.Duchy, gameState) == 0
                    && CountInDeckAndDiscard(Cards.Duchy, gameState) == 0 && PlayersPointLead(gameState) < 0)
                    && CountOfPile(Cards.Province, gameState) > 0;
        }
    }

    public class RebuildJack
        : Strategy
    {        
        public static PlayerAction Player()
        {
            return new MyPlayerAction();
        }

        class MyPlayerAction
            : RebuildAdvanced.MyPlayerAction
        {
            public MyPlayerAction()
                : base("RebuildJack",
                        Cards.JackOfAllTrades,
                        gameState => CountAllOwned(Cards.JackOfAllTrades, gameState) < 1)
            {
            }
        }          
    }

    public class RebuildMonument
        : Strategy
    {            
        public static PlayerAction Player()
        {
            return new MyPlayerAction();
        }

        class MyPlayerAction
            : RebuildAdvanced.MyPlayerAction
        {
            public MyPlayerAction()
                : base("RebuildMonument",
                        Cards.Monument,
                        gameState => CountAllOwned(Cards.Monument, gameState) < 2)
            {
            }
        }
    }
}

using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Author: SheCantSayNo

namespace Program
{
    public static partial class Strategies
    {
        public static class RebuildAdvanced
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

                public override Card NameACard(GameState gameState)
                {

                    PlayerState self = gameState.Self;

                    int pointLead = PlayersPointLead(gameState); 

                    //Name Duchy
                    if (CountOfPile(Cards.Duchy, gameState) > 0 &&
                        CountInDeckAndDiscard(Cards.Estate, gameState) > 0 &&
                        (CountInDeckAndDiscard(Cards.Province, gameState) == 0 || 
                           CountInDeck(Cards.Province, gameState) == 0 &&
                           CountInDeck(Cards.Duchy, gameState) > 0 && 
                           CountInDeck(Cards.Estate, gameState) > 0))
                    {
                        return Cards.Duchy;
                    }

                    //Name Province if you are ensured of gaining a Province
                    if (CountInDeck(Cards.Estate, gameState) == 0 &&
                        CountInDeck(Cards.Province, gameState) >= 0 && 
                        CountInDeck(Cards.Duchy, gameState) > 0)
                    {
                        return Cards.Province;
                    }

                    //Name Province if you are ensured of gaining a Province
                    if (CountInDeckAndDiscard(Cards.Estate, gameState) == 0
                        && CountInDeckAndDiscard(Cards.Province, gameState) >= 0
                        && CountInDeckAndDiscard(Cards.Duchy, gameState) > 0)
                    {
                        return Cards.Province;
                    }

                    //Name Estate if you can end it with a win                    
                    if (CountInHand(Cards.Rebuild, gameState) + 1 >= CountOfPile(Cards.Province, gameState) && 
                        pointLead > 0)
                    {
                        return Cards.Estate;
                    }

                    //Name Estate if it's the only thing left in your draw pile and the Duchies are gone
                    if (CountOfPile(Cards.Duchy, gameState) == 0 &&
                        CountInDeck(Cards.Province, gameState) == 0 && 
                        CountInDeck(Cards.Estate, gameState) > 0)
                    {
                        return Cards.Estate;
                    }

                    //Name Province if Duchy is in Draw and Draw contains more P than E
                    if (CountOfPile(Cards.Duchy, gameState) == 0 && 
                        CountInDeck(Cards.Duchy, gameState) > 0 && 
                        CountInDeck(Cards.Province, gameState) > CountInDeck(Cards.Estate, gameState))
                    {
                        return Cards.Province;
                    }

                    //Name Estate if you're ahead and both P and E are left in draw
                    if (CountOfPile(Cards.Duchy, gameState) == 0 && 
                        CountInDeck(Cards.Province, gameState) > 0 && 
                        CountInDeck(Cards.Estate, gameState) > 0 && 
                        pointLead > 2)
                    {
                        return Cards.Estate;
                    }

                    //Name Estate over Province if you're way ahead
                    if (CountOfPile(Cards.Duchy, gameState) == 0 && 
                        CountInDeckAndDiscard(Cards.Province, gameState) > 0 &&
                        CountInDeckAndDiscard(Cards.Duchy, gameState) < 3 && 
                        CountInDeckAndDiscard(Cards.Estate, gameState) > 0 && 
                        pointLead > 4)
                    {
                        return Cards.Estate;
                    }

                    //Province -> Province when ahead without any Duchies left
                    if (CountOfPile(Cards.Duchy, gameState) == 0 && 
                        CountAllOwned(Cards.Duchy, gameState) == 0 &&
                        pointLead > 0)
                    {
                        return Cards.Estate;
                    }

                    //Province -> Province when ahead without any Duchies not in hand
                    if (CountOfPile(Cards.Duchy, gameState) == 0 && 
                        CountInDeckAndDiscard(Cards.Duchy, gameState) == 0 && 
                        CountInDeckAndDiscard(Cards.Province, gameState) > 0 && 
                        pointLead > 2)
                    {
                        return Cards.Estate;
                    }

                    if (CountInDeckAndDiscard(Cards.Province, gameState) > 0)
                    {
                        return Cards.Province;
                    }

                    return Cards.Estate;
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

        public static class RebuildJack
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

        public static class RebuildMonument
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
}

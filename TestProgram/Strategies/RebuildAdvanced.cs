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

            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            public class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : this(playerNumber, "RebuildAdvanced", null, CardAcceptance.AlwaysMatch)                        
                {
                }

                public MyPlayerAction(int playerNumber, string strategyName, Card withCard, GameStatePredicate withCardPurchaseCondition)
                    : base(strategyName,
                        playerNumber,
                        purchaseOrder: PurchaseOrder(withCard, withCardPurchaseCondition),
                        treasurePlayOrder: Default.TreasurePlayOrder(),
                        actionOrder: ActionOrder(withCard))
                {
                }

                public override Type NameACard(GameState gameState)
                {

                    PlayerState currentPlayer = gameState.players.CurrentPlayer;


                    int pointLead = PlayersPointLead(gameState); 

                    //Name Duchy
                    if (CountOfPile<CardTypes.Duchy>(gameState) > 0 &&
                        CountInDeckAndDiscard<CardTypes.Estate>(gameState) > 0 &&
                        (CountInDeckAndDiscard<CardTypes.Province>(gameState) == 0 || 
                           CountInDeck<CardTypes.Province>(gameState) == 0 &&
                           CountInDeck<CardTypes.Duchy>(gameState) > 0 && 
                           CountInDeck<CardTypes.Estate>(gameState) > 0))
                    {
                        return typeof(CardTypes.Duchy);
                    }

                    //Name Province if you are ensured of gaining a Province
                    if (CountInDeck<CardTypes.Estate>(gameState) == 0 &&
                        CountInDeck<CardTypes.Province>(gameState) >= 0 && 
                        CountInDeck<CardTypes.Duchy>(gameState) > 0)
                    {
                        return typeof(CardTypes.Province);
                    }

                    //Name Province if you are ensured of gaining a Province
                    if (CountInDeckAndDiscard<CardTypes.Estate>(gameState) == 0
                        && CountInDeckAndDiscard<CardTypes.Province>(gameState) >= 0
                        && CountInDeckAndDiscard<CardTypes.Duchy>(gameState) > 0)
                    {
                        return typeof(CardTypes.Province);
                    }

                    //Name Estate if you can end it with a win                    
                    if (CountInHand<CardTypes.Rebuild>(gameState) + 1 >= CountOfPile<CardTypes.Province>(gameState) && 
                        pointLead > 0)
                    {
                        return typeof(CardTypes.Estate);
                    }

                    //Name Estate if it's the only thing left in your draw pile and the Duchies are gone
                    if (CountOfPile<CardTypes.Duchy>(gameState) == 0 &&
                        CountInDeck<CardTypes.Province>(gameState) == 0 && 
                        CountInDeck<CardTypes.Estate>(gameState) > 0)
                    {
                        return typeof(CardTypes.Estate);
                    }

                    //Name Province if Duchy is in Draw and Draw contains more P than E
                    if (CountOfPile<CardTypes.Duchy>(gameState) == 0 && 
                        CountInDeck<CardTypes.Duchy>(gameState) > 0 && 
                        CountInDeck<CardTypes.Province>(gameState) > CountInDeck<CardTypes.Estate>(gameState))
                    {
                        return typeof(CardTypes.Province);
                    }

                    //Name Estate if you're ahead and both P and E are left in draw
                    if (CountOfPile<CardTypes.Duchy>(gameState) == 0 && 
                        CountInDeck<CardTypes.Province>(gameState) > 0 && 
                        CountInDeck<CardTypes.Estate>(gameState) > 0 && 
                        pointLead > 2)
                    {
                        return typeof(CardTypes.Estate);
                    }

                    //Name Estate over Province if you're way ahead
                    if (CountOfPile<CardTypes.Duchy>(gameState) == 0 && 
                        CountInDeckAndDiscard<CardTypes.Province>(gameState) > 0 &&
                        CountInDeckAndDiscard<CardTypes.Duchy>(gameState) < 3 && 
                        CountInDeckAndDiscard<CardTypes.Estate>(gameState) > 0 && 
                        pointLead > 4)
                    {
                        return typeof(CardTypes.Estate);
                    }

                    //Province -> Province when ahead without any Duchies left
                    if (CountOfPile<CardTypes.Duchy>(gameState) == 0 && 
                        CountAllOwned<CardTypes.Duchy>(gameState) == 0 &&
                        pointLead > 0)
                    {
                        return typeof(CardTypes.Estate);
                    }

                    //Province -> Province when ahead without any Duchies not in hand
                    if (CountOfPile<CardTypes.Duchy>(gameState) == 0 && 
                        CountInDeckAndDiscard<CardTypes.Duchy>(gameState) == 0 && 
                        CountInDeckAndDiscard<CardTypes.Province>(gameState) > 0 && 
                        pointLead > 2)
                    {
                        return typeof(CardTypes.Estate);
                    }

                    if (CountInDeckAndDiscard<CardTypes.Province>(gameState) > 0)
                    {
                        return typeof(CardTypes.Province);
                    }

                    return typeof(CardTypes.Estate);
                }
            }

            private static CardPickByPriority PurchaseOrder(Card withCard, GameStatePredicate withCardPurchaseCondition)
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Province>(),
                           
                    CardAcceptance.For<CardTypes.Rebuild>(gameState => 
                        CountAllOwned<CardTypes.Rebuild>(gameState) < 2),
                    
                    //In non-mirrors, get a 3rd Rebuild
                    CardAcceptance.For<CardTypes.Rebuild>(gameState =>
                        CountAllOwned<CardTypes.Rebuild>(gameState) < 3 && 
                        CountOfPile<CardTypes.Rebuild>(gameState) > 7),

                    CardAcceptance.For<CardTypes.Duchy>(),

                    CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 1),

                    CardAcceptance.For<CardTypes.Estate>(gameState =>
                        CountOfPile<CardTypes.Province>(gameState) == 2 &&
                        PlayersPointLead(gameState) > -8),
                           
                    CardAcceptance.For<CardTypes.Gold>(),
                           
                    CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 2),

                    CardAcceptance.For<CardTypes.Rebuild>(gameState => 
                        (CountAllOwned<CardTypes.Duchy>(gameState) > 0 || PlayersPointLead(gameState) > 2) && 
                        (CountOfPile<CardTypes.Rebuild>(gameState) > 2 || 
                        PlayersPointLead(gameState) > 3 || 
                        (CountOfPile<CardTypes.Rebuild>(gameState) == 1 && PlayersPointLead(gameState) > 0))),

                    CardAcceptance.For<CardTypes.Estate>(gameState => 
                        CountOfPile<CardTypes.Duchy>(gameState) >= 4 && 
                        CountAllOwned<CardTypes.Duchy>(gameState) == 0 && 
                        CountAllOwned<CardTypes.Estate>(gameState) == 0),

                    CardAcceptance.For<CardTypes.Estate>(gameState => 
                        CountOfPile<CardTypes.Duchy>(gameState) == 0 && 
                        CountAllOwned<CardTypes.Duchy>(gameState) == 0),

                    new CardAcceptance(withCard, withCardPurchaseCondition),

                    CardAcceptance.For<CardTypes.Silver>());
            }

            private static CardPickByPriority ActionOrder(Card withCard)
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Rebuild>(ShouldPlayRebuild),
                           new CardAcceptance(withCard)
                           );
            }            

            private static bool ShouldPlayRebuild(GameState gameState)
            {
                return !(gameState.players.CurrentPlayer.ExpectedCoinValueAtEndOfTurn >= 8 && CountOfPile<CardTypes.Province>(gameState) == 1)
                       && !(CountOfPile<CardTypes.Duchy>(gameState) == 0
                       && CountInDeckAndDiscard<CardTypes.Duchy>(gameState) == 0 && PlayersPointLead(gameState) < 0)
                       && CountOfPile<CardTypes.Province>(gameState) > 0;
            }
        }

        public static class RebuildJack
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : RebuildAdvanced.MyPlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base(playerNumber,
                           "RebuildJack",
                           Example<CardTypes.JackOfAllTrades>.Card,
                           gameState => CountAllOwned<CardTypes.JackOfAllTrades>(gameState) < 1)
                {
                }
            }          
        }

        public static class RebuildMonument
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : RebuildAdvanced.MyPlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base(playerNumber,
                           "RebuildMonument",
                           Example<CardTypes.Monument>.Card,
                           gameState => CountAllOwned<CardTypes.Monument>(gameState) < 3)
                {
                }
            }
        }                 
    }
}

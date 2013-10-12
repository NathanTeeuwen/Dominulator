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
                        treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                        actionOrder: ActionOrder(withCard))
                {
                }

                public override Card NameACard(GameState gameState)
                {

                    PlayerState self = gameState.Self;

                    int pointLead = PlayersPointLead(gameState); 

                    //Name Duchy
                    if (CountOfPile(CardTypes.Duchy.card, gameState) > 0 &&
                        CountInDeckAndDiscard(CardTypes.Estate.card, gameState) > 0 &&
                        (CountInDeckAndDiscard(CardTypes.Province.card, gameState) == 0 || 
                           CountInDeck(CardTypes.Province.card, gameState) == 0 &&
                           CountInDeck(CardTypes.Duchy.card, gameState) > 0 && 
                           CountInDeck(CardTypes.Estate.card, gameState) > 0))
                    {
                        return CardTypes.Duchy.card;
                    }

                    //Name Province if you are ensured of gaining a Province
                    if (CountInDeck(CardTypes.Estate.card, gameState) == 0 &&
                        CountInDeck(CardTypes.Province.card, gameState) >= 0 && 
                        CountInDeck(CardTypes.Duchy.card, gameState) > 0)
                    {
                        return CardTypes.Province.card;
                    }

                    //Name Province if you are ensured of gaining a Province
                    if (CountInDeckAndDiscard(CardTypes.Estate.card, gameState) == 0
                        && CountInDeckAndDiscard(CardTypes.Province.card, gameState) >= 0
                        && CountInDeckAndDiscard(CardTypes.Duchy.card, gameState) > 0)
                    {
                        return CardTypes.Province.card;
                    }

                    //Name Estate if you can end it with a win                    
                    if (CountInHand(CardTypes.Rebuild.card, gameState) + 1 >= CountOfPile(CardTypes.Province.card, gameState) && 
                        pointLead > 0)
                    {
                        return CardTypes.Estate.card;
                    }

                    //Name Estate if it's the only thing left in your draw pile and the Duchies are gone
                    if (CountOfPile(CardTypes.Duchy.card, gameState) == 0 &&
                        CountInDeck(CardTypes.Province.card, gameState) == 0 && 
                        CountInDeck(CardTypes.Estate.card, gameState) > 0)
                    {
                        return CardTypes.Estate.card;
                    }

                    //Name Province if Duchy is in Draw and Draw contains more P than E
                    if (CountOfPile(CardTypes.Duchy.card, gameState) == 0 && 
                        CountInDeck(CardTypes.Duchy.card, gameState) > 0 && 
                        CountInDeck(CardTypes.Province.card, gameState) > CountInDeck(CardTypes.Estate.card, gameState))
                    {
                        return CardTypes.Province.card;
                    }

                    //Name Estate if you're ahead and both P and E are left in draw
                    if (CountOfPile(CardTypes.Duchy.card, gameState) == 0 && 
                        CountInDeck(CardTypes.Province.card, gameState) > 0 && 
                        CountInDeck(CardTypes.Estate.card, gameState) > 0 && 
                        pointLead > 2)
                    {
                        return CardTypes.Estate.card;
                    }

                    //Name Estate over Province if you're way ahead
                    if (CountOfPile(CardTypes.Duchy.card, gameState) == 0 && 
                        CountInDeckAndDiscard(CardTypes.Province.card, gameState) > 0 &&
                        CountInDeckAndDiscard(CardTypes.Duchy.card, gameState) < 3 && 
                        CountInDeckAndDiscard(CardTypes.Estate.card, gameState) > 0 && 
                        pointLead > 4)
                    {
                        return CardTypes.Estate.card;
                    }

                    //Province -> Province when ahead without any Duchies left
                    if (CountOfPile(CardTypes.Duchy.card, gameState) == 0 && 
                        CountAllOwned(CardTypes.Duchy.card, gameState) == 0 &&
                        pointLead > 0)
                    {
                        return CardTypes.Estate.card;
                    }

                    //Province -> Province when ahead without any Duchies not in hand
                    if (CountOfPile(CardTypes.Duchy.card, gameState) == 0 && 
                        CountInDeckAndDiscard(CardTypes.Duchy.card, gameState) == 0 && 
                        CountInDeckAndDiscard(CardTypes.Province.card, gameState) > 0 && 
                        pointLead > 2)
                    {
                        return CardTypes.Estate.card;
                    }

                    if (CountInDeckAndDiscard(CardTypes.Province.card, gameState) > 0)
                    {
                        return CardTypes.Province.card;
                    }

                    return CardTypes.Estate.card;
                }
            }

            private static CardPickByPriority PurchaseOrder(Card withCard, GameStatePredicate withCardPurchaseCondition)
            {
                return new CardPickByPriority(
                    CardAcceptance.For(CardTypes.Province.card),
                           
                    CardAcceptance.For(CardTypes.Rebuild.card, gameState => 
                        CountAllOwned(CardTypes.Rebuild.card, gameState) < 2),
                    
                    //In non-mirrors, get a 3rd Rebuild
                    CardAcceptance.For(CardTypes.Rebuild.card, gameState =>
                        CountAllOwned(CardTypes.Rebuild.card, gameState) < 3 && 
                        CountOfPile(CardTypes.Rebuild.card, gameState) > 7),

                    CardAcceptance.For(CardTypes.Duchy.card),

                    CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 1),

                    CardAcceptance.For(CardTypes.Estate.card, gameState =>
                        CountOfPile(CardTypes.Province.card, gameState) == 2 &&
                        PlayersPointLead(gameState) > -8),
                           
                    CardAcceptance.For(CardTypes.Gold.card),
                           
                    CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 2),

                    CardAcceptance.For(CardTypes.Rebuild.card, gameState => 
                        (CountAllOwned(CardTypes.Duchy.card, gameState) > 0 || PlayersPointLead(gameState) > 2) && 
                        (CountOfPile(CardTypes.Rebuild.card, gameState) > 2 || 
                        PlayersPointLead(gameState) > 3 || 
                        (CountOfPile(CardTypes.Rebuild.card, gameState) == 1 && PlayersPointLead(gameState) > 0))),

                    CardAcceptance.For(CardTypes.Estate.card, gameState => 
                        CountOfPile(CardTypes.Duchy.card, gameState) >= 4 && 
                        CountAllOwned(CardTypes.Duchy.card, gameState) == 0 && 
                        CountAllOwned(CardTypes.Estate.card, gameState) == 0),

                    CardAcceptance.For(CardTypes.Estate.card, gameState => 
                        CountOfPile(CardTypes.Duchy.card, gameState) == 0 && 
                        CountAllOwned(CardTypes.Duchy.card, gameState) == 0),

                    new CardAcceptance(withCard, withCardPurchaseCondition),

                    CardAcceptance.For(CardTypes.Silver.card));
            }

            private static CardPickByPriority ActionOrder(Card withCard)
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Rebuild.card, ShouldPlayRebuild),
                           new CardAcceptance(withCard)
                           );
            }            

            private static bool ShouldPlayRebuild(GameState gameState)
            {
                return !(gameState.players.CurrentPlayer.ExpectedCoinValueAtEndOfTurn >= 8 && CountOfPile(CardTypes.Province.card, gameState) == 1)
                       && !(CountOfPile(CardTypes.Duchy.card, gameState) == 0
                       && CountInDeckAndDiscard(CardTypes.Duchy.card, gameState) == 0 && PlayersPointLead(gameState) < 0)
                       && CountOfPile(CardTypes.Province.card, gameState) > 0;
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
                           CardTypes.JackOfAllTrades.card,
                           gameState => CountAllOwned(CardTypes.JackOfAllTrades.card, gameState) < 1)
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
                           CardTypes.Monument.card,
                           gameState => CountAllOwned(CardTypes.Monument.card, gameState) < 2)
                {
                }
            }
        }                 
    }
}

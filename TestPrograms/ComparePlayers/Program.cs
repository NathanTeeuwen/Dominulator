using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CardTypes = Dominion.CardTypes;
using Dominion;
using Dominion.Strategy.Description;
using Dominion.Strategy;
using Dominion.Data;

namespace Program
{
    class Program
    {        
        static void Main()
        {            
            using (var testOutput = new TestOutput())
            {
                var player1 = Strategies.GardensCounterPlay.Player(Cards.Monument);
                var player2 = Strategies.GardensWorkshop.Player();                
                
                var builder = new GameConfigBuilder();                
                builder.CardSplit = StartingCardSplit.Split43;
                builder.SetKingdomCards(player1, player2);
                
                testOutput.ComparePlayers(
                    new PlayerAction[] { player1, player2},                    
                    builder.ToGameConfig(),
                    rotateWhoStartsFirst:true,
                    createHtmlReport: true, 
                    numberOfGames: 1000, 
                    shouldParallel: false);
            }         
        }

        public class SilverCutpurse
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
                    : base("SilverCutpurse",
                        purchaseOrder: PurchaseOrder())
                {
                }              
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                            CardAcceptance.For(Cards.Province),
                            CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                            CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) == 0),
                            CardAcceptance.For(Cards.Hoard),
                            CardAcceptance.For(Cards.Cutpurse, 2),
                            CardAcceptance.For(Cards.Silver));
            }
        }

        public class NativeVillageCutpurse
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
                    : base("NataiveVillageCutPurse",
                        purchaseOrder: PurchaseOrder())
                {
                }

                override public PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {
                    if (gameState.CurrentContext.CurrentCard == Cards.NativeVillage)
                    {
                        int expectedCoinWithNativeVillage = ValueOf(gameState.Self.Hand) + ValueOf(gameState.Self.CardsOnNativeVillageMat);
                        if (expectedCoinWithNativeVillage >= 6)
                        {                            
                            return PlayerActionChoice.PutNativeVillageMatInHand;
                        }

                        return PlayerActionChoice.SetAsideTopCardOnNativeVillageMat;
                    }
                    return base.ChooseBetween(gameState, acceptableChoice);
                }

                public int ValueOf(CollectionCards cards)
                {
                    return cards.Select( c => c.plusCoin ).Sum();                    
                }
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                            CardAcceptance.For(Cards.Province),
                            CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                            CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) == 0),
                            CardAcceptance.For(Cards.Hoard),
                            CardAcceptance.For(Cards.Cutpurse, 1),
                            CardAcceptance.For(Cards.NativeVillage, 2),
                            CardAcceptance.For(Cards.Cutpurse, 2),
                            CardAcceptance.For(Cards.Silver),
                            CardAcceptance.For(Cards.NativeVillage));
            }         
        }

        public class NativeVillageCutpurseOnly
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
                    : base("NativeVillageCutpurseOnly",
                        purchaseOrder: PurchaseOrder())
                {
                }

                override public PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {
                    if (gameState.CurrentContext.CurrentCard == Cards.NativeVillage)
                    {
                        int expectedCoinWithNativeVillage = ValueOf(gameState.Self.Hand) + ValueOf(gameState.Self.CardsOnNativeVillageMat);
                        if (expectedCoinWithNativeVillage >= 6)
                        {
                            return PlayerActionChoice.PutNativeVillageMatInHand;
                        }

                        return PlayerActionChoice.SetAsideTopCardOnNativeVillageMat;
                    }
                    return base.ChooseBetween(gameState, acceptableChoice);
                }

                public int ValueOf(CollectionCards cards)
                {
                    return cards.Select(c => c.plusCoin).Sum();
                }
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                            CardAcceptance.For(Cards.Province),
                            CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                            CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) == 0),
                            CardAcceptance.For(Cards.Hoard),
                            CardAcceptance.For(Cards.Cutpurse),
                            CardAcceptance.For(Cards.NativeVillage));
            }
        }
    }            
}

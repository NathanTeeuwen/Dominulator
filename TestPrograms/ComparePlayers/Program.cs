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
                StrategyDescription descr1 = StrategyDescription.GetDefaultStrategyDescription().AddCardToPurchaseOrder(Cards.YoungWitch).AddCardToPurchaseOrder(Cards.Sage);
                StrategyDescription descr2 = StrategyDescription.GetDefaultStrategyDescription().AddCardToPurchaseOrder(Cards.SeaHag).AddCardToPurchaseOrder(Cards.Sage);
                
                //var player1 = Strategies.BigMoneyWithCard.Player(Cards.TreasureTrove, cardCount:10);
                //var player2 = Strategies.BigMoneyWithCard.Player(Cards.Gold);                

                var player1 = descr1.ToPlayerAction("young witch");
                var player2 = descr2.ToPlayerAction("sea hag");
                
                var builder = new GameConfigBuilder();
                builder.SetBaneCard(Cards.Sage);
                builder.CardSplit = StartingCardSplit.Split43;
                builder.SetKingdomCards(player1, player2);
                
                testOutput.ComparePlayers(
                    player1,
                    player2,                    
                    builder.ToGameConfig(),
                    rotateWhoStartsFirst:true,
                    createHtmlReport: true, 
                    numberOfGames: 1000, 
                    shouldParallel: false);
            }         
        }

        static Dominion.CardCountPair[] shuffleLuckDoubleFiveOpenning = new Dominion.CardCountPair[]
        {
            new CardCountPair(Cards.Copper, 3), new CardCountPair(Cards.Estate, 2),
            new CardCountPair(Cards.Copper, 4), new CardCountPair(Cards.Estate, 1),
            new CardCountPair(Cards.Copper, 3), new CardCountPair(Cards.Estate, 1), new CardCountPair(Cards.Silver, 1),
            new CardCountPair(Cards.Copper, 3), new CardCountPair(Cards.Estate, 1), new CardCountPair(Cards.Silver, 1)
        };
    }            
}

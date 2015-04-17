using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CardTypes = Dominion.CardTypes;
using Dominion;
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
                var player1 = Strategies.BigMoneyWithCard.Player(Cards.Scavenger, cardCount:2);
                var player2 = Strategies.BigMoneyWithCard.Player(Cards.Gold);                
                
                var builder = new GameConfigBuilder();
                builder.CardSplit = StartingCardSplit.Split43;

                PlayerAction.SetKingdomCards(builder, player1, player2);
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

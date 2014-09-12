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
                var player1 = Strategies.MintBaker.PlayerCustom(playerName: "Start4");
                var player2 = Strategies.MintBaker.PlayerCustom(playerName: "Start5");

                var builder = new GameConfigBuilder();

                PlayerAction.SetKingdomCards(builder, player1, player2);
                testOutput.ComparePlayers(
                    player1,
                    player2,
                    builder.ToGameConfig(),
                    firstPlayerAdvantage:false,
                    createHtmlReport: true, 
                    numberOfGames: 10000, 
                    shouldParallel: true);
            }         
        }                
    }            
}

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
                var player1 = Strategies.GovernorSpiceMerchantHorseTraderJunkDealer.PlayerCustom(true);
                var player2 = Strategies.GovernorSpiceMerchantHorseTraderJunkDealer.PlayerCustom(false);
                //var player2 = Strategies.BigMoney.Player();
               
                var builder = new GameConfigBuilder();

                PlayerAction.SetKingdomCards(builder, player1, player2);
                testOutput.ComparePlayers(
                    player1,
                    player2,
                    builder.ToGameConfig(),
                    firstPlayerAdvantage:false,
                    createHtmlReport: true, 
                    numberOfGames: 1000, 
                    shouldParallel: false);
            }         
        }                
    }            
}

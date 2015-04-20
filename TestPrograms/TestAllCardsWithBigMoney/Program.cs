using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CardTypes = Dominion.CardTypes;
using Dominion;
using Dominion.Strategy;

namespace TestAllCardsWithBigMoney
{
    class Program
    {
        static void Main(string[] args)
        {            
            using (var testOutput = new TestOutput())
            {                
                var bigMoneyPlayer = Strategies.BigMoneyWithCard.Player(Cards.Magpie, playerName:"single magpie");
                foreach (PlayerAction playerAction in AllBigMoneyWithCard())
                {
                    testOutput.ComparePlayers(bigMoneyPlayer, playerAction, numberOfGames: 1000, shouldParallel: true, createHtmlReport: false, createRankingReport: true, logGameCount: 0);
                }                
            }          
        }

        static PlayerAction[] AllBigMoneyWithCard()
        {
            var result = new List<PlayerAction>();

            foreach (var member in typeof(Cards).GetMembers(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
            {
                if (member.MemberType == System.Reflection.MemberTypes.Field)
                {
                    object potentialCard = typeof(Cards).GetField(member.Name).GetValue(null);                    
                    Card card = potentialCard as Card;
                    if (card == null)
                        continue;

                    if (!card.isKingdomCard)
                    {
                        continue;
                    }

                    if (Dominion.Cards.UnimplementedCards.Contains(card))
                        continue;

                    if (Dominion.Strategy.MissingDefaults.CardsWithoutDefaultBehaviors.Contains(card))
                        continue;

                    var playerAction = Strategies.BigMoneyWithCard.Player(card);
                    result.Add(playerAction);
                }
            }

            return result.ToArray();
        }
    }
}

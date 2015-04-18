using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CardTypes = Dominion.CardTypes;
using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy
{
    public class MissingDefaults
    {
        public static Card[] CardsWithoutDefaultBehaviors = new Card[]
        {
            // implemented cards that require default behaviors            
            Cards.Embargo,            
            Cards.Graverobber,
            Cards.Haven,            
            Cards.Inn,
            Cards.Journeyman,        
            Cards.Mandarin,            
            Cards.Minion,          
            Cards.NativeVillage,
            Cards.Navigator,
            Cards.Oracle,
            Cards.Pawn,
            Cards.PirateShip,            
            Cards.Squire,            
            Cards.StoneMason,
            Cards.Torturer,
            Cards.Tournament,              
        };

        public static IEnumerable<Card> FullyImplementedKingdomCards()
        {
            foreach(Card card in Dominion.Cards.AllKingdomCards())
            {
                if (Cards.UnimplementedCards.Contains(card))
                    continue;
                if (CardsWithoutDefaultBehaviors.Contains(card))
                    continue;

                yield return card;
            }
        }

        public static IEnumerable<Card> UnImplementedKingdomCards()
        {

            foreach (Card card in Cards.UnimplementedCards)
            {                                
                yield return card;
            }

            foreach (Card card in CardsWithoutDefaultBehaviors)
            {                
                yield return card;
            }
        }        
    }
}

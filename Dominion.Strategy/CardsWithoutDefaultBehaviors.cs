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
            Cards.Herald,
            Cards.Herbalist,
            Cards.Inn,
            Cards.Island,            
            Cards.Mandarin,
            Cards.Masquerade,
            Cards.Masterpiece,
            Cards.Minion,          
            Cards.NativeVillage,
            Cards.Navigator,
            Cards.NomadCamp,
            Cards.Oracle,
            Cards.Pawn,
            Cards.PearlDiver,
            Cards.PirateShip,
            Cards.Scavenger,
            Cards.Scout,
            Cards.Squire,            
            Cards.StoneMason,
            Cards.Torturer,
            Cards.Tournament,            
            Cards.Vault,                   
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
    }
}

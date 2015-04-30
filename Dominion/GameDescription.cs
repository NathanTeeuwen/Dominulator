using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class GameDescription
    {
        public readonly Card[] kingdomPiles;
        public readonly Card[] events;
        public readonly Card baneCard;
        public readonly bool useShelters;
        public readonly bool useColonyAndPlatinum;        

        public GameDescription(Card[] kingdomPiles, Card[] events, Card baneCard, bool useShelters, bool useColonyAndPlatinum)
        {
            this.kingdomPiles = kingdomPiles;
            this.baneCard = baneCard;
            this.events = events;
            this.useShelters = useShelters;
            this.useColonyAndPlatinum = useColonyAndPlatinum;
        }

        public GameDescription(string[] kingdomPiles, string[] events, string baneCard, bool useShelters, bool useColonyAndPlatinum)
            : this(GetCardsFromProgrammaticNames(kingdomPiles), GetCardsFromProgrammaticNames(events), GetCardFromProgrammaticName(baneCard), useShelters, useColonyAndPlatinum)
        {            
        }

        public string[] KingdomPileProgramaticNames()
        {
            return this.kingdomPiles.Select(c => c.ProgrammaticName).ToArray();
        }

        public string[] EventProgramaticNames()
        {
            return this.events.Select(c => c.ProgrammaticName).ToArray();
        }

        public string BanePileProgrammaticName()
        {
            return this.baneCard != null ? baneCard.ProgrammaticName : null;
        }

        public Expansion[] GetRequiredExpansions()
        {
            var isExpansionRequired = new bool[(int)Expansion.Count];

            foreach(Card card in kingdomPiles)
            {
                isExpansionRequired[(int)card.expansion] = true;
            }

            foreach (Card card in events)
            {
                isExpansionRequired[(int)card.expansion] = true;
            }
            
            if (this.baneCard != null)
            {
                isExpansionRequired[(int)baneCard.expansion] = true;
            }

            var result = new List<Expansion>();
            for (int i = 0; i < isExpansionRequired.Length; ++i)
            {
                if (isExpansionRequired[i])
                {
                    result.Add((Expansion)i);
                }
            }

            return result.ToArray();
        }

        public static Card GetCardFromProgrammaticName(string name)
        {
            return Cards.AllCards().Where(c => c.ProgrammaticName == name).FirstOrDefault();
        }

        public static Card[] GetCardsFromProgrammaticNames(string[] cardNames)
        {
            return cardNames.Select(name => GetCardFromProgrammaticName(name)).ToArray();
        }
    }
}

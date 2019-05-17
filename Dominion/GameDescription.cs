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
        public readonly Event[] events;
        public readonly Landmark[] landmarks;
        public readonly Card baneCard;
        public readonly bool useShelters;
        public readonly bool useColonyAndPlatinum;

        public GameDescription(Card[] kingdomPiles, Event[] events, Landmark[] landmarks, Card baneCard, bool useShelters, bool useColonyAndPlatinum)
        {
            this.kingdomPiles = kingdomPiles;
            this.baneCard = baneCard;
            this.events = events;
            this.landmarks = landmarks;
            this.useShelters = useShelters;
            this.useColonyAndPlatinum = useColonyAndPlatinum;
        }

        public GameDescription(string[] kingdomPiles, string[] events, string[] landmarks, string baneCard, bool useShelters, bool useColonyAndPlatinum)
            : this(GetCardsFromProgrammaticNames<Card>(kingdomPiles), GetCardsFromProgrammaticNames<Event>(events), GetCardsFromProgrammaticNames<Landmark>(landmarks), GetCardFromProgrammaticName<Card>(baneCard), useShelters, useColonyAndPlatinum)
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

        public void GetRequiredExpansions(out Expansion[] present, out Expansion[] missing)
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

            var presentList = new List<Expansion>();
            var missingList = new List<Expansion>();
            for (int i = 0; i < isExpansionRequired.Length; ++i)
            {
                if (isExpansionRequired[i])
                {
                    presentList.Add((Expansion)i);
                }
                else
                {
                    missingList.Add((Expansion)i);
                }
            }

            present = presentList.ToArray();
            missing = missingList.ToArray();
        }

        public static T GetCardFromProgrammaticName<T>(string name)
            where T : CardShapedObject
        {
            return (T)(Cards.AllCardsList.Where(c => c.ProgrammaticName == name).FirstOrDefault());
        }

        public static T[] GetCardsFromProgrammaticNames<T>(string[] cardNames)
            where T: CardShapedObject
        {
            return cardNames.Select(name => GetCardFromProgrammaticName<T>(name)).ToArray();
        }
    }
}

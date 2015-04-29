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
        public readonly Card baneCard;
        public readonly bool useShelters;
        public readonly bool useColonyAndPlatinum;

        public GameDescription(Card[] kingdomPiles, Card baneCard, bool useShelters, bool useColonyAndPlatinum)
        {
            this.kingdomPiles = kingdomPiles;
            this.baneCard = baneCard;
            this.useShelters = useShelters;
            this.useColonyAndPlatinum = useColonyAndPlatinum;
        }

        public GameDescription(string[] kingdomPiles, string baneCard, bool useShelters, bool useColonyAndPlatinum)
            : this(GetCardsFromNames(kingdomPiles), GetCardFromName(baneCard), useShelters, useColonyAndPlatinum)
        {            
        }

        public string[] KingdomPileNames()
        {
            return this.kingdomPiles.Select(c => c.name).ToArray();
        }

        public string BanePileName()
        {
            return this.baneCard != null ? baneCard.name : null;
        }

        public Expansion[] GetRequiredExpansions()
        {
            var isExpansionRequired = new bool[(int)Expansion.Count];

            foreach(Card card in kingdomPiles)
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

        public static Card GetCardFromName(string name)
        {
            return Cards.AllCards().Where(c => c.name == name).FirstOrDefault();
        }

        public static Card[] GetCardsFromNames(string[] cardNames)
        {
            return cardNames.Select(name => GetCardFromName(name)).ToArray();
        }
    }
}

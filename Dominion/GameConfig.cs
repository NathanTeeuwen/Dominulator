using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class GameConfig
    {
        public readonly bool useShelters;
        public readonly bool useColonyAndPlatinum;
        public readonly Card[] kingdomPiles;
        internal readonly int parameter;

        public GameConfig(bool useShelters, bool useColonyAndPlatinum, int parameter, params Card[] supplyPiles)
        {
            this.useShelters = useShelters;
            this.useColonyAndPlatinum = useColonyAndPlatinum;
            this.kingdomPiles = supplyPiles;
            this.parameter = parameter;
        }

        public GameConfig(params Card[] supplyPiles)
        {
            this.useShelters = false;
            this.useColonyAndPlatinum = false;
            this.kingdomPiles = supplyPiles;
            this.parameter = 0;
        }

        public PileOfCards[] GetSupplyPiles(int playerCount, Random random)
        {
            var supplyCardPiles = new List<PileOfCards>(capacity: 20);            

            int curseCount = (playerCount - 1) * 10;
            int ruinsCount = curseCount;
            int victoryCount = (playerCount == 2) ? 8 : 12;

            Add<CardTypes.Copper>(supplyCardPiles, 60);
            Add<CardTypes.Silver>(supplyCardPiles, 40);
            Add<CardTypes.Gold>(supplyCardPiles, 30);
            Add<CardTypes.Curse>(supplyCardPiles, curseCount);
            Add<CardTypes.Estate>(supplyCardPiles, victoryCount);
            Add<CardTypes.Duchy>(supplyCardPiles, victoryCount);
            Add<CardTypes.Province>(supplyCardPiles, victoryCount);
            
            if (this.useColonyAndPlatinum)
            {
                Add<CardTypes.Colony>(supplyCardPiles, victoryCount);
                Add<CardTypes.Platinum>(supplyCardPiles, 20);
            }

            bool requiresRuins = false;

            foreach (Card card in this.kingdomPiles)
            {
                if (card.isVictory)
                {
                    Add(supplyCardPiles, victoryCount, card);
                }
                else
                {
                    Add(supplyCardPiles, card.defaultSupplyCount, card);
                }

                requiresRuins |= card.requiresRuins;
            }

            if (requiresRuins)
            {
                supplyCardPiles.Add(CreateRuins(ruinsCount, random));
            }

            return supplyCardPiles.ToArray();
        }

        public PileOfCards[] GetNonSupplyPiles()
        {
            var nonSupplyCardPiles = new List<PileOfCards>();

            if (this.useShelters)
            {
                Add<CardTypes.Necropolis>(nonSupplyCardPiles, 0);
                Add<CardTypes.OvergrownEstate>(nonSupplyCardPiles, 0);
                Add<CardTypes.Hovel>(nonSupplyCardPiles, 0);
            }

            return nonSupplyCardPiles.ToArray();
        }

        private static PileOfCards CreateRuins(int ruinsCount, Random random)
        {
            int ruinCountPerPile = 10;
            var allRuinsCards = new ListOfCards();
            allRuinsCards.AddNCardsToTop(new CardTypes.AbandonedMine(), ruinCountPerPile);
            allRuinsCards.AddNCardsToTop(new CardTypes.RuinedMarket(), ruinCountPerPile);
            allRuinsCards.AddNCardsToTop(new CardTypes.RuinedLibrary(), ruinCountPerPile);
            allRuinsCards.AddNCardsToTop(new CardTypes.RuinedVillage(), ruinCountPerPile);
            allRuinsCards.AddNCardsToTop(new CardTypes.Survivors(), ruinCountPerPile);

            allRuinsCards.Shuffle(random);

            var result = new PileOfCards(new CardTypes.Ruin());

            for (int i = 0; i < ruinsCount; ++i)
            {
                Card card = allRuinsCards.DrawCardFromTop();
                if (card == null)
                {
                    throw new Exception("Not enough ruins available.");
                }
                result.AddCardToTop(card);
            }
            result.EraseKnownCountKnowledge();

            return result;
        }

        private static void Add<CardType>(List<PileOfCards> cardPiles, int initialCount)
            where CardType : Card, new()
        {

            Add(cardPiles, initialCount, new CardType());
        }

        private static void Add(List<PileOfCards> cardPiles, int initialCount, Card protoType)
        {
            cardPiles.Add(new PileOfCards(protoType, initialCount));
        }
    }
}

using System.Linq;

namespace Dominion.Strategy.Description
{
    public class PickByPriorityDescription
    {
        public readonly CardAcceptanceDescription[] descriptions;

        public PickByPriorityDescription(CardAcceptanceDescription[] descriptions)
        {
            this.descriptions = descriptions;
        }

        public CardPickByPriority ToCardPicker()
        {
            return new CardPickByPriority(this.descriptions.Select(descr => descr.ToCardAcceptance()).ToArray());
        }

        public void Write(System.IO.TextWriter writer)
        {
            writer.WriteLine("Pick By Priority: {");
            foreach (var acceptance in descriptions)
            {
                acceptance.WriteText(writer);
                writer.WriteLine("");
            }
            writer.WriteLine("}");
        }

        public PickByPriorityDescription AddCardInBestLocation(Card card, int count)
        {
            var resultDescriptions = new CardAcceptanceDescription[this.descriptions.Length+1];

            int currentReadLocation = 0;
            int currentWriteLocation = 0;

            while (currentReadLocation < this.descriptions.Length)
            {
                if (this.descriptions[currentReadLocation].card.DefaultCoinCost <= card.DefaultCoinCost &&
                    this.descriptions[currentReadLocation].IsConditionedOnlyOnSelfOwnership())
                    break;
                resultDescriptions[currentWriteLocation++] = this.descriptions[currentReadLocation++];
            }

            resultDescriptions[currentWriteLocation++] = new CardAcceptanceDescription(card, new MatchDescription(CountSource.CountAllOwned, card, Comparison.LessThan, count));

            while (currentReadLocation < this.descriptions.Length)
            {
                resultDescriptions[currentWriteLocation++] = this.descriptions[currentReadLocation++];
            }

            var result = new PickByPriorityDescription(resultDescriptions);
            if (card.potionCost > 0 && !result.HasPotionCard())
            {
                return result.AddCardInBestLocation(Cards.Potion, 1);
            }

            return result;
        }

        private bool HasPotionCard()
        {
            return this.descriptions.Where(descr => descr.card == Cards.Potion).Any();
        }
    }
}

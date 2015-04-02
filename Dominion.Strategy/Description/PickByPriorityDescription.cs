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
            foreach (var acceptance in descriptions)
            {
                acceptance.WriteText(writer);
                writer.Write(", ");
            }
        }

        public PickByPriorityDescription AddCardInBestLocation(Card card)
        {
            var resultDescriptions = new CardAcceptanceDescription[this.descriptions.Length+1];

            int currentReadLocation = 0;
            int currentWriteLocation = 0;

            while (currentReadLocation < this.descriptions.Length)
            {
                if (this.descriptions[currentReadLocation].card.DefaultCoinCost < card.DefaultCoinCost)
                    break;
                resultDescriptions[currentWriteLocation++] = this.descriptions[currentReadLocation++];
            }

            resultDescriptions[currentWriteLocation++] = new CardAcceptanceDescription(card, new MatchDescription());

            while (currentReadLocation < this.descriptions.Length)
            {
                resultDescriptions[currentWriteLocation++] = this.descriptions[currentReadLocation++];
            }

            var result = new PickByPriorityDescription(resultDescriptions);
            if (card.potionCost > 0 && !result.CanPurchasePotion())
            {
                return result.AddCardInBestLocation(Cards.Potion);
            }

            return result;
        }

        private bool CanPurchasePotion()
        {
            return this.descriptions.Where(descr => descr.card.potionCost > 0).Any();
        }
    }
}

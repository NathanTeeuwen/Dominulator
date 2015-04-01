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
    }
}

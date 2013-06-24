using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public class CardPickConcatenator
        : ICardPicker
    {
        private readonly ICardPicker[] matchers;

        public CardPickConcatenator(params ICardPicker[] matchers)
        {
            this.matchers = matchers;
        }

        public Type GetPreferredCard(GameState gameState, CardPredicate cardPredicate)
        {
            foreach (ICardPicker matcher in this.matchers)
            {
                Type result = matcher.GetPreferredCard(gameState, cardPredicate);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public IEnumerable<Card> GetNeededCards()
        {
            foreach (ICardPicker matcher in this.matchers)
            {
                foreach (Card card in matcher.GetNeededCards())
                {
                    yield return card;
                }
            }
        }
    }
}

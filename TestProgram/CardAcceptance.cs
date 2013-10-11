using Dominion;

namespace Program
{  
    public class CardAcceptance
    {
        internal readonly Card card;
        internal readonly GameStatePredicate match;
        internal readonly GameStateIntValue overpayAmount;

        public static int OverPayZero(GameState gameState)
        {
            return 0;
        }

        public static int OverPayMaxAmount(GameState gameState)
        {
            return int.MaxValue;
        }

        public static bool AlwaysMatch(GameState gameState)
        {
            return true;
        }

        public CardAcceptance(Card card)
        {
            this.card = card;
            this.match = AlwaysMatch;
            this.overpayAmount = OverPayZero;
        }

        public CardAcceptance(Card card, GameStatePredicate match)
        {
            this.card = card;
            this.match = match;
            this.overpayAmount = OverPayZero;
        }

        public CardAcceptance(Card card, GameStatePredicate match, GameStateIntValue overpayAmount)
        {
            this.card = card;
            this.match = match;
            this.overpayAmount = overpayAmount;
        }

        public static CardAcceptance For(Card card)
        {
            return new CardAcceptance(card);
        }

        public static CardAcceptance For(Card card, GameStatePredicate match)
        {
            return new CardAcceptance(card, match);
        }

        public static CardAcceptance For<T>()
            where T : Card, new()
        {
            return new CardAcceptance(Card.Type<T>());
        }

        public static CardAcceptance For<T>(GameStatePredicate match)
            where T : Card, new()
        {
            return new CardAcceptance(Card.Type<T>(), match);
        }

        public static CardAcceptance For<T>(GameStatePredicate match, GameStateIntValue overpayAmount)
            where T : Card, new()
        {
            return new CardAcceptance(Card.Type<T>(), match, overpayAmount);
        }

        public static CardAcceptance For<T>(int threshhold)
            where T : Card, new()
        {
            return For<T>(CountSource.AllOwned, Comparison.LessThan, threshhold);
        }

        public static CardAcceptance For<T>(int threshhold, GameStatePredicate match)
            where T : Card, new()
        {
            return For<T>(CountSource.AllOwned, Comparison.LessThan, threshhold, match);
        }

        public static CardAcceptance For<T>(CountSource countSource, Comparison comparison, int threshhold)
           where T : Card, new()
        {
            MatchDescription descr = new MatchDescription(Card.Type<T>(), countSource, comparison, threshhold);

            return descr.ToCardAcceptance();            
        }

        public static CardAcceptance For<T>(CountSource countSource, Comparison comparison, int threshhold, GameStatePredicate match)
           where T : Card, new()
        {
            MatchDescription descr = new MatchDescription(Card.Type<T>(), countSource, comparison, threshhold);

            return CardAcceptance.For(Card.Type<T>(), gameState => descr.GameStatePredicate(gameState) && match(gameState));
        } 
    }
}

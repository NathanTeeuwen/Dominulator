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

        public static GameStateIntValue DefaultOverpayAmount = OverPayMaxAmount;

        public CardAcceptance(Card card)
        {
            this.card = card;
            this.match = AlwaysMatch;
            this.overpayAmount = DefaultOverpayAmount;
        }

        public CardAcceptance(Card card, GameStatePredicate match)
        {
            this.card = card;
            this.match = match;
            this.overpayAmount = DefaultOverpayAmount;
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

        public static CardAcceptance For(Card card, GameStatePredicate match, GameStateIntValue overpayAmount)
        {
            return new CardAcceptance(card, match, overpayAmount);
        }

        public static CardAcceptance For(Card card, int threshhold)
        {
            return For(card, CountSource.AllOwned, Comparison.LessThan, threshhold);
        }

        public static CardAcceptance For(Card card, int threshhold, GameStatePredicate match)
        {
            return For(card, CountSource.AllOwned, Comparison.LessThan, threshhold, match);
        }

        public static CardAcceptance For(Card card, CountSource countSource, Comparison comparison, int threshhold)
        {
            MatchDescription descr = new MatchDescription(card, countSource, comparison, threshhold);

            return descr.ToCardAcceptance();            
        }

        public static CardAcceptance For(Card card, CountSource countSource, Comparison comparison, int threshhold, GameStatePredicate match)
        {
            MatchDescription descr = new MatchDescription(card, countSource, comparison, threshhold);

            return CardAcceptance.For(card, gameState => descr.GameStatePredicate(gameState) && match(gameState));
        } 
    }
}

using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public enum CountSource
    {
        Pile,
        AllOwned,
        InHand,
        None,
    }

    public enum Comparison
    {
        LessThan,
        GreaterThan,
        None,
    }

    public class MatchDescription
    {
        // e.g. countAllOwned<Province>(gameState) < 6
        internal readonly Type cardType;
        internal readonly CountSource countSource;
        internal readonly Comparison comparison;
        internal int countThreshHold;

        public MatchDescription(Type cardType, CountSource countSource, Comparison comparison, int threshhold)
        {
            this.cardType = cardType;
            this.countSource = countSource;
            this.comparison = comparison;
            this.countThreshHold = threshhold;
        }

        public MatchDescription Clone()
        {
            return new MatchDescription(this.cardType, this.countSource, this.comparison, this.countThreshHold);
        }

        public bool GameStatePredicate(GameState gameState)
        {
            int countOfTheSource;

            switch (countSource)
            {                            
                case CountSource.Pile:
                    countOfTheSource = Strategies.CountOfPile(this.cardType, gameState);
                    break;
                case CountSource.AllOwned:
                    countOfTheSource = Strategies.CountAllOwned(this.cardType, gameState);
                    break;
                case CountSource.InHand:
                    countOfTheSource = Strategies.CountInHand(this.cardType, gameState);
                    break;
                case CountSource.None:
                    return true;
                default:
                    throw new Exception("Unhandled source case");
            }

            switch (this.comparison)
            {
                case Comparison.GreaterThan:
                {
                    return countOfTheSource > this.countThreshHold;
                }
                case Comparison.LessThan:
                {
                    return countOfTheSource < this.countThreshHold;
                }
                default:
                throw new Exception("Unhandled comparison case");
            }
        }

        public void WriteText(System.IO.TextWriter writer)
        {
            if (this.countThreshHold > 0)
            {
                writer.Write("({0})", this.countThreshHold);
            }
        }
    }    

    public class CardAcceptanceDescription
    {
        internal readonly Card card;
        internal readonly MatchDescription[] matchDescriptions;

        public CardAcceptanceDescription(Card card, MatchDescription[] matchDescriptions)
        {
            this.card = card;
            this.matchDescriptions = matchDescriptions;
        }

        public bool GameStatePredicate(GameState gameState)
        {
            foreach (MatchDescription description in this.matchDescriptions)
            {
                if (!description.GameStatePredicate(gameState))
                    return false;
            }
            return true;
        }

        public CardAcceptance ToCardAcceptance()
        {
            return new CardAcceptance(this.card, this.GameStatePredicate);
        }

        public CardAcceptanceDescription Clone()
        {
            return new CardAcceptanceDescription(this.card, matchDescriptions.Select(m => m.Clone()).ToArray());
        }

        public void WriteText(System.IO.TextWriter writer)
        {
            writer.Write(card.name);            
            this.matchDescriptions[0].WriteText(writer);
        }
    }

    class PickByPriorityDescription
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

    public struct CardAcceptance
    {
        internal Card card;
        internal GameStatePredicate match;

        public CardAcceptance(Card card)
        {
            this.card = card;
            this.match = gameState => true;
        }

        public CardAcceptance(Card card, GameStatePredicate match)
        {
            this.card = card;
            this.match = match;
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
            return new CardAcceptance(new T());
        }

        public static CardAcceptance For<T>(GameStatePredicate match)
            where T : Card, new()
        {
            return new CardAcceptance(new T(), match);
        }
    }
}

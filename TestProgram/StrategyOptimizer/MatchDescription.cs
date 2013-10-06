using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public class MatchDescription
    {
        // e.g. countAllOwned<Province>(gameState) < 6
        internal readonly Card cardType;
        internal readonly CountSource countSource;
        internal readonly Comparison comparison;
        internal int countThreshHold;

        public MatchDescription(Card cardType, CountSource countSource, Comparison comparison, int threshhold)
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
}
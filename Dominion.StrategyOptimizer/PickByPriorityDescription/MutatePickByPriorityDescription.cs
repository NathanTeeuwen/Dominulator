using Dominion;
using Dominion.Strategy;
using Dominion.Strategy.Description;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program.GeneticAlgorithm;

namespace Program
{
    class MutatePickByPriorityDescription
           : ISpecidesMutator<PickByPriorityDescription>
    {
        private delegate bool ApplyMutation(List<CardAcceptanceDescription> descripton);

        private readonly ApplyMutation[] mutators;
        private readonly Random random;
        private readonly Card[] availableCards;

        public MutatePickByPriorityDescription(Random random, Card[] availableCards)
        {
            this.random = random;
            this.availableCards = availableCards;
            this.mutators = CreateMutators();
        }

        public PickByPriorityDescription Mutate(PickByPriorityDescription member)
        {
            List<CardAcceptanceDescription> descriptions = new List<CardAcceptanceDescription>(member.descriptions);

            bool applied = false;
            while (!applied)
            {
                var mutator = this.mutators[this.random.Next(this.mutators.Length)];
                applied = mutator(descriptions);
            }

            return new PickByPriorityDescription(descriptions.ToArray());
        }

        ApplyMutation[] CreateMutators()
        {
            return new ApplyMutation[]
                {
                    this.ApplyAddNewCardAcceptance,
                    this.ApplyRemoveCardAcceptance,
                    this.ApplyModifyCardAcceptanceCount,
                    this.ApplySwapOrderCardAcceptance,
                    //this.ApplyAddNewUniqueCardAcceptance
                };
        }

        private Card PickRandomCardFromSupply(Card[] excluded)
        {
            for (int i = 0; i < 3; ++i)
            {
                Card result = this.availableCards[this.random.Next(this.availableCards.Length)];
                if (!DoesSetInclude(excluded, result))
                    return result;
            }

            return null;
        }

        private bool DoesSetInclude(Card[] cards, Card test)
        {
            foreach (Card card in cards)
            {
                if (card.Equals(test))
                    return true;
            }

            return false;
        }

        private bool ApplyAddNewUniqueCardAcceptance(List<CardAcceptanceDescription> descriptions)
        {
            Card card = this.PickRandomCardFromSupply(descriptions.Select(descr => descr.card).ToArray());
            if (card == null)
                return false;
            int insertLocation = FindLocationByCost(descriptions, card);
            descriptions.Insert(insertLocation, new CardAcceptanceDescription(card, CountSource.CountAllOwned, null, Comparison.LessThan, 10));

            return true;
        }

        private bool ApplyAddNewCardAcceptance(List<CardAcceptanceDescription> descriptions)
        {
            Card card = this.PickRandomCardFromSupply(new Card[0] { });
            if (card == null)
                return false;

            int insertLocation = FindLocationByCost(descriptions, card);

            if (this.random.Next(2) == 0)
            {
                insertLocation += this.random.Next(2) + 2;
            }
            else
            {
                insertLocation -= this.random.Next(2) + 2;
            }
            //int insertLocation = this.random.Next(descriptions.Count());

            insertLocation = Math.Max(0, insertLocation);
            insertLocation = Math.Min(descriptions.Count, insertLocation);

            if (insertLocation > 0 && descriptions[insertLocation - 1].card.Equals(card))
                return false;

            if (insertLocation < descriptions.Count - 1 && descriptions[insertLocation + 1].card.Equals(card))
                return false;

            descriptions.Insert(insertLocation, new CardAcceptanceDescription(card, CountSource.CountAllOwned, null, Comparison.LessThan, 1));

            return true;
        }

        private int FindLocationByCost(List<CardAcceptanceDescription> descriptions, Card card)
        {
            int insertLocation = 0;

            while (insertLocation < descriptions.Count)
            {
                if (descriptions[insertLocation].matchDescriptions[0].countSource != CountSource.Always ||
                    descriptions[insertLocation].card.DefaultCoinCost > card.DefaultCoinCost)
                {
                    insertLocation++;
                }
                else
                    break;
            }

            return insertLocation;
        }

        private bool ApplyRemoveCardAcceptance(List<CardAcceptanceDescription> descriptions)
        {
            if (descriptions.Count <= 3)
                return false;

            int removeLocation = this.random.Next(descriptions.Count);

            if (descriptions[removeLocation].matchDescriptions[0].countSource == CountSource.Always)
                return false;

            descriptions.RemoveAt(removeLocation);
            return true;
        }

        private bool ApplyModifyCardAcceptanceCount(List<CardAcceptanceDescription> descriptions)
        {
            int descriptionIndex = this.random.Next(descriptions.Count);

            var description = descriptions[descriptionIndex];

            int threshhold = description.matchDescriptions[0].countThreshHold;

            if (threshhold == 0)
            {
                return false;
            }


            bool shouldIncrement = this.random.Next(2) == 0 || threshhold == 1;

            if (shouldIncrement)
            {
                threshhold++;
            }
            else
            {
                threshhold--;
            }

            var newDescription = description.Clone();
            newDescription.matchDescriptions[0].countThreshHold = threshhold;
            descriptions[descriptionIndex] = newDescription;

            return true;
        }

        private bool ApplySwapOrderCardAcceptance(List<CardAcceptanceDescription> descriptions)
        {
            if (descriptions.Count <= 1)
                return false;

            int swapFirstIndex = this.random.Next(descriptions.Count - 1);
            int nextSwapIndex = swapFirstIndex + 1;

            var temp = descriptions[swapFirstIndex];
            descriptions[swapFirstIndex] = descriptions[nextSwapIndex];
            descriptions[nextSwapIndex] = temp;

            return true;
        }
    }
}
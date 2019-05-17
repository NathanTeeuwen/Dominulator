using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class GameConfigBuilder
    {
        public bool useShelters;
        public bool useColonyAndPlatinum;
        private List<Card> kingdomPiles;
        private List<Event> events;
        private List<Landmark> landmarks;
        private Card baneCard;
        private MapPlayerGameConfigToCardSet startingDeck;
        private MapPlayerGameConfigToCardSet shuffleLuck;

        public static GameConfig Create(StartingCardSplit split, params Card[] cards)
        {
            var result = new GameConfigBuilder();
            result.kingdomPiles.AddRange(cards);
            result.CardSplit = split;
            return result.ToGameConfig();
        }

        public static GameConfig Create(params Card[] cards)
        {
            var result = new GameConfigBuilder();
            result.kingdomPiles.AddRange(cards);
            result.events = new List<Event>();
            result.landmarks = new List<Landmark>();
            return result.ToGameConfig();
        }

        public GameConfigBuilder()
        {
            this.useShelters = false;
            this.useColonyAndPlatinum = false;
            this.kingdomPiles = new List<Card>();
            this.events = new List<Event>();
            this.landmarks = new List<Landmark>();
            this.startingDeck = GetDefaultStartingDeck;
            this.shuffleLuck = GetDefaultStartingHand;
        }

        public GameConfigBuilder(GameConfig gameConfig)
        {
            this.useShelters = gameConfig.useShelters;
            this.useColonyAndPlatinum = gameConfig.useColonyAndPlatinum;
            this.baneCard = gameConfig.baneCard;
            this.kingdomPiles = new List<Card>(gameConfig.kingdomPiles);
            this.events = new List<Event>(gameConfig.gameDescription.events);
            this.landmarks = new List<Landmark>(gameConfig.gameDescription.landmarks);
            this.startingDeck = gameConfig.startingDeck;
            this.shuffleLuck = gameConfig.startingHand;
        }

        public void SetStartingDeck(IEnumerable<CardCountPair> startingCards)
        {
            this.startingDeck = GetCardSetSameForAllPlayers(startingCards);
        }

        public void SetStartingDeckPerPlayer(IEnumerable<CardCountPair>[] cardPerPlayer)
        {
            this.startingDeck = GetCardSetFromArray(cardPerPlayer);
        }

        public void SetShuffleLuckPerPlayer(IEnumerable<CardCountPair>[] cardPerPlayer)
        {
            this.shuffleLuck = GetCardSetFromArray(cardPerPlayer);
        }

        public void SetBaneCard(Card card)
        {
            this.baneCard = card;
        }

        public StartingCardSplit CardSplit
        {
            set
            {
                this.shuffleLuck = GetStartingHandForSplit(value);
            }
        }

        public void SetCardSplitPerPlayer(StartingCardSplit[] splits)
        {
            this.shuffleLuck = GetStartingHandForSplit(splits);
        }

        public void SetEvents(IEnumerable<Event> cards)
        {
            this.events.Clear();
            this.events.AddRange(cards);
        }

        public void SetKingdomPiles(IEnumerable<Card> cards)
        {
            var setCards = new HashSet<Card>();

            foreach (Card card in cards)
            {
                setCards.Add(card);
            }
            KeepOnlyKingdomCard(setCards);

            this.kingdomPiles.Clear();
            this.kingdomPiles.AddRange(setCards);
        }

        public static void KeepOnlyKingdomCard(HashSet<Card> setCards)
        {
            var cardsToRemove = new List<Card>();
            foreach (Card card in setCards)
            {
                if (!card.isKingdomCard)
                    cardsToRemove.Add(card);
            }

            foreach (var card in cardsToRemove)
            {
                setCards.Remove(card);
            }
        }

        public GameConfig ToGameConfig()
        {
            return new GameConfig(
                this.ToGameDescription(),
                this.startingDeck,
                this.shuffleLuck);
        }

        public GameDescription ToGameDescription()
        {
            return new GameDescription(
                    this.kingdomPiles.ToArray(),
                    this.events.ToArray(),
                    this.landmarks.ToArray(),
                    this.baneCard,
                    this.useShelters,
                    this.useColonyAndPlatinum);
        }

        static readonly CardCountPair[] ShelterStartingDeck = new CardCountPair[] {
                            new CardCountPair(Cards.Copper, 7),
                            new CardCountPair(Cards.Hovel, 1),
                            new CardCountPair(Cards.Necropolis, 1),
                            new CardCountPair(Cards.OvergrownEstate, 1)
                        };

        static readonly CardCountPair[] EstateStartingDeck = new CardCountPair[] {
                            new CardCountPair(Cards.Copper, 7),
                            new CardCountPair(Cards.Estate, 3)
                        };

        static readonly CardCountPair[] Starting52Split = new CardCountPair[] {
                            new CardCountPair(Cards.Copper, 5)
                        };

        static readonly CardCountPair[] Starting25Split = new CardCountPair[] {
                            new CardCountPair(Cards.Copper, 2),
                            new CardCountPair(Cards.Estate, 3)
                        };
        static readonly CardCountPair[] Starting25SplitShelter = new CardCountPair[] {
                            new CardCountPair(Cards.Copper, 2),
                            new CardCountPair(Cards.Necropolis, 1),
                            new CardCountPair(Cards.Hovel, 1),
                            new CardCountPair(Cards.OvergrownEstate, 1)
                        };

        static readonly CardCountPair[] Starting43SplitEstate = new CardCountPair[] {
                            new CardCountPair(Cards.Copper, 4),
                            new CardCountPair(Cards.Estate, 1)
                        };

        static readonly CardCountPair[] Starting43SplitShelter = new CardCountPair[] {
                            new CardCountPair(Cards.Copper, 4),
                            new CardCountPair(Cards.Necropolis, 1)
                        };

        static readonly CardCountPair[] Starting34SplitEstate = new CardCountPair[] {
                            new CardCountPair(Cards.Copper, 4),
                            new CardCountPair(Cards.Estate, 1)
                        };

        static readonly CardCountPair[] Starting34SplitShelter = new CardCountPair[] {
                            new CardCountPair(Cards.Copper, 3),
                            new CardCountPair(Cards.OvergrownEstate, 1),
                            new CardCountPair(Cards.Hovel, 1)
                        };

        static IEnumerable<CardCountPair> GetDefaultStartingDeck(int playerPosition, GameConfig gameConfig)
        {
            return gameConfig.useShelters ? ShelterStartingDeck : EstateStartingDeck;
        }

        static IEnumerable<CardCountPair> GetDefaultStartingHand(int playerPosition, GameConfig gameConfig)
        {
            return null;
        }

        static MapPlayerGameConfigToCardSet GetStartingHandForSplit(StartingCardSplit split)
        {
            return delegate(int playerIndex, GameConfig gameConfig)
            {
                return GetCardsForSplit(split, gameConfig);
            };
        }

        static MapPlayerGameConfigToCardSet GetStartingHandForSplit(StartingCardSplit[] splits)
        {
            return delegate(int playerIndex, GameConfig gameConfig)
            {
                return GetCardsForSplit(splits[playerIndex], gameConfig);
            };
        }

        private static CardCountPair[] GetCardsForSplit(StartingCardSplit split, GameConfig gameConfig)
        {
            switch (split)
            {
                case StartingCardSplit.Random: return null;
                case StartingCardSplit.Split52: return Starting52Split;
                case StartingCardSplit.Split43: return gameConfig.useShelters ? Starting43SplitShelter : Starting43SplitEstate;
                case StartingCardSplit.Split25: return gameConfig.useShelters ? Starting25SplitShelter : Starting25Split;
                case StartingCardSplit.Split34: return gameConfig.useShelters ? Starting34SplitShelter : Starting34SplitEstate;

                default:
                    throw new Exception();
            }
        }

        static MapPlayerGameConfigToCardSet GetCardSetSameForAllPlayers(IEnumerable<CardCountPair> cards)
        {
            return delegate(int playerIndex, GameConfig gameConfig)
            {
                return cards;
            };
        }

        static MapPlayerGameConfigToCardSet GetCardSetFromArray(IEnumerable<CardCountPair>[] cards)
        {
            return delegate(int playerIndex, GameConfig gameConfig)
            {
                return cards[playerIndex];
            };
        }

        public void GenerateCompletelyRandomKingdom(
            IEnumerable<Card> allCards,
            IEnumerable<Event> allEvents,
            IEnumerable<Landmark> allLandMarks,
            Random random)
        {
            var cardPicker = new UniqueCardPicker<Card>(allCards, random);

            this.RandomizeKingdom(allCards, random);
            this.RandomizeEvents(random);
            this.ReRollPlatinumColony(random);
            this.ReRollShelter(random);
        }

        public void RandomizeKingdom(IEnumerable<Card> allCards, Random random)
        {
            this.kingdomPiles.Clear();
            var cardPicker = new UniqueCardPicker<Card>(allCards, random);
            PopulateCardListToCount(10, this.kingdomPiles, cardPicker, c => c.isKingdomCard);

            if (this.kingdomPiles.Contains(Cards.YoungWitch))
            {
                this.baneCard = cardPicker.GetCard(c => c.isKingdomCard && (c.DefaultCoinCost == 2 || c.DefaultCoinCost == 3));
            }
            else
                this.baneCard = null;
        }

        public void RandomizeEvents(Random random)
        {
            int cEventsToInclude = 0;
            int cLandmarksToInclude = 0;

            var allEventsCards = Dominion.Cards.AllCardsList.Where(c => c is Event).Select(c=> (Event)c).ToArray();
            var allLandmarkCards = Dominion.Cards.AllCardsList.Where(c => c is Landmark).Select(c => (Landmark)c).ToArray();

            int cEventRemaining = allEventsCards.Length;
            int totalKingdomCount = Dominion.Cards.AllKingdomCardsList.Length;
            for (int i = 0; i < 10; ++i)
            {
                int roll = random.Next(totalKingdomCount);
                if (roll <= cEventRemaining)
                {
                    cEventsToInclude++;
                    cEventRemaining--;
                    i--;
                }
                else
                {
                    totalKingdomCount--;
                }
            }

            var cardPicker = new Dominion.UniqueCardPicker<Event>(allEventsCards, random);
            this.events.Clear();
            PopulateCardListToCount<Event>(cEventsToInclude, this.events, cardPicker, c => true);

            var cardPicker2 = new Dominion.UniqueCardPicker<Landmark>(allLandmarkCards, random);
            this.landmarks.Clear();
            PopulateCardListToCount<Landmark>(cLandmarksToInclude, this.landmarks, cardPicker2, c => true);
        }

        public void ReRollPlatinumColony(Random random)
        {
            this.useColonyAndPlatinum = ShouldIncludeExpansion(Dominion.Expansion.Prosperity, random);
        }

        public void ReRollShelter(Random random)
        {
            this.useShelters = ShouldIncludeExpansion(Dominion.Expansion.DarkAges, random);
        }

        private bool ShouldIncludeExpansion(Dominion.Expansion expansion, Random random)
        {
            int cExpansion = this.kingdomPiles.Where(c => c.expansion == expansion).Count();
            int roll = random.Next(1, 10);
            return cExpansion >= roll ? true : false;
        }

        private static void PopulateCardListToCount<T>(int targetCount, List<T> list, UniqueCardPicker<T> cardPicker, Func<T, bool> meetConstraint)
            where T : CardShapedObject
        {
            while (list.Count < targetCount)
            {
                T currentCard = cardPicker.GetCard(meetConstraint);
                if (currentCard == null)
                    break;
                list.Add(currentCard);
            }
        }
    }
}

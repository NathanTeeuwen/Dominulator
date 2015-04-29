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
        private Card[] kingdomPiles;
        private Card[] events;
        private Card baneCard;
        private MapPlayerGameConfigToCardSet startingDeck;
        private MapPlayerGameConfigToCardSet shuffleLuck;

        public static GameConfig Create(StartingCardSplit split, params Card[] cards)
        {
            var result = new GameConfigBuilder();
            result.kingdomPiles = cards;
            result.CardSplit = split;
            return result.ToGameConfig();
        }

        public static GameConfig Create(params Card[] cards)
        {
            var result = new GameConfigBuilder();
            result.kingdomPiles = cards;
            return result.ToGameConfig();
        }

        public GameConfigBuilder()
        {
            this.useShelters = false;
            this.useColonyAndPlatinum = false;
            this.kingdomPiles = null;
            this.startingDeck = GetDefaultStartingDeck;
            this.shuffleLuck = GetDefaultStartingHand;
        }

        public GameConfigBuilder(GameConfig gameConfig)
        {
            this.useShelters = gameConfig.useShelters;
            this.useColonyAndPlatinum = gameConfig.useColonyAndPlatinum;
            this.kingdomPiles = gameConfig.kingdomPiles;
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

        public void SetEvents(IEnumerable<Card> cards)
        {
            var setCards = new HashSet<Card>();

            foreach (Card card in cards)
            {
                setCards.Add(card);
            }
            KeepOnlyKingdomCard(setCards);

            this.events = setCards.ToArray();
        }

        public void SetKingdomPiles(IEnumerable<Card> cards)
        {
            var setCards = new HashSet<Card>();

            foreach (Card card in cards)
            {
                setCards.Add(card);
            }
            KeepOnlyKingdomCard(setCards);

            this.kingdomPiles = setCards.ToArray();
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
                new GameDescription(
                    this.kingdomPiles,
                    this.events,
                    this.baneCard,
                    this.useShelters,
                    this.useColonyAndPlatinum),
                this.startingDeck,
                this.shuffleLuck);
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
    }
}

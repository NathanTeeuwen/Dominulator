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
        private MapPlayerGameConfigToCardSet startingDeck;
        private MapPlayerGameConfigToCardSet startingHand;        

        public static GameConfig CreateFromWithPlayPositionsSwapped(GameConfig gameConfig)
        {
            var result = new GameConfigBuilder(gameConfig);
            result.SwapPlayerOneAndTwo();
            return result.ToGameConfig();
        }

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
            this.startingHand = GetDefaultStartingHand;
        }

        public GameConfigBuilder(GameConfig gameConfig)
        {
            this.useShelters = gameConfig.useShelters;
            this.useColonyAndPlatinum = gameConfig.useColonyAndPlatinum;
            this.kingdomPiles = gameConfig.kingdomPiles;
            this.startingDeck = gameConfig.startingDeck;
            this.startingHand = gameConfig.startingHand;
        }

        public void SetStartingDeck(IEnumerable<CardCountPair> startingCards)
        {
            this.startingDeck = GetCardSetSameForAllPlayers(startingCards);
        }

        public void SetStartingDeckPerPlayer(IEnumerable<CardCountPair>[] cardPerPlayer)
        {
            this.startingDeck = GetCardSetFromArray(cardPerPlayer);
        }

        public void SetKingdomPiles(IEnumerable<Card> cards)
        {
            var setCards = new HashSet<Card>();
            
            foreach(Card card in cards)
            {
                setCards.Add(card);
            }

            foreach (Card card in cardsIncludedbyDefault)
            {
                setCards.Remove(card);
            }

            this.kingdomPiles = setCards.ToArray();
        }

        static readonly Card[] cardsIncludedbyDefault = new Card[] { 
                // treaures
                Cards.Platinum,
                Cards.Gold,
                Cards.Silver,
                Cards.Copper,
                Cards.Colony,
                Cards.Potion,
                // victory
                Cards.Province,
                Cards.Duchy,
                Cards.Estate,
                Cards.Curse,                
                // ruins
                Cards.AbandonedMine,
                Cards.RuinedLibrary,
                Cards.RuinedVillage,
                Cards.RuinedMarket,
                Cards.Survivors,
                // shelters
                Cards.OvergrownEstate,
                Cards.Hovel,
                Cards.Necropolis,
                // non-supply piles
                Cards.Spoils,                
                Cards.Madman,
                Cards.Mercenary,
                // tournament prizes
                Cards.BagOfGold, 
                Cards.Diadem, 
                Cards.Followers, 
                Cards.Princess, 
                Cards.TrustySteed,
                // base classes that should not be used
                Cards.Ruins,
                Cards.Prize
            };

        public StartingCardSplit CardSplit
        {
            set
            {
                this.startingHand = GetStartingHandForSplit(value);
            }
        }

        public GameConfig ToGameConfig()
        {
            return new GameConfig(this.kingdomPiles, this.useShelters, this.useColonyAndPlatinum, this.startingDeck, this.startingHand);
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

        static readonly CardCountPair[] Starting43SplitEstate = new CardCountPair[] {
                            new CardCountPair(Cards.Copper, 4),
                            new CardCountPair(Cards.Estate, 1)
                        };

        static readonly CardCountPair[] Starting43SplitShelter = new CardCountPair[] {
                            new CardCountPair(Cards.Copper, 4),
                            new CardCountPair(Cards.Necropolis, 1)
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
                switch (split)
                {
                    case StartingCardSplit.Random: return null;
                    case StartingCardSplit.Split52: return Starting52Split;
                    case StartingCardSplit.Split43: return gameConfig.useShelters ? Starting43SplitShelter : Starting43SplitEstate;
                    default:
                        throw new Exception();
                }
            };
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

        static MapPlayerGameConfigToCardSet SwapPlayersInSet(MapPlayerGameConfigToCardSet original)
        {
            return delegate(int playerIndex, GameConfig gameConfig)
            {
                if (playerIndex == 0)
                {
                    return original(1, gameConfig);
                }
                else if (playerIndex == 1)
                {
                    return original(0, gameConfig);
                }
                else
                    throw new Exception("Only expected 2 players");
            };
        }

        private void SwapPlayerOneAndTwo()
        {
            this.startingDeck = SwapPlayersInSet(this.startingDeck);
            this.startingHand = SwapPlayersInSet(this.startingHand);
        }
    }


    public class GameConfig
    {
        public readonly bool useShelters;
        public readonly bool useColonyAndPlatinum;
        public readonly Card[] kingdomPiles;
        public readonly MapPlayerGameConfigToCardSet startingDeck;
        public readonly MapPlayerGameConfigToCardSet startingHand;
        public readonly CardGameSubset cardGameSubset;

        public GameConfig(
            Card[] supplyPiles, 
            bool useShelters, 
            bool useColonyAndPlatinum,            
            MapPlayerGameConfigToCardSet startingDecks = null,
            MapPlayerGameConfigToCardSet startingHands = null)
        {
            this.useShelters = useShelters;
            this.useColonyAndPlatinum = useColonyAndPlatinum;
            this.kingdomPiles = supplyPiles;
            this.startingDeck = startingDecks; 
            this.startingHand = startingHands;
            this.cardGameSubset = new CardGameSubset();

            GetSupplyPiles(1, null, this.cardGameSubset);
            GetNonSupplyPiles(this.cardGameSubset);            

            this.cardGameSubset.isInitializing = false;
        }      

        public IEnumerable<CardCountPair> StartingHand(int playerIndex)
        {
            return this.startingHand(playerIndex, this);
        }

        public IEnumerable<CardCountPair> StartingDeck(int playerIndex)
        {
            return this.startingDeck(playerIndex, this);
        }

        public PileOfCards[] GetSupplyPiles(int playerCount, Random random)
        {
            return GetSupplyPiles(playerCount, random, this.cardGameSubset);
        }                   

        private PileOfCards[] GetSupplyPiles(int playerCount, Random random, CardGameSubset gameSubset)
        {
            var supplyCardPiles = new List<PileOfCards>(capacity: 20);

            int curseCount = (playerCount - 1) * 10;
            int ruinsCount = curseCount;
            int victoryCount = (playerCount == 2) ? 8 : 12;

            // cards always in the supply
            Add(gameSubset, supplyCardPiles, 60, Cards.Copper);
            Add(gameSubset, supplyCardPiles, 40, Cards.Silver);
            Add(gameSubset, supplyCardPiles, 30, Cards.Gold);
            Add(gameSubset, supplyCardPiles, curseCount, Cards.Curse);
            Add(gameSubset, supplyCardPiles, victoryCount + (!this.useShelters ? playerCount * 3 : 0), Cards.Estate);
            Add(gameSubset, supplyCardPiles, victoryCount, Cards.Duchy);
            Add(gameSubset, supplyCardPiles, victoryCount, Cards.Province);
            
            if (this.useColonyAndPlatinum)
            {
                Add(gameSubset, supplyCardPiles, victoryCount, Cards.Colony);
                Add(gameSubset, supplyCardPiles, 20, Cards.Platinum);
            }

            if (this.kingdomPiles.Where(card => card.potionCost != 0).Any())
            {
                Add(gameSubset, supplyCardPiles, 16, Cards.Potion);
            }            

            if (this.kingdomPiles.Where(card => card.requiresRuins).Any())
            {
                supplyCardPiles.Add(CreateRuins(gameSubset, ruinsCount, random));
            }           

            foreach (Card card in this.kingdomPiles)
            {
                if (card.isVictory)
                {
                    Add(gameSubset, supplyCardPiles, victoryCount, card);
                }
                else
                {
                    Add(gameSubset, supplyCardPiles, card.defaultSupplyCount, card);
                }                
            }
            
            return supplyCardPiles.ToArray();
        }

        public PileOfCards[] GetNonSupplyPiles()
        {
            return GetNonSupplyPiles(this.cardGameSubset);
        }

        private PileOfCards[] GetNonSupplyPiles(CardGameSubset gameSubset)
        {
            var nonSupplyCardPiles = new List<PileOfCards>();

            if (this.useShelters)
            {
                gameSubset.AddCard(Cards.Necropolis);
                gameSubset.AddCard(Cards.Hovel);
                gameSubset.AddCard(Cards.OvergrownEstate);
            }

            if (this.kingdomPiles.Where(card => card.requiresSpoils).Any())
            {
                Add(gameSubset, nonSupplyCardPiles, 16, Cards.Spoils);
            }

            if (this.kingdomPiles.Where(card => card == Cards.Hermit).Any())
            {
                Add(gameSubset, nonSupplyCardPiles, 10, Cards.Madman);
            }

            if (this.kingdomPiles.Where(card => card == Cards.Urchin).Any())
            {
                Add(gameSubset, nonSupplyCardPiles, 10, Cards.Mercenary);
            }

            if (this.kingdomPiles.Where(card => card == Cards.Tournament).Any())
            {
                Add(gameSubset, nonSupplyCardPiles, 10, Cards.Mercenary);
            }            

            return nonSupplyCardPiles.ToArray();
        }

        private static PileOfCards CreateTournamentPrizes(CardGameSubset gameSubset)
        {
            Card[] prizes = { Cards.BagOfGold, Cards.Diadem, Cards.Followers, Cards.Princess, Cards.TrustySteed }; 

            if (gameSubset.isInitializing)
            {
                gameSubset.AddCard(Cards.Prize);
                foreach (Card prize in prizes)
                {
                    gameSubset.AddCard(prize);
                }
                return null;
            }
            else
            {
                var result = new PileOfCards(gameSubset, Cards.Prize);
                foreach (Card prize in prizes)
                {
                    result.AddCardToTop(prize);
                }

                return result;
            }            
        }

        private static PileOfCards CreateRuins(CardGameSubset gameSubset, int ruinsCount, Random random)
        {
            if (gameSubset.isInitializing)
            {
                gameSubset.AddCard(Cards.Ruins);
                gameSubset.AddCard(Cards.AbandonedMine);
                gameSubset.AddCard(Cards.RuinedMarket);
                gameSubset.AddCard(Cards.RuinedLibrary);
                gameSubset.AddCard(Cards.RuinedVillage);
                gameSubset.AddCard(Cards.Survivors);
                return null;
            }
            else
            {
                int ruinCountPerPile = 10;
                var allRuinsCards = new ListOfCards(gameSubset);
                allRuinsCards.AddNCardsToTop(Cards.AbandonedMine, ruinCountPerPile);
                allRuinsCards.AddNCardsToTop(Cards.RuinedMarket, ruinCountPerPile);
                allRuinsCards.AddNCardsToTop(Cards.RuinedLibrary, ruinCountPerPile);
                allRuinsCards.AddNCardsToTop(Cards.RuinedVillage, ruinCountPerPile);
                allRuinsCards.AddNCardsToTop(Cards.Survivors, ruinCountPerPile);

                allRuinsCards.Shuffle(random);

                var result = new PileOfCards(gameSubset, Cards.Ruins);

                for (int i = 0; i < ruinsCount; ++i)
                {
                    Card card = allRuinsCards.DrawCardFromTop();
                    if (card == null)
                    {
                        throw new Exception("Not enough ruins available.");
                    }
                    result.AddCardToTop(card);
                }
                result.EraseKnownCountKnowledge();

                return result;
            }
        }

        private static void Add(CardGameSubset gameSubset, List<PileOfCards> cardPiles, int initialCount, Card protoType)
        {
            if (gameSubset.isInitializing)
            {
                gameSubset.AddCard(protoType);
            }
            else
            {
                cardPiles.Add(new PileOfCards(gameSubset, protoType, initialCount));
            }
        }
    }
}

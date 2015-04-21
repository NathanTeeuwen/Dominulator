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

        public void SetKingdomPiles(IEnumerable<Card> cards)
        {
            var setCards = new HashSet<Card>();
            
            foreach(Card card in cards)
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

            foreach(var card in cardsToRemove)
            {
                setCards.Remove(card);
            }
        }
          
        public GameConfig ToGameConfig()
        {
            return new GameConfig(this.kingdomPiles, this.baneCard, this.useShelters, this.useColonyAndPlatinum, this.startingDeck, this.shuffleLuck);
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

    public enum StartingLocation
    {
        Supply,
        NonSupply,
        Hand
    }


    // lists the types of cards availble for selection by a player when buying or gaining
    public class CardGainAvailablility
    {
        public readonly Card card;
        public readonly int count;
        public StartingLocation startingLocation;

        public CardGainAvailablility(Card card, int count, StartingLocation startingLocation)
        {
            this.card = card;
            this.count = count;
            this.startingLocation = startingLocation; 
        }
    }    

    public enum CardAvailabilityType
    {
        AllPossibleCardsInGame,    // used for enumerating the types of cards available in the game
        TypesForBuyingOrGaining,   // used for showing in a UI what cards a player can choose between when buying or gaining        
        AdditionalCardsAfterKingdom
    }

    class CardGainAvailabilityBuilder
    {        
        List<CardGainAvailablility> list = new List<CardGainAvailablility>(capacity: 30);  // 30 is rough number of cards available in a kingom      

        public void AddSupply(int count, Card cardType)
        {
            this.list.Add(new CardGainAvailablility(cardType, count, StartingLocation.Supply));
        }

        public void AddNonSupply(int count, Card cardType)
        {
            this.list.Add(new CardGainAvailablility(cardType, count, StartingLocation.NonSupply));
        }

        public void AddStartingCard(Card cardType)
        {
            this.list.Add(new CardGainAvailablility(cardType, 1, StartingLocation.Hand));
        }

        public CardGainAvailablility[] Result
        {
            get
            {
                return this.list.ToArray();
            }
        }
    }

    public class GameConfig
    {
        static Card[] prizes = { Cards.BagOfGold, Cards.Diadem, Cards.Followers, Cards.Princess, Cards.TrustySteed };
        static Card[] ruins = { Cards.AbandonedMine, Cards.RuinedMarket, Cards.RuinedLibrary, Cards.RuinedVillage, Cards.Survivors };                         

        public readonly bool useShelters;
        public readonly bool useColonyAndPlatinum;
        public readonly Card[] kingdomPiles;
        public readonly Card baneCard;

        public readonly MapPlayerGameConfigToCardSet startingDeck;
        public readonly MapPlayerGameConfigToCardSet startingHand;        
        public readonly CardGameSubset cardGameSubset;

        public GameConfig(            
            Card[] kingdomPiles, 
            Card baneCard,
            bool useShelters, 
            bool useColonyAndPlatinum,            
            MapPlayerGameConfigToCardSet startingDecks = null,
            MapPlayerGameConfigToCardSet startingHands = null)
        {
            this.useShelters = useShelters;
            this.useColonyAndPlatinum = useColonyAndPlatinum;
            this.kingdomPiles = kingdomPiles;
            this.startingDeck = startingDecks; 
            this.startingHand = startingHands;            
            this.baneCard = baneCard;
            
            this.cardGameSubset = new CardGameSubset();
            var availabilities = GetCardAvailability(1, CardAvailabilityType.AllPossibleCardsInGame);
            foreach(var availability in availabilities)
                this.cardGameSubset.AddCard(availability.card);            
        }      

        public CardGainAvailablility[] GetCardAvailability(int numberOfPlayers, CardAvailabilityType cardAvailabilityType)
        {
            var builder = new CardGainAvailabilityBuilder();            

            int curseCount = (numberOfPlayers - 1) * 10;
            int ruinsCount = (numberOfPlayers - 1) * 10;
            int victoryCount = (numberOfPlayers == 2) ? 8 : 12;

            builder.AddSupply(60, Cards.Copper);
            builder.AddSupply(40, Cards.Silver);
            builder.AddSupply(30, Cards.Gold);
            if (this.useColonyAndPlatinum)
            {
                builder.AddSupply(20, Cards.Platinum);
            }            
            if (this.kingdomPiles.Where(card => card.potionCost != 0).Any())
            {
                builder.AddSupply(16, Cards.Potion);
            }
            if (this.kingdomPiles.Where(card => card.requiresSpoils).Any())
            {
                builder.AddNonSupply(16, Cards.Spoils);
            }

            if (this.useShelters)
            {
                switch (cardAvailabilityType)
                {
                    case CardAvailabilityType.AdditionalCardsAfterKingdom:
                    case CardAvailabilityType.AllPossibleCardsInGame:
                        {
                            builder.AddStartingCard(Cards.Necropolis);
                            builder.AddStartingCard(Cards.Hovel);
                            builder.AddStartingCard(Cards.OvergrownEstate);
                            break;
                        }
                }
            }

            
            builder.AddSupply(victoryCount + (!this.useShelters ? numberOfPlayers * 3 : 0), Cards.Estate);
            builder.AddSupply(victoryCount, Cards.Duchy);
            builder.AddSupply(victoryCount, Cards.Province);
            if (this.useColonyAndPlatinum)
            {
                builder.AddSupply(victoryCount, Cards.Colony);
                
            }
            builder.AddSupply(curseCount, Cards.Curse);

            if (cardAvailabilityType != CardAvailabilityType.AdditionalCardsAfterKingdom)
            {
                foreach (Card card in this.kingdomPiles)
                {
                    if (card.isVictory)
                    {
                        builder.AddSupply(victoryCount, card);
                    }
                    else
                    {
                        builder.AddSupply(card.defaultSupplyCount, card);
                    }

                    if (card == Cards.YoungWitch && baneCard != null)
                    {
                        if (baneCard.isVictory)
                        {
                            builder.AddSupply(victoryCount, baneCard);
                        }
                        else
                        {
                            builder.AddSupply(card.defaultSupplyCount, baneCard);
                        }                        
                    }
                }
            }
         
            if (this.kingdomPiles.Where(card => card.requiresRuins).Any())
            {
                switch (cardAvailabilityType)
                {
                    case CardAvailabilityType.AllPossibleCardsInGame:
                    case CardAvailabilityType.AdditionalCardsAfterKingdom:
                        {
                            foreach (var card in ruins)
                                builder.AddSupply(1, card);
                            break;
                        }
                }

                switch (cardAvailabilityType)
                {
                    case CardAvailabilityType.AllPossibleCardsInGame:
                    case CardAvailabilityType.TypesForBuyingOrGaining:
                        {
                            builder.AddSupply(ruinsCount, Cards.Ruins);
                            break;
                        }
                }
            }            

            if (this.kingdomPiles.Where(card => card == Cards.Hermit).Any())
            {
                builder.AddNonSupply(10, Cards.Madman);
            }

            if (this.kingdomPiles.Where(card => card == Cards.Urchin).Any())
            {
                builder.AddNonSupply(10, Cards.Mercenary);
            }

            if (this.kingdomPiles.Where(card => card == Cards.Tournament).Any())
            {
                foreach (Card prize in prizes)
                {
                    builder.AddNonSupply(1, prize);                    
                }                
            }

            return builder.Result;
        }

        public IEnumerable<CardCountPair> ShuffleLuck(int playerIndex)
        {
            return this.startingHand(playerIndex, this);
        }

        public IEnumerable<CardCountPair> StartingDeck(int playerIndex)
        {
            return this.startingDeck(playerIndex, this);
        }

        private PileOfCards[] GetPiles(int numberOfPlayers, Random random, bool isSupply)
        {
            var startingLocation = isSupply ? StartingLocation.Supply : StartingLocation.NonSupply;
            var result = new List<PileOfCards>();
            CardGainAvailablility[] availabilities = this.GetCardAvailability(numberOfPlayers, CardAvailabilityType.TypesForBuyingOrGaining);
            foreach (var availability in availabilities)
            {
                if (availability.startingLocation == startingLocation)
                {
                    if (availability.card == Cards.Ruins)
                    {
                        result.Add(CreateRuins(this.cardGameSubset, availability.count, random));
                    }
                    else
                    {
                        result.Add(new PileOfCards(this.cardGameSubset, availability.card, availability.count));
                    }
                }
            }

            return result.ToArray();
        }      
        
        public PileOfCards[] GetSupplyPiles(int numberOfPlayers, Random random)
        {
            return GetPiles(numberOfPlayers, random, isSupply: true);
        }
      
        public PileOfCards[] GetNonSupplyPiles(int numberOfPlayers)
        {
            return GetPiles(numberOfPlayers, null, isSupply: false);
        }
    
        private static PileOfCards CreateRuins(CardGameSubset gameSubset, int ruinsCount, Random random)
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
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
  
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

    public class GameConfig
    {
        static Card[] prizes = { Cards.BagOfGold, Cards.Diadem, Cards.Followers, Cards.Princess, Cards.TrustySteed };
        static Card[] ruins = { Cards.AbandonedMine, Cards.RuinedMarket, Cards.RuinedLibrary, Cards.RuinedVillage, Cards.Survivors };

        public readonly GameDescription gameDescription;

        public bool useShelters { get { return this.gameDescription.useShelters; } }
        public bool useColonyAndPlatinum { get { return this.gameDescription.useColonyAndPlatinum; } }
        public Card[] kingdomPiles { get { return this.gameDescription.kingdomPiles; } }
        public Card baneCard { get { return this.gameDescription.baneCard; } }

        public readonly MapPlayerGameConfigToCardSet startingDeck;
        public readonly MapPlayerGameConfigToCardSet startingHand;        
        public readonly CardGameSubset cardGameSubset;

        public GameConfig(            
            GameDescription gameDescription,
            MapPlayerGameConfigToCardSet startingDecks = null,
            MapPlayerGameConfigToCardSet startingHands = null)
        {
            this.gameDescription = gameDescription;
            this.startingDeck = startingDecks; 
            this.startingHand = startingHands;            
            
            
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

            if (this.NeedsRuins)
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

        public bool NeedsRuins
        {
            get
            {
                return this.kingdomPiles.Where(card => card.requiresRuins).Any();
            }
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
    }
}

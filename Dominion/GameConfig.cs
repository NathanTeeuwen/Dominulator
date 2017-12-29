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
        Hand,
        Trash,
        Special,  // used for sentinels like ruins, prizes etc
    }

    // lists the types of cards availble for selection by a player when buying or gaining
    public class CardGainAvailablility
    {
        public readonly Card card;
        public readonly int count;
        public StartingLocation startingLocation;

        public CardGainAvailablility(Card card, int count, StartingLocation startingLocation)
        {
            if (card == null)
            {
                throw new Exception();
            }

            this.card = card;
            this.count = count;
            this.startingLocation = startingLocation; 
        }
    }    

    public enum CardAvailabilityType
    {
        AllPossibleCardsInGame,    // used for enumerating the types of cards available in the game
        TypesForBuyingOrGaining,   // used for showing in a UI what cards a player can choose between when buying or gaining
    }

    public class GameConfig
    {
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
            int victoryCount = (numberOfPlayers == 2) ? 8 : 12;

            var builder = new CardGainAvailabilityBuilder(victoryCount, numberOfPlayers, cardAvailabilityType);
            
            builder.AddCardTypeIfNotPresent(Cards.Copper);
            builder.AddCardTypeIfNotPresent(Cards.Silver);
            builder.AddCardTypeIfNotPresent(Cards.Gold);
            if (this.useColonyAndPlatinum)
            {
                builder.AddCardTypeIfNotPresent(Cards.Platinum);
                builder.AddCardTypeIfNotPresent(Cards.Colony);
            }

            if (this.useShelters)
            {
                switch (cardAvailabilityType)
                {
                    case CardAvailabilityType.AllPossibleCardsInGame:
                        {
                            builder.AddStartingCard(Cards.Necropolis);
                            builder.AddStartingCard(Cards.Hovel);
                            builder.AddStartingCard(Cards.OvergrownEstate);
                            break;
                        }
                }
            }

            builder.AddSupply(victoryCount + (this.useShelters ? 0 : numberOfPlayers * 3), Cards.Estate);
            builder.AddCardTypeIfNotPresent(Cards.Duchy);
            builder.AddCardTypeIfNotPresent(Cards.Province);
            builder.AddSupply((numberOfPlayers - 1) * 10, Cards.Curse);

            foreach (Card card in this.kingdomPiles)
            {
                builder.AddCardTypeIfNotPresent(card);
                if (card == Cards.YoungWitch && baneCard != null)
                {
                    builder.AddCardTypeIfNotPresent(baneCard);
                }
            }

            return builder.Result;
        }

        public bool NeedsRuins
        {
            get
            {
                return this.kingdomPiles.Where(card => card.isLooter).Any();
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

        internal class CardGainAvailabilityBuilder
        {
            List<CardGainAvailablility> list = new List<CardGainAvailablility>(capacity: 30);  // 30 is rough number of cards available in a kingom      
            private System.Collections.Generic.HashSet<Dominion.Card> addedCards = new System.Collections.Generic.HashSet<Dominion.Card>();
            private bool hasSpirits = false;
            public readonly int victoryCount;
            public readonly int numberOfPlayers;
            public readonly CardAvailabilityType cardAvailabilityType;

            public CardGainAvailabilityBuilder(int victoryCount, int numberOfPlayers, CardAvailabilityType cardAvailabilityType)
            {
                this.victoryCount = victoryCount;
                this.numberOfPlayers = numberOfPlayers;
                this.cardAvailabilityType = cardAvailabilityType;
            }

            public void AddCardTypeIfNotPresent(Card card)
            {
                if (this.addedCards.Contains(card))
                    return;
                this.addedCards.Add(card);

                int count = card.isVictory ? victoryCount : card.defaultSupplyCount;
                if (card.startingLocation != StartingLocation.Special)
                    this.list.Add(new CardGainAvailablility(card, count, card.startingLocation));
                card.AddAdditionalCardsNeeded(this);

                if (card.requiresSpoils)
                    this.AddCardTypeIfNotPresent(Cards.Spoils);
                if (card.potionCost != 0)
                    this.AddCardTypeIfNotPresent(Cards.Potion);
                if (card.isLooter)
                    this.AddCardTypeIfNotPresent(Cards.Ruins);
                if (card.isFate)
                    //add boons 
                    ;
                if (card.isDoom)
                    //add hexes 
                    ;
            }

            public void AddSpirits()
            {
                if (this.hasSpirits)
                    return;
                this.hasSpirits = true;
                this.AddCardTypeIfNotPresent(Cards.Ghost);
                this.AddCardTypeIfNotPresent(Cards.Imp);
                this.AddCardTypeIfNotPresent(Cards.WilloWisp);
            }

            public void AddSupply(Card cardType)
            {
                if (cardType.isVictory)
                {
                    this.AddSupply(victoryCount, cardType);
                }
                else
                {
                    this.AddSupply(cardType.defaultSupplyCount, cardType);
                }
            }

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

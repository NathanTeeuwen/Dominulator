using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public enum StartingCardSplit
    {
        Random,
        Split43,
        Split52,
    }

    public class GameConfig
    {
        public readonly bool useShelters;
        public readonly bool useColonyAndPlatinum;
        public readonly Card[] kingdomPiles;
        public readonly IEnumerable<CardCountPair> startingDeck;
        public readonly IEnumerable<CardCountPair> startingHand;
        public readonly CardGameSubset cardGameSubset;

        public GameConfig(bool useShelters, bool useColonyAndPlatinum, Card[] supplyPiles, IEnumerable<CardCountPair> startingDeck, IEnumerable<CardCountPair> startingHand)
        {
            this.useShelters = useShelters;
            this.useColonyAndPlatinum = useColonyAndPlatinum;
            this.kingdomPiles = supplyPiles;
            this.startingDeck = startingDeck;
            this.startingHand = startingHand;
            this.cardGameSubset = new CardGameSubset();

            GetSupplyPiles(1, null, this.cardGameSubset);
            GetNonSupplyPiles(this.cardGameSubset);
            foreach (var pair in this.StartingDecks(1)[0])
            {
                this.cardGameSubset.AddCard(pair.Card);
            }

            this.cardGameSubset.isInitializing = false;
        }

        public GameConfig(bool useShelters, bool useColonyAndPlatinum, params Card[] supplyPiles)
            : this(useShelters, useColonyAndPlatinum, supplyPiles, null, null)
        {            
        }

        public GameConfig(params Card[] supplyPiles)
            : this(false, false, supplyPiles, null, null)
        {            
        }

        public GameConfig(StartingCardSplit split, params Card[] supplyPiles)
            : this(false, false, supplyPiles, null, GetStartingHandForSplit(split))
        {
        }

        public IEnumerable<CardCountPair>[] StartingHands(int playerCount)
        {
            return this.startingHand == null ? null : GetCardSetSameForAllPlayers(playerCount, this.startingHand);
        }

        public IEnumerable<CardCountPair>[] StartingDecks(int playerCount)
        {
            return GetCardSetSameForAllPlayers(playerCount, this.StartingDeck);            
        }

        public static IEnumerable<CardCountPair>[] GetCardSetSameForAllPlayers(int playerCount, IEnumerable<CardCountPair> cards)
        {            
            var result = new IEnumerable<CardCountPair>[playerCount];
            for (int i = 0; i < playerCount; ++i)
                result[i] = cards;

            return result;
        }

        public IEnumerable<CardCountPair> StartingDeck
        {
            get
            {
                if (this.startingDeck != null)
                    return this.startingDeck;

                if (this.useShelters)
                {
                    return 
                        new CardCountPair[] {
                            new CardCountPair(Card.Type<CardTypes.Copper>(), 7),
                            new CardCountPair(Card.Type<CardTypes.Hovel>(), 1),
                            new CardCountPair(Card.Type<CardTypes.Necropolis>(), 1),
                            new CardCountPair(Card.Type<CardTypes.OvergrownEstate>(), 1)
                        };
                }
                else
                {
                    return
                        new CardCountPair[] {
                            new CardCountPair(Card.Type<CardTypes.Copper>(), 7),
                            new CardCountPair(Card.Type<CardTypes.Estate>(), 3) 
                        };
                }
            }
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
            Add<CardTypes.Copper>(gameSubset, supplyCardPiles, 60);
            Add<CardTypes.Silver>(gameSubset, supplyCardPiles, 40);
            Add<CardTypes.Gold>(gameSubset, supplyCardPiles, 30);
            Add<CardTypes.Curse>(gameSubset, supplyCardPiles, curseCount);
            Add<CardTypes.Estate>(gameSubset, supplyCardPiles, victoryCount + (!this.useShelters ? playerCount * 3 : 0));
            Add<CardTypes.Duchy>(gameSubset, supplyCardPiles, victoryCount);
            Add<CardTypes.Province>(gameSubset, supplyCardPiles, victoryCount);
            
            if (this.useColonyAndPlatinum)
            {
                Add<CardTypes.Colony>(gameSubset, supplyCardPiles, victoryCount);
                Add<CardTypes.Platinum>(gameSubset, supplyCardPiles, 20);
            }

            if (this.kingdomPiles.Where(card => card.potionCost != 0).Any())
            {
                Add<CardTypes.Potion>(gameSubset, supplyCardPiles, 16);
            }
            else
            {
                gameSubset.AddCard(Card.Type<CardTypes.Potion>());
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
            
            if (this.kingdomPiles.Where(card => card.requiresSpoils).Any())
            {
                Add<CardTypes.Spoils>(gameSubset, nonSupplyCardPiles, 16);
            }

            if (this.kingdomPiles.Where(card => card.Is<CardTypes.Hermit>()).Any())
            {
                Add<CardTypes.Madman>(gameSubset, nonSupplyCardPiles, 10);
            }
            if (this.kingdomPiles.Where(card => card.Is<CardTypes.Urchin>()).Any())
            {
                Add<CardTypes.Mercenary>(gameSubset, nonSupplyCardPiles, 10);
            }            

            return nonSupplyCardPiles.ToArray();
        }

        private static IEnumerable<CardCountPair> GetStartingHandForSplit(StartingCardSplit split)
        {
            switch (split)
            {
                case StartingCardSplit.Random: return null;
                case StartingCardSplit.Split52: return new CardCountPair[] { new CardCountPair(Card.Type<CardTypes.Copper>(), 5) };
                case StartingCardSplit.Split43: return new CardCountPair[] { new CardCountPair(Card.Type<CardTypes.Copper>(), 4),  
                                                                              new CardCountPair(Card.Type<CardTypes.Estate>(), 1)};
                default:
                    throw new Exception();
            }
        }

        private static PileOfCards CreateRuins(CardGameSubset gameSubset, int ruinsCount, Random random)
        {
            if (gameSubset.isInitializing)
            {
                gameSubset.AddCard(Card.Type<CardTypes.Ruins>());
                gameSubset.AddCard(Card.Type<CardTypes.AbandonedMine>());
                gameSubset.AddCard(Card.Type<CardTypes.RuinedMarket>());
                gameSubset.AddCard(Card.Type<CardTypes.RuinedLibrary>());
                gameSubset.AddCard(Card.Type<CardTypes.RuinedVillage>());
                gameSubset.AddCard(Card.Type<CardTypes.Survivors>());
                return null;
            }
            else
            {
                int ruinCountPerPile = 10;
                var allRuinsCards = new ListOfCards(gameSubset);
                allRuinsCards.AddNCardsToTop(Card.Type<CardTypes.AbandonedMine>(), ruinCountPerPile);
                allRuinsCards.AddNCardsToTop(Card.Type<CardTypes.RuinedMarket>(), ruinCountPerPile);
                allRuinsCards.AddNCardsToTop(Card.Type<CardTypes.RuinedLibrary>(), ruinCountPerPile);
                allRuinsCards.AddNCardsToTop(Card.Type<CardTypes.RuinedVillage>(), ruinCountPerPile);
                allRuinsCards.AddNCardsToTop(Card.Type<CardTypes.Survivors>(), ruinCountPerPile);

                allRuinsCards.Shuffle(random);

                var result = new PileOfCards(gameSubset, Card.Type<CardTypes.Ruins>());

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

        private static void Add<CardType>(CardGameSubset gameSubset, List<PileOfCards> cardPiles, int initialCount)
            where CardType : Card, new()
        {
            Add(gameSubset, cardPiles, initialCount, Card.Type<CardType>());
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

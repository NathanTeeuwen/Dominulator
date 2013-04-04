using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public struct CompareCardByType
            : IEqualityComparer<Card>,
              IComparer<Card>
    {
        public bool Equals(Card x, Card y)
        {
            return x.GetType().Equals(y.GetType());
        }

        public int GetHashCode(Card x)
        {
            return x.GetType().GetHashCode();
        }
        
        public int Compare(Card x, Card y)
        {         
            return x.name.CompareTo(y.name);
        }
    }

    public enum GainReason
    {
        Gain,
        Buy
    }

    public enum DeckPlacement
    {
        Hand,
        Discard,
        Play,
        Trash,
        TopOfDeck,
    }

    public struct CardPlacementPair
    {
        public readonly Card card;
        public readonly DeckPlacement placement;

        public CardPlacementPair(Card card, DeckPlacement placement)
        {
            this.card = card;
            this.placement = placement;
        }
    }

    public delegate int VictoryPointCounter(PlayerState player);
    public delegate DeckPlacement MapCardToPlacement(Card card);
    public delegate CardPlacementPair PlaceCardsFromList(BagOfCards cards);    
    public delegate bool IsValidChoice(PlayerActionChoice availableChoice);

    public delegate bool CardPredicate(Card card);
    public delegate bool GameStatePredicate(GameState gameState);

    public enum CostConstraint
    {
        Exactly,
        UpTo
    }

    public enum CardRelativeCost
    {
        RelativeCost,
        AbsoluteCost
    }


    public class GameState
    {
        public IGameLog gameLog;
        public PlayerCircle players;
        public PileOfCards[] supplyPiles;
        public BagOfCards trash;

        // special piles not in the supply - not available in every game
        private PileOfCards blackMarketDeck;

        public GameState(             
            IGameLog gameLog,
            IPlayerAction[] players,
            GameConfig gameConfig)
        {
            int playerCount = players.Length;
            this.gameLog = gameLog;
            this.players = new PlayerCircle(playerCount, players, this.gameLog);
 
            var cardPiles = new List<PileOfCards>(capacity:20);

            int curseCount = (playerCount - 1) * 10;
            int ruinsCount = curseCount;
            int victoryCount = (playerCount == 2) ? 8 : 12;

            Add<CardTypes.Copper>(cardPiles, 60);
            Add<CardTypes.Silver>(cardPiles, 40);
            Add<CardTypes.Gold>(cardPiles, 30);
            Add<CardTypes.Curse>(cardPiles, curseCount);
            Add<CardTypes.Estate>(cardPiles, victoryCount + playerCount * 3);
            Add<CardTypes.Duchy>(cardPiles, victoryCount);
            Add<CardTypes.Province>(cardPiles, victoryCount);

            if (gameConfig.useColonyAndPlatinum)
            {
                Add<CardTypes.Colony>(cardPiles, victoryCount);
                Add<CardTypes.Platinum>(cardPiles, 20);
            }

            bool requiresRuins = false;

            foreach (Card card in gameConfig.supplyPiles)
            {
                if (card.isVictory)
                {
                    Add(cardPiles, victoryCount, card);
                }
                else
                {
                    Add(cardPiles, card.defualtSupplyCount, card);
                }

                requiresRuins |= card.requiresRuins;
            }

            if (requiresRuins)
            {
                cardPiles.Add(CreateRuins(ruinsCount));
            }

            this.supplyPiles = cardPiles.ToArray();
            this.trash = new BagOfCards();
                       
            foreach (PlayerState player in this.players.AllPlayers)
            {
                player.GainCardsFromSupply(this, typeof(CardTypes.Estate), 3);
                player.GainCardsFromSupply(this, typeof(CardTypes.Copper), 7);                
            }

            this.players.AllPlayersDrawInitialCards();         
        }

        private static PileOfCards CreateRuins(int ruinsCount)
        {
            int ruinCountPerPile = 10;
            var allRuinsCards = new ListOfCards();
            allRuinsCards.AddNCardsToTop(new CardTypes.AbandonedMine(), ruinCountPerPile);
            allRuinsCards.AddNCardsToTop(new CardTypes.RuinedMarket(), ruinCountPerPile);
            allRuinsCards.AddNCardsToTop(new CardTypes.RuinedLibrary(), ruinCountPerPile);
            allRuinsCards.AddNCardsToTop(new CardTypes.RuinedVillage(), ruinCountPerPile);
            allRuinsCards.AddNCardsToTop(new CardTypes.Survivors(), ruinCountPerPile);

            allRuinsCards.Shuffle();

            var result = new PileOfCards(new CardTypes.Ruin());

            for (int i = 0; i < ruinsCount; ++i)
            {
                Card card = allRuinsCards.DrawCardFromTop();
                if (card == null)
                {
                    throw new Exception("Not enough ruins available.");
                }
                result.AddCardToTop(card);
            }
                        
            return result;
        }

        private static void Add<CardType>(List<PileOfCards> cardPiles, int initialCount)
            where CardType : Card, new()
        {
            
            Add(cardPiles, initialCount, new CardType());
        }

        private static void Add(List<PileOfCards> cardPiles, int initialCount, Card protoType)           
        {                        
            cardPiles.Add( new PileOfCards(protoType, initialCount));
        }

        public bool IsVictoryConditionReached()
        {
            int countEmptyPiles = 0;
            foreach (PileOfCards pile in this.supplyPiles)
            {
                if (pile.IsEmpty)
                {
                    if (pile.IsType<CardTypes.Province>() ||
                        pile.IsType<CardTypes.Colony>())
                    {
                        return true;
                    }

                    ++countEmptyPiles;
                }                
            }

            if (countEmptyPiles >= 3)
            {
                return true;
            }

            return false;
        }

        public void PlayGameToEnd()
        {
            while (!IsVictoryConditionReached())
            {
                if (this.players.BeginningOfRound)
                {
                    this.gameLog.BeginRound();
                }

                PlayerState currentPlayer = this.players.CurrentPlayer;
                PlayTurn(currentPlayer);
                if (currentPlayer.actions.WantToResign(this))
                {
                    break;
                }
                this.players.PassTurnLeft();                
            }

            this.gameLog.EndGame(this);
        }

        public PlayerState[] WinningPlayers
        {
            get
            {
                var orderedPlayers = this.players.AllPlayers.ToArray();
                Array.Sort(orderedPlayers, ComparePlayerWinner);
                var result = new List<PlayerState>();
                result.Add(orderedPlayers[0]);
                for (int i = 1; i < orderedPlayers.Length; ++i)
                {
                    if (ComparePlayerWinner(orderedPlayers[i], orderedPlayers[0]) != 0)
                    {
                        break;
                    }
                    result.Add(orderedPlayers[i]);
                }

                return result.ToArray();
            }
        }

        static int ComparePlayerWinner(PlayerState first, PlayerState second)
        {
            int scoreDifference = second.TotalScore() - first.TotalScore();
            if (scoreDifference > 0)
            {
                return 1;
            }
            if (scoreDifference < 0)
            {
                return -1;
            }

            return second.numberOfTurnsPlayed - first.numberOfTurnsPlayed;
        }

        public void PlayTurn(PlayerState currentPlayerState)
        {
            currentPlayerState.numberOfTurnsPlayed += 1;
            IPlayerAction currentPlayer = currentPlayerState.actions;

            this.gameLog.BeginTurn(currentPlayerState);
            this.gameLog.BeginScope();
            currentPlayerState.InitializeTurn();
            currentPlayer.BeginTurn();            

            DoActionPhase(currentPlayerState);
            DoPlayTreasures(currentPlayerState);
            DoBuyPhase(currentPlayerState);            
            currentPlayerState.CleanupPhase();
            currentPlayerState.DrawUntilCountInHand(5);
            
            currentPlayer.EndTurn();            
            this.gameLog.EndTurn(currentPlayerState);
            this.gameLog.EndScope();
        }

        private void DoActionPhase(PlayerState currentPlayer)
        {
            while (currentPlayer.turnCounters.availableActionCount > 0)
            {
                currentPlayer.turnCounters.availableActionCount--;

                if (!currentPlayer.RequestPlayerPlayActionFromHand(this, isOptional: true))
                {
                    break;
                }                
            }
        }

        private void DoBuyPhase(PlayerState currentPlayerState)
        {
            while (currentPlayerState.turnCounters.availableBuys > 0)
            {
                Type cardType = currentPlayerState.actions.GetCardFromSupplyToBuy(this);
                if (cardType == null)
                {
                    return;
                }

                Card gainedCard = this.PlayerGainCardFromSupply(cardType, currentPlayerState, DeckPlacement.Discard, GainReason.Buy);
                if (gainedCard == null)
                {
                    return;
                }
                currentPlayerState.turnCounters.availableCoins -= gainedCard.CurrentCoinCost(currentPlayerState);
                currentPlayerState.turnCounters.availableBuys -= 1;
            }
        }

        internal void DoPlayTreasures(PlayerState currentPlayer)
        {
            while (true)
            {
                Type cardTypeToPlay = currentPlayer.actions.GetTreasureFromHandToPlay(this);
                if (cardTypeToPlay == null)
                {
                    break;
                }

                Card currentCard = currentPlayer.RemoveCardFromHand(cardTypeToPlay);
                if (currentCard == null)
                {
                    throw new Exception("Player tried to remove a card that wasn't available in hand");
                }
                
                currentPlayer.DoPlayTreasure(currentCard, this);
            }
        }

        internal PileOfCards GetSpecialPile(Type cardType)
        {
            if (cardType.Equals(typeof(CardTypes.BlackMarket)))
            {
                return this.blackMarketDeck;
            }
            else
            {
                throw new Exception("Card type does not have a special pile");
            }
        }

        public PileOfCards GetPile(Card card)
        {
            return GetPile(card.GetType());
        }

        public PileOfCards GetPile(Type cardType)
        {
            for (int i = 0; i < this.supplyPiles.Length; ++i)
            {
                PileOfCards cardPile = this.supplyPiles[i];
                if (cardPile.IsType(cardType))
                {
                    return cardPile;
                }
            }

            return null;
        }

        public PileOfCards GetPile<cardType>()
        {
            for (int i = 0; i < this.supplyPiles.Length; ++i)
            {
                PileOfCards cardPile = this.supplyPiles[i];
                if (cardPile.IsType<cardType>())
                {
                    return cardPile;
                }
            }

            return null;
        }

        public Card PlayerGainCardFromSupply(Type cardType, PlayerState playerState, DeckPlacement defaultLocation = DeckPlacement.Discard, GainReason gainReason = GainReason.Gain)
        {
            PileOfCards pile = this.GetPile(cardType);
            if (pile == null)
            {
                return null;
            }

            Card card = pile.DrawCardFromTop();
            if (card == null)
            {
                return null;
            }

            playerState.GainCard(this, card, defaultLocation, gainReason);

            return card;
        }

        public void PlayerGainCardFromSupply<cardType>(PlayerState playerState)
        {
            PlayerGainCardFromSupply(typeof(cardType), playerState);            
        }
    }
        
    public enum PlayerActionChoice
    {
        PlusCoin,
        Discard,
        PlusCard,
        PlusAction,
        PlusBuy,
        GainCard,
        Trash
    }      
}

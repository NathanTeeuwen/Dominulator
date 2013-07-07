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
        None,
        Default
    }

    public enum PlayerActionChoice
    {
        PlusCoin,
        PutInHand,
        Discard,
        PlusCard,
        PlusAction,
        PlusBuy,
        GainCard,
        TopDeck,
        Trash,
        Nothing,
        SetAsideTopCardOnNativeVillageMat,
        PutNativeVillageMatInHand
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

    public enum PlayPhase
    {
        Action,
        PlayTreasure,
        Buy,
        Cleanup,
        NotMyTurn
    }

    class MapPileOfCardsToProperty<T>
    {
        PileOfCards[] supplyPiles;
        T[] array;

        public MapPileOfCardsToProperty(PileOfCards[] supplyPiles)
        {
            this.supplyPiles = supplyPiles;
            this.array = new T[this.supplyPiles.Length];
        }

        public T this[Card card]
        {
            get
            {
                return this.array[this.GetIndexForPile(GetPile(card))];
            }

            set
            {
                this.array[this.GetIndexForPile(GetPile(card))] = value;
            }
        }

        public T this[Type card]
        {
            get
            {
                return this.array[this.GetIndexForPile(GetPile(card))];
            }

            set
            {
                this.array[this.GetIndexForPile(GetPile(card))] = value;
            }
        }

        public T this[PileOfCards pileOfCards]
        {
            get
            {
                return this.array[this.GetIndexForPile(pileOfCards)];
            }

            set
            {
                this.array[this.GetIndexForPile(pileOfCards)] = value;
            }
        }

        private PileOfCards GetPile(Card card)
        {
            return GetPile(card.GetType());
        }

        private PileOfCards GetPile(Type cardType)
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

        private int GetIndexForPile(PileOfCards pile)
        {
            for (int index = 0; index < this.supplyPiles.Length; ++index)
            {
                if (object.ReferenceEquals(pile, this.supplyPiles[index]))
                {
                    return index;
                }
            }

            throw new Exception("Pile not a part of supply");
        }
    }

    public class GameState
    {
        internal bool hasCurrentPlayerGainedCard;
        internal bool doesCurrentPlayerNeedOutpostTurn;
        public IGameLog gameLog;
        public PlayerCircle players;
        public PileOfCards[] supplyPiles;
        public PileOfCards[] nonSupplyPiles;
        public BagOfCards trash;
        private MapPileOfCardsToProperty<bool> hasPileEverBeenGained;
        private MapPileOfCardsToProperty<int> pileEmbargoTokenCount;

        // special piles not in the supply - not available in every game
        private PileOfCards blackMarketDeck;

        public GameState(             
            IGameLog gameLog,
            IPlayerAction[] players,
            GameConfig gameConfig,
            Random random)
        {
            int playerCount = players.Length;
            this.gameLog = gameLog;
            this.players = new PlayerCircle(playerCount, players, this.gameLog, random);                         
            this.supplyPiles = gameConfig.GetSupplyPiles(playerCount, random);
            this.nonSupplyPiles = gameConfig.GetNonSupplyPiles();
            this.hasPileEverBeenGained = new MapPileOfCardsToProperty<bool>(this.supplyPiles);
            this.pileEmbargoTokenCount = new MapPileOfCardsToProperty<int>(this.supplyPiles);
            this.trash = new BagOfCards();
                       
            foreach (PlayerState player in this.players.AllPlayers)
            {
                if (gameConfig.useShelters)
                {
                    player.GainCard(this, new CardTypes.Hovel());
                    player.GainCard(this, new CardTypes.Necropolis());
                    player.GainCard(this, new CardTypes.OvergrownEstate());
                }
                else
                {
                    for (int i = 0; i < 3; ++i)
                        player.GainCard(this, new CardTypes.Estate());
                }
                
                player.GainCardsFromSupply(this, typeof(CardTypes.Copper), 7);
            }

            this.players.AllPlayersDrawInitialCards();         
        }

        private static PileOfCards CreateRuins(int ruinsCount, Random random)
        {
            int ruinCountPerPile = 10;
            var allRuinsCards = new ListOfCards();
            allRuinsCards.AddNCardsToTop(new CardTypes.AbandonedMine(), ruinCountPerPile);
            allRuinsCards.AddNCardsToTop(new CardTypes.RuinedMarket(), ruinCountPerPile);
            allRuinsCards.AddNCardsToTop(new CardTypes.RuinedLibrary(), ruinCountPerPile);
            allRuinsCards.AddNCardsToTop(new CardTypes.RuinedVillage(), ruinCountPerPile);
            allRuinsCards.AddNCardsToTop(new CardTypes.Survivors(), ruinCountPerPile);

            allRuinsCards.Shuffle(random);

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
            result.EraseKnownCountKnowledge();
                        
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
            int noGainCount = 0;

            while (!IsVictoryConditionReached())
            {
                this.hasCurrentPlayerGainedCard = false;
                this.doesCurrentPlayerNeedOutpostTurn = false;

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

                if (this.hasCurrentPlayerGainedCard == false)
                {
                    ++noGainCount;
                    if (noGainCount >= 10)
                    {
                        break;
                    }
                }
                else
                {
                    noGainCount = 0;
                }

                if (!this.doesCurrentPlayerNeedOutpostTurn)
                {
                    this.players.PassTurnLeft();
                }
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
            this.gameLog.PushScope();
            currentPlayerState.InitializeTurn();
            currentPlayer.BeginTurn();

            ReturnCardsToHandAtStartOfTurn(currentPlayerState);
            DoDurationActionsFromPreviousTurn(currentPlayerState);
            DoActionPhase(currentPlayerState);
            DoPlayTreasures(currentPlayerState);
            DoBuyPhase(currentPlayerState);
            DoCleanupPhase(currentPlayerState);

            int cardCountForNextTurn = this.doesCurrentPlayerNeedOutpostTurn ? 3 : 5;
            currentPlayerState.DrawUntilCountInHand(cardCountForNextTurn);
            currentPlayerState.playPhase = PlayPhase.NotMyTurn;

            currentPlayer.EndTurn();            
            this.gameLog.EndTurn(currentPlayerState);
            this.gameLog.PopScope();
        }

        private void ReturnCardsToHandAtStartOfTurn(PlayerState currentPlayer)
        {
            currentPlayer.MoveCardsFromPreviousTurnIntoHand();
        }

        private void DoDurationActionsFromPreviousTurn(PlayerState currentPlayer)
        {            
            foreach (Card card in currentPlayer.durationCards)
            {
                this.gameLog.ReceivedDurationEffectFrom(currentPlayer, card);
                this.gameLog.PushScope();
                card.DoSpecializedDurationActionAtBeginningOfTurn(currentPlayer, this);
                this.gameLog.PopScope();
            }

            currentPlayer.MoveDurationCardsToInPlay();         
        }

        private void DoActionPhase(PlayerState currentPlayer)
        {
            currentPlayer.playPhase = PlayPhase.Action;
            while (currentPlayer.AvailableActions > 0)
            {
                currentPlayer.turnCounters.RemoveAction();

                if (!currentPlayer.RequestPlayerPlayActionFromHand(this, isOptional: true))
                {
                    break;
                }                
            }
        }

        private bool CardAvailableForPurchaseForCurrentPlayer(Card card)
        {
            PlayerState currentPlayer = this.players.CurrentPlayer;
            return currentPlayer.AvailableCoins >= card.CurrentCoinCost(currentPlayer) &&
                       this.GetPile(card).Any() &&
                       !card.IsRestrictedFromBuy(currentPlayer, this) &&
                       !currentPlayer.turnCounters.cardsBannedFromPurchase.Contains(card.GetType());
        }

        private void DoBuyPhase(PlayerState currentPlayer)
        {
            currentPlayer.playPhase = PlayPhase.Buy;
            while (currentPlayer.turnCounters.AvailableBuys > 0)
            {
                Type cardType = currentPlayer.actions.GetCardFromSupplyToBuy(this, CardAvailableForPurchaseForCurrentPlayer);
                if (cardType == null)
                {
                    return;
                }

                Card boughtCard = this.PlayerGainCardFromSupply(cardType, currentPlayer, DeckPlacement.Discard, GainReason.Buy);
                if (boughtCard == null)
                {
                    return;
                }
                
                int embargoCount = this.pileEmbargoTokenCount[boughtCard];
                for (int i = 0; i < embargoCount; ++i)
                {
                    currentPlayer.GainCardFromSupply<CardTypes.Curse>(this);                    
                }

                currentPlayer.turnCounters.RemoveCoins(boughtCard.CurrentCoinCost(currentPlayer));
                currentPlayer.turnCounters.RemoveBuy();

                if (boughtCard.canOverpay)
                {
                    currentPlayer.RequestPlayerOverpayForCard(boughtCard, this);
                }                

                foreach (Card cardInPlay in currentPlayer.CardsInPlay)
                {
                    gameLog.PushScope();
                    cardInPlay.DoSpecializedActionOnBuyWhileInPlay(currentPlayer, this, boughtCard);                    
                    gameLog.PopScope();
                }
            }
        }

        private void DoCleanupPhase(PlayerState currentPlayer)
        {
            currentPlayer.playPhase = PlayPhase.Cleanup;

            foreach (Card cardInPlay in currentPlayer.cardsPlayed)
            {
                cardInPlay.DoSpecializedCleanupAtStartOfCleanup(currentPlayer, this);
            }
            
            currentPlayer.CleanupCardsToDiscard();
        }

        internal void DoPlayTreasures(PlayerState currentPlayer)
        {
            currentPlayer.playPhase = PlayPhase.PlayTreasure;
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
            var result = GetPile(this.supplyPiles, cardType);
            if (result != null)
                return result;

            result = GetPile(this.nonSupplyPiles, cardType);            

            return result;
        }

        private static PileOfCards GetPile(PileOfCards[] piles, Type cardType)
        {
            for (int i = 0; i < piles.Length; ++i)
            {
                PileOfCards cardPile = piles[i];
                if (cardPile.IsType(cardType))
                {
                    return cardPile;
                }
            }

            return null;
        }       

        public PileOfCards GetPile<cardType>()
        {
            return GetPile(typeof(cardType));            
        }

        public Card PlayerGainCardFromSupply(Type cardType, PlayerState playerState, DeckPlacement defaultLocation = DeckPlacement.Discard, GainReason gainReason = GainReason.Gain)
        {
            PileOfCards pile = this.GetPile(cardType);
            if (pile == null)
            {
                return null;
            }

            this.hasPileEverBeenGained[pile] = true;            

            Card card = pile.DrawCardFromTop();
            if (card == null)
            {
                return null;
            }

            playerState.GainCard(this, card, defaultLocation, gainReason);

            return card;
        }       

        internal void AddEmbargoTokenToPile(PileOfCards pile)
        {
            this.pileEmbargoTokenCount[pile] += 1;
        }         

        internal bool HasCardEverBeenGainedFromPile(PileOfCards pile)
        {
            return this.hasPileEverBeenGained[pile];            
        }

        internal bool DoesSupplyHaveCard<T>()
            where T : Card
        {
            return this.supplyPiles.Select( pile => pile.ProtoTypeCard.Is<T>()).Any();
        }

        internal int CountOfDifferentTreasuresInTrash()
        {
            return this.trash.Where(card => card.isTreasure).GroupBy(card => card.GetType()).Count();
        }
    }          
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominion
{   
    public class GameState
    {
        private readonly Game game;

        internal IGameLog gameLog { get { return this.game.GameLog; } }
        internal bool hasCurrentPlayerGainedCard;
        internal bool doesCurrentPlayerNeedOutpostTurn;
        internal PlayerState self;        
        public PlayerCircle players;
        public PileOfCards[] supplyPiles;
        public PileOfCards[] nonSupplyPiles;
        private MapOfCardsForGameSubset<PileOfCards> mapCardToPile;
        public BagOfCards trash;
        private MapPileOfCards<bool> hasPileEverBeenGained;
        private MapPileOfCards<int> pileEmbargoTokenCount;                     

        public int InProgressGameIndex;

        public Game Game
        {
            get
            {
                return this.game;
            }
        }


        public CardGameSubset CardGameSubset
        {
            get
            {
                return this.game.CardGameSubset;
            }
        }

        public PlayerState Self
        {
            get
            {
                return this.self;
            }
        }

        public Card CurrentCardBeingPlayed 
        { 
            get 
            { 
                return this.players.CurrentPlayer.CurrentCardBeingPlayed; 
            } 
        }

        public Card CurrentCardBeingBought
        {
            get
            {
                return this.players.CurrentPlayer.CurrentCardBeingBought;
            }
        }

        // special piles not in the supply - not available in every game
        private PileOfCards blackMarketDeck = null;

        public GameState(                         
            IPlayerAction[] playerActions,
            int[] playerPositions,            
            Game game,
            IEnumerable<CardCountPair>[] startingDeckPerPlayer = null)
        {
            this.game = game;
            GameConfig gameConfig = game.GameConfig;

            int playerCount = playerActions.Length;            
            this.supplyPiles = gameConfig.GetSupplyPiles(playerCount, game.random);
            this.nonSupplyPiles = gameConfig.GetNonSupplyPiles();

            this.mapCardToPile = new MapOfCardsForGameSubset<PileOfCards>(this.CardGameSubset);
            this.BuildMapOfCardToPile();

            this.players = new PlayerCircle(playerCount, playerActions, playerPositions, game);

            this.hasPileEverBeenGained = new MapPileOfCards<bool>(this.supplyPiles);
            this.pileEmbargoTokenCount = new MapPileOfCards<int>(this.supplyPiles);
            this.trash = new BagOfCards(this.CardGameSubset);            

            this.GainStartingCards(gameConfig);

            this.players.AllPlayersDrawInitialCards(gameConfig);    
     
            foreach (PileOfCards cardPile in this.supplyPiles)
            {
                cardPile.ProtoTypeCard.DoSpecializedSetupIfInSupply(this);
            }
        }        

        private void BuildMapOfCardToPile()
        {
            foreach (Card card in this.CardGameSubset)
            {
                this.mapCardToPile[card] = this.GetPileBuilder(card);
            }
        }
        
        private void GainStartingCards(GameConfig gameConfig)
        {
            foreach (PlayerState player in this.players.AllPlayers)
            {                 
                foreach (CardCountPair pair in gameConfig.StartingDeck(player.PlayerIndex))
                {
                    if (pair.Card.isShelter)
                    {
                        player.GainCard(this, pair.Card, DeckPlacement.Supply);
                    }
                    else
                    {
                        player.GainCardsFromSupply(this, pair.Card, pair.Count);
                    }
                }
            }                                        
        }              
      
        public bool IsVictoryConditionReached()
        {
            int countEmptyPiles = 0;
            foreach (PileOfCards pile in this.supplyPiles)
            {
                if (pile.IsEmpty)
                {
                    if (pile.IsType(Cards.Province) ||
                        pile.IsType(Cards.Colony))
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
            
            this.gameLog.StartGame(this);

            while (!IsVictoryConditionReached())
            {
                this.hasCurrentPlayerGainedCard = false;
                this.doesCurrentPlayerNeedOutpostTurn = false;

                if (this.players.BeginningOfRound)
                {
                    this.gameLog.BeginRound(this.players.CurrentPlayer);
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

                this.gameLog.EndRound(this);
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

        static public int turnTotalCount = 0;

        public void PlayTurn(PlayerState currentPlayer)
        {
            System.Threading.Interlocked.Increment(ref turnTotalCount);
            currentPlayer.numberOfTurnsPlayed += 1;
            IPlayerAction currentPlayerAction = currentPlayer.actions;

            this.gameLog.BeginTurn(currentPlayer);
            this.gameLog.PushScope();
            currentPlayer.InitializeTurn();            

            ReturnCardsToHandAtStartOfTurn(currentPlayer);
            DoActionsQueuedFromPreviousTurn(currentPlayer);
            DoDurationActionsFromPreviousTurn(currentPlayer);            
            DoActionPhase(currentPlayer);
            DoPlayTreasures(currentPlayer);
            currentPlayer.RequestPlayerSpendCoinTokensBeforeBuyPhase(this);
            DoBuyPhase(currentPlayer);
            DoCleanupPhase(currentPlayer);

            int cardCountForNextTurn = this.doesCurrentPlayerNeedOutpostTurn ? 3 : 5;
            currentPlayer.EnterPhase(PlayPhase.DrawCards);
            currentPlayer.DrawUntilCountInHand(cardCountForNextTurn);
            currentPlayer.EnterPhase(PlayPhase.NotMyTurn);

            this.gameLog.PopScope();
            this.gameLog.EndTurn(currentPlayer);            
        }

        private void ReturnCardsToHandAtStartOfTurn(PlayerState currentPlayer)
        {
            currentPlayer.MoveCardsFromPreviousTurnIntoHand(this);
        }

        private void DoActionsQueuedFromPreviousTurn(PlayerState currentPlayer)
        {
            foreach (GameStateMethod method in currentPlayer.actionsToExecuteAtBeginningOfNextTurn)
            {
                method(currentPlayer, this);
            }
            currentPlayer.actionsToExecuteAtBeginningOfNextTurn.Clear();
        }

        private void DoDurationActionsFromPreviousTurn(PlayerState currentPlayer)
        {
            if (currentPlayer.durationCards.Any)
            {
                foreach (Card card in currentPlayer.durationCards)
                {
                    this.gameLog.ReceivedDurationEffectFrom(currentPlayer, card);
                    this.gameLog.PushScope();
                    card.DoSpecializedDurationActionAtBeginningOfTurn(currentPlayer, this);
                    this.gameLog.PopScope();
                }
            }

            currentPlayer.MoveDurationCardsToInPlay();         
        }

        private void DoActionPhase(PlayerState currentPlayer)
        {
            currentPlayer.EnterPhase(PlayPhase.Action);            
            while (currentPlayer.AvailableActions > 0)
            {
                currentPlayer.turnCounters.RemoveAction();

                if (!currentPlayer.RequestPlayerPlayActionFromHand(this, Delegates.IsActionCardPredicate, isOptional: true))
                {
                    break;
                }                
            }
        }

        private bool CardAvailableForPurchaseForCurrentPlayer(Card card)
        {
            PlayerState currentPlayer = this.players.CurrentPlayer;
            return currentPlayer.AvailableCoins >= card.CurrentCoinCost(currentPlayer) &&
                   currentPlayer.AvailablePotions >= card.potionCost &&
                   this.CardGameSubset.HasCard(card) &&
                   this.GetPile(card).Any &&
                   !card.IsRestrictedFromBuy(currentPlayer, this) &&
                   !currentPlayer.turnCounters.cardsBannedFromPurchase.Contains(card);
        }

        private void DoBuyPhase(PlayerState currentPlayer)
        {
            currentPlayer.EnterPhase(PlayPhase.Buy);
            while (currentPlayer.turnCounters.AvailableBuys > 0)
            {
                Card cardType = currentPlayer.actions.GetCardFromSupplyToBuy(this, CardAvailableForPurchaseForCurrentPlayer);
                if (cardType == null)
                {
                    return;
                }

                if (!CardAvailableForPurchaseForCurrentPlayer(cardType))
                {
                    throw new Exception("Tried to buy card that didn't meet criteria");
                }

                Card boughtCard = this.PlayerGainCardFromSupply(cardType, currentPlayer, DeckPlacement.Discard, GainReason.Buy);
                if (boughtCard == null)
                {
                    return;
                }

                currentPlayer.cardBeingBought = boughtCard;
                
                int embargoCount = this.pileEmbargoTokenCount[boughtCard];
                for (int i = 0; i < embargoCount; ++i)
                {
                    currentPlayer.GainCardFromSupply(Cards.Curse, this);                    
                }

                currentPlayer.turnCounters.RemoveCoins(boughtCard.CurrentCoinCost(currentPlayer));
                currentPlayer.turnCounters.RemovePotions(boughtCard.potionCost);
                currentPlayer.turnCounters.RemoveBuy();

                if (boughtCard.canOverpay)
                {
                    currentPlayer.RequestPlayerOverpayForCard(boughtCard, this);
                }

                if (currentPlayer.ownsCardWithSpecializedActionOnBuyWhileInPlay)
                {
                    foreach (Card cardInPlay in currentPlayer.CardsInPlay)
                    {
                        gameLog.PushScope();
                        cardInPlay.DoSpecializedActionOnBuyWhileInPlay(currentPlayer, this, boughtCard);
                        gameLog.PopScope();
                    }
                }

                currentPlayer.cardBeingBought = null;
            }
        }

        private void DoCleanupPhase(PlayerState currentPlayer)
        {
            currentPlayer.EnterPhase(PlayPhase.Cleanup);

            if (currentPlayer.ownsCardThatHasSpecializedCleanupAtStartOfCleanup)
            {                
                currentPlayer.cardsInPlayAtBeginningOfCleanupPhase.CopyFrom(currentPlayer.cardsPlayed);
                foreach (Card cardInPlay in currentPlayer.cardsInPlayAtBeginningOfCleanupPhase)
                {
                    currentPlayer.cardBeingCleanedUp = cardInPlay;
                    cardInPlay.DoSpecializedCleanupAtStartOfCleanup(currentPlayer, this);
                    currentPlayer.cardBeingCleanedUp = null;
                }
                currentPlayer.cardsInPlayAtBeginningOfCleanupPhase.Clear();
            }

            currentPlayer.CleanupCardsToDiscard(this);
        }

        internal void DoPlayTreasures(PlayerState currentPlayer)
        {
            currentPlayer.EnterPhase(PlayPhase.PlayTreasure);
            while (true)
            {
                Card cardTypeToPlay = currentPlayer.actions.GetTreasureFromHandToPlay(this, acceptableCard => true, isOptional:true);
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

        public PileOfCards GetSupplyPile(Card cardType)
        {
            return GetPile(this.supplyPiles, cardType);
        }

        public PileOfCards GetPile(Card cardType)
        {
            return this.mapCardToPile[cardType];
        }

        public PileOfCards GetPileBuilder(Card cardType)
        {
            var result = GetPile(this.supplyPiles, cardType);
            if (result != null)
                return result;

            result = GetPile(this.nonSupplyPiles, cardType);
            
            return result;
        }

        private static PileOfCards GetPile(PileOfCards[] piles, Card cardType)
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

        public Card PlayerGainCardFromSupply(Card cardType, PlayerState playerState, DeckPlacement defaultLocation = DeckPlacement.Discard, GainReason gainReason = GainReason.Gain)
        {            
            PileOfCards pile = this.GetPile(cardType);
            if (pile == null)
            {
                return null;
            }

            if (GetPile(this.supplyPiles, cardType) != null)
                this.hasPileEverBeenGained[pile] = true;            

            Card card = pile.DrawCardFromTop();
            if (card == null)
            {
                return null;
            }

            playerState.GainCard(this, card, DeckPlacement.Supply, defaultLocation, gainReason);

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

        internal bool DoesGameHaveCard(Card card)
        {
            return this.CardGameSubset.HasCard(card);
        }

        internal int CountOfDifferentTreasuresInTrash()
        {
            return this.trash.Where(card => card.isTreasure).GroupBy(card => card).Count();
        }
        
    }          
}

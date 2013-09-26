using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{            
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

            this.GainStartingCards(gameConfig.StartingDeck);            

            this.players.AllPlayersDrawInitialCards();         
        }
        
        private void GainStartingCards(IEnumerable<CardCountPair> pairs)
        {
            foreach (PlayerState player in this.players.AllPlayers)
            {
                foreach (CardCountPair pair in pairs)
                {
                    player.GainCardsFromSupply(this, pair.Card.GetType(), pair.Count);
                }
            }
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

            this.gameLog.StartGame(this);

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

        public void PlayTurn(PlayerState currentPlayer)
        {
            currentPlayer.numberOfTurnsPlayed += 1;
            IPlayerAction currentPlayerAction = currentPlayer.actions;

            this.gameLog.BeginTurn(currentPlayer);
            this.gameLog.PushScope();
            currentPlayer.InitializeTurn();
            currentPlayerAction.BeginTurn();

            ReturnCardsToHandAtStartOfTurn(currentPlayer);
            DoDurationActionsFromPreviousTurn(currentPlayer);
            DoActionPhase(currentPlayer);
            DoPlayTreasures(currentPlayer);
            currentPlayer.RequestPlayerSpendCoinTokensBeforeBuyPhase(this);
            DoBuyPhase(currentPlayer);
            DoCleanupPhase(currentPlayer);

            int cardCountForNextTurn = this.doesCurrentPlayerNeedOutpostTurn ? 3 : 5;
            currentPlayer.DrawUntilCountInHand(cardCountForNextTurn);
            currentPlayer.playPhase = PlayPhase.NotMyTurn;

            currentPlayerAction.EndTurn();            
            this.gameLog.EndTurn(currentPlayer);
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

                if (!currentPlayer.RequestPlayerPlayActionFromHand(this, acceptableCard => true, isOptional: true))
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

            currentPlayer.cardsInPlayAtBeginningOfCleanupPhase.CopyFrom(currentPlayer.cardsPlayed);

            foreach (Card cardInPlay in currentPlayer.cardsInPlayAtBeginningOfCleanupPhase)
            {
                cardInPlay.DoSpecializedCleanupAtStartOfCleanup(currentPlayer, this);
            }

            currentPlayer.cardsInPlayAtBeginningOfCleanupPhase.Clear();

            currentPlayer.CleanupCardsToDiscard(this);
        }

        internal void DoPlayTreasures(PlayerState currentPlayer)
        {
            currentPlayer.playPhase = PlayPhase.PlayTreasure;
            while (true)
            {
                Type cardTypeToPlay = currentPlayer.actions.GetTreasureFromHandToPlay(this, acceptableCard => true, isOptional:true);
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

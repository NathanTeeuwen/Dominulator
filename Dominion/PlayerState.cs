using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class PlayerState
    {
        internal int numberOfTurnsPlayed;
        
        internal readonly int playerIndex;
        internal readonly IPlayerAction actions;
        internal readonly IGameLog gameLog;
        
        internal PlayPhase playPhase;
        internal Random random;                

        internal PlayerTurnCounters turnCounters = new PlayerTurnCounters();
        
        internal bool ownsCardThatMightProvideDiscountWhileInPlay;
        internal bool ownsCardThatHasSpecializedCleanupAtStartOfCleanup;
        internal bool ownsCardWithSpecializedActionOnBuyWhileInPlay;
        internal bool ownsCardWithSpecializedActionOnTrashWhileInHand;

        // all of the cards the player owns.  Always move from one list to the other
        internal ListOfCards deck = new ListOfCards();
        internal BagOfCards discard = new BagOfCards();        
        internal ListOfCards cardsBeingPlayed = new ListOfCards();  // a stack for recursion
        internal BagOfCards cardsBeingRevealed = new BagOfCards();
        internal BagOfCards hand = new BagOfCards();        
        internal BagOfCards cardsPlayed = new BagOfCards();
        internal BagOfCards durationCards = new BagOfCards();
        internal BagOfCards cardsToReturnToHandAtStartOfTurn = new BagOfCards();
        internal Card cardToPass = null;
        internal Card cardBeingDiscarded = null;
        internal BagOfCards islandMat = new BagOfCards();
        internal BagOfCards nativeVillageMat = new BagOfCards();

        internal List<Action> actionsToExecuteAtBeginningOfNextTurn = new List<Action>();

        // expose information for use by strategies
        public IPlayerAction Actions { get { return this.actions; } }
        public int AvailableCoins { get { return this.turnCounters.AvailableCoins; } }
        public int AvailableCoinTokens { get { return this.turnCounters.AvailableCoinTokens; } }
        public int AvailableActions { get { return this.turnCounters.AvailableActions; } }
        public int AvailableBuys { get { return this.turnCounters.AvailableBuys; } }        
        public BagOfCards Hand { get { return this.hand; } }
        public BagOfCards CardsBeingRevealed { get { return this.cardsBeingRevealed; } }
        public BagOfCards Discard { get { return this.discard; } }
        public int TurnNumber { get { return this.numberOfTurnsPlayed; } }
        public Card CurrentCardBeingPlayed { get { return this.cardsBeingPlayed.TopCard(); } }
        public int PlayerIndex { get { return this.playerIndex; } }

        public int ExpectedCoinValueAtEndOfTurn { get { return this.AvailableCoins + this.hand.Where(card => card.isTreasure).Select(card => card.plusCoin).Sum(); } }

        // counters and duplicates.
        internal BagOfCards cardsInPlayAtBeginningOfCleanupPhase = new BagOfCards();

        // persistent Counters
        internal int victoryTokenCount;
        internal int pirateShipTokenCount;

        internal PlayerState(IPlayerAction actions, int playerIndex, IGameLog gameLog, Random random)
        {
            this.gameLog = gameLog;
            this.actions = actions;
            this.playPhase = PlayPhase.NotMyTurn;
            this.random = random;
            this.playerIndex = playerIndex;
        }

        internal void InitializeTurn()
        {            
            this.turnCounters.InitializeTurn();
        }        

        public int TotalScore()
        {
            return this.AllOwnedCards.Where(card => card.isVictory).Select(card => card.VictoryPoints(this)).Sum() +
                   this.victoryTokenCount -
                   this.AllOwnedCards.Where(card => card.isCurse).Count();
        }

        internal void DrawUntilCountInHand(int totalCount)
        {
            while (hand.Count < totalCount)
            {
                if (!DrawOneCardIntoHand())
                {
                    return;
                }
            }
        }

        internal void DrawAdditionalCardsIntoHand(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                if (!DrawOneCardIntoHand())
                {
                    return;
                }
            }
        }

        internal Card TrashCardFromTopOfDeck(GameState gameState)
        {
            Card card = DrawOneCard();
            if (card == null)
            {
                return null;
            }

            MoveCardToTrash(card, gameState);

            return card;
        }

        internal bool DrawOneCardIntoHand()
        {
            Card card = this.DrawOneCard();
            if (card == null)
            {
                return false;
            }

            this.gameLog.DrewCardIntoHand(this, card);
            this.hand.AddCard(card);

            return true;
        }

        private Card DrawOneCard()
        {
            if (this.deck.IsEmpty && !this.discard.IsEmpty)
            {
                this.gameLog.ReshuffledDiscardIntoDeck(this);
                TriggerShuffleOfDiscardIntoDeck();
            }

            Card card = this.deck.DrawCardFromTop();            
            return card;
        }

        internal void RevealCardsFromDiscard(int cardCount, CardPredicate cardPredicate)
        {
            for (int i = 0; i < cardCount; ++i)
            {
                // warning, n^2 algorithm here.
                Card card = this.discard.RemoveCard(cardPredicate);
                if (card == null)
                {
                    throw new System.Exception("Could not reveal needed number of cards from discard");
                }
                RevealCard(card, DeckPlacement.Discard);                
                this.cardsBeingRevealed.AddCard(card);
            }
        }

        internal void RevealCardsFromDeck(int cardCount)
        {
            for (int i = 0; i < cardCount; ++i)
            {
                if (this.DrawAndRevealOneCardFromDeck() == null)
                {
                    break;
                }
            }
        }

        internal Card LookAtBottomCardFromDeck()
        {
            if (this.deck.IsEmpty)
                this.TriggerShuffleOfDiscardIntoDeck();

            return this.deck.BottomCard();
        }

        internal void MoveCardFromBottomOfDeckToTop()
        {
            this.deck.MoveBottomCardToTop();
        }

        internal void LookAtCardsFromDeck(int cardCount)
        {
            this.RevealCardsFromDeck(cardCount);
        }

        internal void RevealCard(Card card, DeckPlacement source)
        {
            this.gameLog.PlayerRevealedCard(this, card, DeckPlacement.TopOfDeck);            
        }

        internal Card DrawAndLookAtOneCardFromDeck()
        {
            // TODO: separate reveal from look
            return DrawAndRevealOneCardFromDeck();
        }

        internal Card DrawAndRevealOneCardFromDeck()
        {
            Card card = this.DrawOneCard();            
            if (card != null)
            {
                RevealCard(card, DeckPlacement.TopOfDeck);
                this.cardsBeingRevealed.AddCard(card);
            }
            return card;
        }        

        internal void RevealHand()
        {

        }

        public int CountCardsPlayedThisTurn
        {
            get
            {
                return this.cardsBeingPlayed.Count + this.cardsPlayed.Count + this.durationCards.Count;
            }
        }

        internal int CountCardsInPlay
        {
            get
            {
                return this.cardsBeingPlayed.Where(card => card != null).Count() + this.cardsPlayed.Count;
            }
        }

        internal void AddBuys(int actionAmount)
        {
            this.turnCounters.AddBuys(this, actionAmount);
        }

        internal void AddActions(int actionAmount)
        {
            this.turnCounters.AddActions(this, actionAmount);
        }

        internal void AddCoins(int coinAmount)
        {
            this.turnCounters.AddCoins(this, coinAmount);
        }

        internal void AddCoinTokens(int coinAmount)
        {
            this.turnCounters.AddCoinTokens(this, coinAmount);
        }

        internal void DoPlayAction(Card currentCard, GameState gameState, int countTimes = 1)
        {
            if (!currentCard.isAction)
            {
                throw new Exception("Can't play a card that isn't a action");
            }

            this.gameLog.PlayedCard(this, currentCard);
            this.gameLog.PushScope();
            this.cardsBeingPlayed.AddCardToTop(currentCard);
            
            for (int i = 0; i < countTimes; ++i)
            {
                this.AddActions(currentCard.plusAction);                
                this.AddBuys(currentCard.plusBuy);
                this.AddCoins(currentCard.plusCoin);
                this.victoryTokenCount += currentCard.plusVictoryToken;
                this.DrawAdditionalCardsIntoHand(currentCard.plusCard);

                if (currentCard.isAttack && currentCard.isAttackBeforeAction)
                {
                    AttackOtherPlayers(gameState, currentCard.DoSpecializedAttack);
                }
                
                currentCard.DoSpecializedAction(gameState.players.CurrentPlayer, gameState);
                
                if (currentCard.isAttack && !currentCard.attackDependsOnPlayerChoice && !currentCard.isAttackBeforeAction)
                {
                    AttackOtherPlayers(gameState, currentCard.DoSpecializedAttack);                    
                }
            }

            CardHasBeenPlayed();

            this.gameLog.PopScope();
        }

        internal delegate void AttackAction(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState);

        internal void AttackOtherPlayers(GameState gameState, AttackAction action)
        {
            foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
            {
                if (otherPlayer.IsAffectedByAttacks(gameState))
                {
                    action(gameState.players.CurrentPlayer, otherPlayer, gameState);
                }
            }
        }

        private void CardHasBeenPlayed()
        {
            Card cardAfterPlay = this.cardsBeingPlayed.DrawCardFromTop();
            if (cardAfterPlay != null)
            {
                if (cardAfterPlay.isDuration)
                {
                    this.durationCards.AddCard(cardAfterPlay);
                }
                else
                {
                    this.cardsPlayed.AddCard(cardAfterPlay);
                }
            }
        }

        internal void DoPlayTreasure(Card currentCard, GameState gameState)
        {            
            if (!currentCard.isTreasure)
            {
                throw new Exception("Can't play a card that isn't a treasure");
            }

            this.gameLog.PlayedCard(this, currentCard);
            this.gameLog.PushScope();
            this.cardsBeingPlayed.AddCardToTop(currentCard);

            this.AddBuys(currentCard.plusBuy);
            this.AddCoins(currentCard.plusCoin);
            if (currentCard.Is<CardTypes.Copper>())
            {
                this.AddCoins(this.turnCounters.copperAdditionalValue);
            }

            currentCard.DoSpecializedAction(gameState.players.CurrentPlayer, gameState);

            CardHasBeenPlayed();

            this.gameLog.PopScope();
        }        

        private void PlaceCardFromPlacement(CardPlacementPair pair, GameState gameState)
        {
            gameLog.CardWentToLocation(pair.placement);
            switch (pair.placement)
            {
                case DeckPlacement.Discard: this.discard.AddCard(pair.card); break;
                case DeckPlacement.Hand: this.hand.AddCard(pair.card); break;
                case DeckPlacement.Trash: this.MoveCardToTrash(pair.card, gameState); break;
                case DeckPlacement.Play: this.PlayCard(pair.card, gameState); break;
                case DeckPlacement.TopOfDeck: this.deck.AddCardToTop(pair.card); break;
                case DeckPlacement.None: throw new NotImplementedException();
                default: throw new Exception("Invalid case");
            }
        }

        internal void PlayCard(Card card, GameState gameState)
        {            
            if (card.isTreasure)
            {                
                this.DoPlayTreasure(card, gameState);
            }
            else if (card.isAction)
            {
                this.DoPlayAction(card, gameState);
            }
            else
            {
                throw new Exception("Could not play card");
            }
        }

        internal Card TrashCardFromHandOfType<T>(GameState gameState, bool guaranteeInHand)
            where T : Card, new()
        {
            return TrashCardFromHandOfType(gameState, Card.Type<T>(), guaranteeInHand);
        }

        internal Card TrashCardFromHandOfType(GameState gameState, Card cardType, bool guaranteeInHand)            
        {            
            Card currentCard = this.RemoveCardFromHand(cardType);
            if (currentCard == null)
            {
                if (!guaranteeInHand)
                {
                    return null;
                }
                else
                {
                    throw new Exception("Player tried to trash a card that wasn't available in hand.");
                }
            }

            MoveCardToTrash(currentCard, gameState);

            return currentCard;
        }

        internal Card TrashCardFromDiscardOfType(GameState gameState, Card cardType, bool guaranteeInDiscard)
        {
            Card currentCard = this.RemoveCardFromDiscard(cardType);
            if (currentCard == null)
            {
                if (!guaranteeInDiscard)
                {
                    return null;
                }
                else
                {
                    throw new Exception("Player tried to trash a card that wasn't available in the discard.");
                }
            }

            MoveCardToTrash(currentCard, gameState);

            return currentCard;
        }

        internal void TrashHand(GameState gameState)
        {
            foreach (Card card in this.hand)
            {
                MoveCardToTrash(card, gameState);
            }

            this.hand.Clear();
        }

        internal bool MoveCardFromPlayToPile(GameState gameState)
        {
            bool wasReturned = false;
            Card cardInPlay = this.cardsBeingPlayed.DrawCardFromTop();
            if (cardInPlay != null)
            {
                PileOfCards pile = gameState.GetPile(cardInPlay);
                pile.AddCardToTop(cardInPlay);
                wasReturned = true;
                this.gameLog.PlayerReturnedCardToPile(this, cardInPlay);
            }

            this.cardsBeingPlayed.AddCardToTop(null);
            return wasReturned;
        }

        internal bool MoveCardFromPlayToTrash(GameState gameState)
        {
            bool wasTrashed = false;
            Card cardInPlay = this.cardsBeingPlayed.DrawCardFromTop();
            if (cardInPlay != null)
            {
                MoveCardToTrash(cardInPlay, gameState);
                wasTrashed = true;
            }

            this.cardsBeingPlayed.AddCardToTop(null);
            return wasTrashed;
        }

        internal void MoveCardBeingDiscardedToTrash(GameState gameState)
        {
            if (this.cardBeingDiscarded != null)
            {
                MoveCardToTrash(this.cardBeingDiscarded, gameState);
                this.cardBeingDiscarded = null;
            }
        }

        internal void MoveCardToTrash(Card card, GameState gameState)
        {            
            this.gameLog.PlayerTrashedCard(this, card);
            gameState.trash.AddCard(card);
            this.gameLog.PushScope();

            card.DoSpecializedTrash(gameState.players.CurrentPlayer, gameState);

            // cards in hand react to trashing.
            if (this.ownsCardWithSpecializedActionOnTrashWhileInHand)
            {
                bool stateHasChanged = true;
                while (stateHasChanged)
                {
                    stateHasChanged = false;
                    foreach (Card cardInHand in this.hand)
                    {
                        stateHasChanged = cardInHand.DoSpecializedActionOnTrashWhileInHand(this, gameState, cardInHand);
                        if (stateHasChanged)
                            break;
                    }
                }
            }            
            
            this.gameLog.PopScope();            
        }

        internal Card RemoveCardFromDiscard(Card cardType)
        {
            return this.discard.RemoveCard(cardType);
        }

        internal Card RemoveCardFromHand(Card cardType)
        {
            return this.hand.RemoveCard(cardType);
        }

        internal void DiscardHandDownToCount(GameState gameState, int count)
        {
            while (this.hand.Count > count)
            {
                this.RequestPlayerDiscardCardFromHand(gameState, acceptableCard => true, isOptional: false);
            }
        }

        internal void DiscardHand(GameState gameState)
        {            
            this.MoveAllCardsToDiscard(this.hand, gameState, DeckPlacement.Hand);
        }

        internal Card DiscardCardFromTopOfDeck()
        {
            Card card = this.deck.DrawCardFromTop();
            if (card != null)
            {
                this.discard.AddCard(card);
            }

            return card;
        }

        internal bool DiscardCardFromHand(GameState gameState, Card card)
        {       
            if (!this.hand.HasCard(card))
            {
                return false;
            }            

            this.MoveCardFromHandToDiscard(card, gameState);            

            return true;        
        }

        internal void MoveCardFromPlayedCardToIslandMat(Card card)
        {
            Card removedCard = this.cardsPlayed.RemoveCard(card);
            if (removedCard != null)
            {
                this.islandMat.AddCard(removedCard);
            }
        }

        internal void MoveCardFromHandToIslandMat(Card cardType)
        {
            Card removedCard = this.Hand.RemoveCard(cardType);
            if (removedCard != null)
            {                
                this.islandMat.AddCard(removedCard);
            }
        }

        internal void MoveNativeVillageMatToHand()
        {
            while (this.nativeVillageMat.Any())
            {
                Card card = this.nativeVillageMat.RemoveCard();
                this.hand.AddCard(card);
            }
        }

        internal void PutOnNativeVillageMatCardFromTopOfDeck()
        {
            Card card = this.DrawOneCard();
            if (card != null)
            {
                this.nativeVillageMat.AddCard(card);
            }
        }

        internal void MoveCardFromPlayedCardToNativeVillageMatt(Card card)
        {
            Card removedCard = this.cardsPlayed.RemoveCard(card);
            if (removedCard != null)
            {
                this.nativeVillageMat.AddCard(removedCard);
            }
        }

        internal void RequestPlayerSpendCoinTokensBeforeBuyPhase(GameState gameState)
        {
            int coinToSpend = this.actions.GetCoinAmountToSpendInBuyPhase(gameState);
            if (coinToSpend > this.AvailableCoinTokens || coinToSpend < 0)
                throw new Exception("Can not spend that many coins");
            this.AddCoinTokens(-coinToSpend);
            this.gameLog.PushScope();
            this.AddCoins(coinToSpend);
            this.gameLog.PopScope();
        }

        internal Card RequestPlayerChooseCardToRemoveFromHandForPlay(GameState gameState, CardPredicate acceptableCard, bool isTreasure, bool isAction, bool isOptional)
        {
            if (!(isTreasure ^ isAction))
                throw new System.InvalidOperationException("Must be action or treasure");
            
            if (!this.hand.HasCard(acceptableCard))
            {
                return null;
            }

            Card cardTypeToPlay = isTreasure ? 
                this.actions.GetTreasureFromHandToPlay(gameState, acceptableCard, isOptional) :
                this.actions.GetCardFromHandToPlay(gameState, acceptableCard, isOptional);

            if (cardTypeToPlay == null)
            {
                if (!isOptional)
                {
                    throw new Exception("Must choose an action to play");
                }
                return null;
            }

            Card currentCard = this.RemoveCardFromHand(cardTypeToPlay);
            if (currentCard == null)
            {
                throw new Exception("Player tried to remove a card that wasn't available in hand");
            }

            return currentCard;
        }

        private static bool IsCardTreasure(Card card)
        {
            return card.isTreasure;
        }

        internal PileOfCards RequestPlayerChooseCardPileFromSupply(GameState gameState)
        {
            Card cardType = this.actions.GetCardPileFromSupply(gameState);

            PileOfCards pile = gameState.GetPile(cardType);
            if (pile == null)
            {
                throw new Exception("Must choose pile from supply");
            }

            return pile;
        }

        internal bool RequestPlayerPlayActionFromHand(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            Card cardToPlay = RequestPlayerChooseCardToRemoveFromHandForPlay(gameState, acceptableCard, isTreasure: false, isAction: true, isOptional: isOptional);

            if (cardToPlay != null)
            {
                this.DoPlayAction(cardToPlay, gameState);                
            }

            return true;
        }

        internal Card RequestPlayerGainCardFromSupply(GameState gameState, CardPredicate acceptableCard, string description, bool isOptional = false, DeckPlacement defaultLocation = DeckPlacement.Discard)
        {
            return RequestPlayerGainCardFromSupply(gameState, this, acceptableCard, description, isOptional, defaultLocation);
        }

        internal Card RequestPlayerGainCardFromSupply(GameState gameState, PlayerState playerGainingCard, CardPredicate acceptableCard, string description, bool isOptional = false, DeckPlacement defaultLocation = DeckPlacement.Discard)
        {
            PileOfCards exampleCard = gameState.supplyPiles.Where(cardPile => !cardPile.IsEmpty && acceptableCard(cardPile.TopCard())).FirstOrDefault();
            bool hasCardOfCost = exampleCard != null;
            if (!hasCardOfCost)
            {
                return null;
            }

            // how do you know which player you are gaining for?
            Card cardType = this.actions.GetCardFromSupplyToGain(gameState, acceptableCard, isOptional);
            if (cardType == null)
            {
                if (isOptional)
                {
                    return null;
                }
                throw new Exception("Must gain a card where " + description);
            }

            Card gainedCard = gameState.PlayerGainCardFromSupply(cardType, playerGainingCard, defaultLocation);
            if (gainedCard == null)
            {
                throw new Exception("Card specified can not be gained");
            }

            if (!acceptableCard(gainedCard))
            {
                throw new Exception("Card does not meet constraint: " + description);
            }

            return gainedCard;
        }

        internal Card[] RequestPlayerTrashCardsFromHand(GameState gameState, int cardCount, bool isOptional, bool allOrNone = false)
        {
            var trashedCards = new List<Card>(cardCount);
            CardPredicate acceptableCardsToTrash = card => true;
            int countCardTrashed = 0;
            while (countCardTrashed < cardCount)
            {
                Card trashedCard = this.RequestPlayerTrashCardFromHand(gameState, acceptableCardsToTrash, isOptional);
                if (trashedCard == null)
                {
                    break;
                }

                if (allOrNone == true)
                    isOptional = false;

                trashedCards.Add(trashedCard);
            }

            return trashedCards.ToArray();
        }

        internal Card RequestPlayerTrashCardFromHandAndGainCard(
            GameState gameState,
            CardPredicate acceptableCardsToTrash,
            CostConstraint costConstraint,
            int cost,
            CardRelativeCost cardRelativeCost,
            bool isOptionalToTrash = false,
            bool isOptionalToGain = false,
            DeckPlacement defaultLocation = DeckPlacement.Discard)
        {
            Card trashedCard = this.RequestPlayerTrashCardFromHand(gameState, acceptableCardsToTrash, isOptionalToTrash);
            if (trashedCard != null)
            {
                int cardCost = cardRelativeCost == CardRelativeCost.RelativeCost ? trashedCard.CurrentCoinCost(this) + cost :
                                   cardRelativeCost == CardRelativeCost.AbsoluteCost ? cost : 0;

                CardPredicate cardPredicate;
                if (costConstraint == CostConstraint.Exactly)
                {
                    cardPredicate = acceptableCard => acceptableCard.CurrentCoinCost(this) == cardCost;
                }
                else if (costConstraint == CostConstraint.UpTo)
                {
                    cardPredicate = acceptableCard => acceptableCard.CurrentCoinCost(this) <= cardCost;
                }
                else
                {
                    throw new Exception("Invalid operation");
                }

                return this.RequestPlayerGainCardFromSupply(
                    gameState,
                    cardPredicate,
                    "",
                    isOptionalToGain,
                    defaultLocation);
            }

            return null;
        }

        internal Card RequestPlayerTrashCardFromHand(GameState gameState, CardPredicate acceptableCardsToTrash, bool isOptional)
        {
            if (!this.hand.HasCard(acceptableCardsToTrash))
            {
                return null;
            }
            Card cardTypeToTrash = this.actions.GetCardFromHandToTrash(gameState, acceptableCardsToTrash, isOptional);
            if (cardTypeToTrash == null)
            {
                if (isOptional)
                {
                    return null;
                }
                else
                {
                    throw new Exception("Player must choose a card to trash");
                }
            }

            return this.TrashCardFromHandOfType(gameState, cardTypeToTrash, guaranteeInHand: true);
        }

        internal Card RequestPlayerTrashCardFromHandOrDiscard(GameState gameState, CardPredicate acceptableCardsToTrash, bool isOptional)
        {
            if (!this.hand.HasCard(acceptableCardsToTrash) &&
                !this.discard.HasCard(acceptableCardsToTrash))
            {
                return null;
            }

            DeckPlacement deckPlacement;
            Card cardTypeToTrash = this.actions.GetCardFromHandOrDiscardToTrash(gameState, acceptableCardsToTrash, isOptional, out deckPlacement);
            if (cardTypeToTrash == null)
            {
                if (isOptional)
                {
                    return null;
                }
                else
                {
                    throw new Exception("Player must choose a card to trash");
                }
            }

            if (deckPlacement == DeckPlacement.Hand)
            {
                return this.TrashCardFromHandOfType(gameState, cardTypeToTrash, guaranteeInHand: true);
            }
            else if (deckPlacement == DeckPlacement.Discard)
            {
                return this.TrashCardFromDiscardOfType(gameState, cardTypeToTrash, guaranteeInDiscard: true);
            }
            else
            {
                throw new Exception("Must trash from hand or discard");
            }
        }

        internal int RequestPlayerDiscardCardsFromHand(GameState gameState, int count, bool isOptional)
        {
            int cardDiscardedCount = 0;
            while (!this.hand.IsEmpty && cardDiscardedCount < count)
            {
                if (!this.RequestPlayerDiscardCardFromHand(gameState, acceptableCard => true, isOptional))
                {
                    break;
                }
                ++cardDiscardedCount;
            }

            return cardDiscardedCount;
        }

        internal void RequestPlayerDiscardDownToCountInHand(GameState gameState, int count)
        {
            while (this.hand.Count > count)
            {
                this.RequestPlayerDiscardCardFromHand(gameState, acceptableCard => true, isOptional: false);
            }
        }

        internal bool RequestPlayerDiscardCardFromHand(GameState gameState, CardPredicate acceptableCardsToDiscard, bool isOptional)
        {
            if (!this.hand.HasCard(acceptableCardsToDiscard))
            {
                return false;
            }

            Card cardTypeToDiscard = this.actions.GetCardFromHandToDiscard(gameState, acceptableCardsToDiscard, this, isOptional);
            if (cardTypeToDiscard == null)
            {
                if (isOptional)
                {
                    return false;
                }
                else
                {
                    throw new Exception("Player must choose a card to discard");
                }
            }
            else
            {
                if (gameState.GetPile(cardTypeToDiscard) != null &&  // TODO: this currently can not find ruins ... rework this method so a card is returned instead of a type.
                    !acceptableCardsToDiscard( cardTypeToDiscard))
                    throw new Exception("Card does not meet constraint: ");
            }

            this.MoveCardFromHandToDiscard(cardTypeToDiscard, gameState);            

            return true;
        }

        internal bool RequestPlayerDiscardCardFromOtherPlayersHand(GameState gameState, PlayerState otherPlayer)
        {
            if (!otherPlayer.hand.Any)
            {
                return false;
            }

            Card cardTypeToDiscard = this.actions.GetCardFromOtherPlayersHandToDiscard(gameState, otherPlayer);
            if (cardTypeToDiscard == null)
            {                
                {
                    throw new Exception("Player must choose a card to discard");
                }
            }
            
            otherPlayer.MoveCardFromHandToDiscard(cardTypeToDiscard, gameState);

            return true;
        }

        internal void RequestPlayerPutRevealedCardsBackOnDeck(GameState gameState)
        {
            while (this.cardsBeingRevealed.Any)
            {
                Card cardToPutOnTop = this.actions.GetCardFromRevealedCardsToPutOnDeck(gameState, this);
                if (cardToPutOnTop == null)
                {
                    throw new Exception("Player must choose a card to put on top of deck");
                }

                this.MoveRevealedCardToTopOfDeck(cardToPutOnTop);
            }
        }

        internal void RequestPlayerTrashLookedAtCard(GameState gameState)
        {
            RequestPlayerTrashRevealedCard(gameState);
        }

        internal void RequestPlayerTrashRevealedCard(GameState gameState)
        {
            if (this.cardsBeingRevealed.Any)
            {
                Card cardtoTrash = this.actions.GetCardFromRevealedCardsToTrash(gameState, this, acceptableCard => true);
                if (cardtoTrash.Is<CardTypes.Salvager>())
                { int i = 0; i++; }
                if (cardtoTrash == null)
                {
                    throw new Exception("Player must choose a card to trash");
                }

                this.MoveRevealedCardToTrash(cardtoTrash, gameState);
            }
        }

        internal void RequestPlayerDiscardRevealedCard(GameState gameState)
        {
            if (this.cardsBeingRevealed.Any)
            {
                Card cardToDiscard = this.actions.GetCardFromRevealedCardsToDiscard(gameState, this);
                if (cardToDiscard == null)
                {
                    throw new Exception("Player must choose a card to trash");
                }
                
                this.MoveRevealedCardToDiscard(cardToDiscard, gameState);
            }
        }

        internal Card RequestPlayerDeferCardFromHandtoNextTurn(GameState gameState)
        {
            if (this.Hand.IsEmpty)
                return null;

            Card cardType = this.actions.GetCardFromHandToDeferToNextTurn(gameState);

            Card cardToDefer = this.hand.RemoveCard(cardType);
            this.cardsToReturnToHandAtStartOfTurn.AddCard(cardToDefer);
            this.gameLog.PlayerSetAsideCardFromHandForNextTurn(this, cardToDefer);
            return cardToDefer;
        }

        internal Card RequestPlayerTopDeckCardFromHand(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            if (!this.hand.HasCard(acceptableCard))
            {
                return null;
            }

            Card cardTypeToTopDeck = this.actions.GetCardFromHandToTopDeck(gameState, acceptableCard, isOptional);
            if (cardTypeToTopDeck == null)
            {
                if (isOptional)
                {
                    return null;
                }
                else
                {
                    throw new Exception("Player must choose a card to top deck");
                }
            }

            Card cardToTopDeck = this.hand.RemoveCard(cardTypeToTopDeck);
            if (cardToTopDeck == null)
            {
                throw new Exception("Could not remove Card From Hand");
            }

            if (!acceptableCard(cardToTopDeck))
            {
                throw new Exception("Card does not meet constraint for top deck");
            }

            this.gameLog.PlayerTopDeckedCard(this, cardToTopDeck);
            this.deck.AddCardToTop(cardToTopDeck);

            return cardToTopDeck;
        }

        internal void RequestPlayerTopDeckCardsFromPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            while (RequestPlayerTopDeckCardFromPlay(gameState, acceptableCard, isOptional) != null) ;
        }

        internal Card RequestPlayerTopDeckCardFromPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            if (!this.cardsPlayed.HasCard(acceptableCard))
            {
                return null;
            }

            Card cardTypeToTopDeck = this.actions.GetCardFromPlayToTopDeck(gameState, acceptableCard, isOptional);
            if (cardTypeToTopDeck == null)
            {
                if (isOptional)
                {
                    return null;
                }
                else
                {
                    throw new Exception("Player must choose a card to top deck");
                }
            }

            Card cardToTopDeck = this.cardsPlayed.RemoveCard(cardTypeToTopDeck);
            if (cardToTopDeck == null)
            {
                throw new Exception("Could not remove Card From Hand");
            }

            if (!acceptableCard(cardToTopDeck))
            {
                throw new Exception("Card does not meet constraint for top deck");
            }

            this.gameLog.PlayerTopDeckedCard(this, cardToTopDeck);
            this.deck.AddCardToTop(cardToTopDeck);

            return cardToTopDeck;
        }

        internal Card RequestPlayerRevealCardFromHand(CardPredicate acceptableCard, GameState gameState)
        {
            if (!this.hand.HasCard(acceptableCard))
            {
                return null;
            }

            Card cardToReveal = this.actions.GetCardFromHandToReveal(gameState, 
                card => acceptableCard(card) && this.hand.HasCard(card));
            
            if (cardToReveal != null)
            {
                Card revealedCard = RevealCardFromHand(cardToReveal, gameState);
                this.MoveRevealedCardsToHand(card => true);
                return revealedCard;
            }

            return null;
        }

        internal Card RequestPlayerTopDeckCardFromRevealed(GameState gameState, bool isOptional)
        {
            Card cardTypeToTopDeck = this.actions.GetCardFromRevealedCardsToTopDeck(gameState, this);
            if (cardTypeToTopDeck == null && !isOptional)
            {
                throw new Exception("Must choose a card to top deck");
            }

            if (cardTypeToTopDeck == null)
            {
                return null;
            }

            Card cardToTopDeck = this.cardsBeingRevealed.RemoveCard(cardTypeToTopDeck);
            if (cardToTopDeck == null)
            {
                throw new Exception("Selected a card that wasn't being revealed");
            }

            this.gameLog.PlayerTopDeckedCard(this, cardToTopDeck);
            this.deck.AddCardToTop(cardToTopDeck);
            return cardToTopDeck;
        }

        internal Card RequestPlayerTopDeckCardFromDiscard(GameState gameState, bool isOptional)
        {
            if (this.Discard.Count == 0)
                return null;

            Card cardTypeToTopDeck = this.actions.GetCardFromDiscardToTopDeck(gameState, this, isOptional);
            if (cardTypeToTopDeck == null && !isOptional)
            {
                throw new Exception("Must choose a card to top deck");
            }

            if (cardTypeToTopDeck == null)
            {
                return null;
            }

            Card cardToTopDeck = this.discard.RemoveCard(cardTypeToTopDeck);
            if (cardToTopDeck == null)
            {
                throw new Exception("Selected a card that wasn't in the discard");
            }

            this.gameLog.PlayerTopDeckedCard(this, cardToTopDeck);
            this.deck.AddCardToTop(cardToTopDeck);
            return cardToTopDeck;
        }

        internal void RequestPlayerTopDeckRevealedCardsInAnyOrder(GameState gameState)
        {
            while (this.cardsBeingRevealed.Any)
            {
                this.RequestPlayerTopDeckCardFromRevealed(gameState, isOptional: false);
            }
        }

        internal Card RequestPlayerGiveCardToPassLeft(GameState gameState)
        {
            if (this.hand.IsEmpty)
            {
                return null;
            }

            Card cardTypeToPassLeft = this.actions.GetCardFromHandToPassLeft(gameState);
            if (cardTypeToPassLeft == null)
            {
                throw new Exception("Player must choose a card to paass");
            }


            Card cardToTopDeck = this.hand.RemoveCard(cardTypeToPassLeft);
            if (cardToTopDeck == null)
            {
                throw new Exception("Could not remove Card From Hand");
            }

            return cardToTopDeck;
        }

        internal DeckPlacement RequestPlayerChooseTrashOrTopDeck(GameState gameState, Card card)
        {
            DeckPlacement choice = this.actions.ChooseBetweenTrashAndTopDeck(gameState, card);
            if (choice != DeckPlacement.TopOfDeck && choice != DeckPlacement.Trash)
                throw new Exception("Player made in invalid action choice");

            return choice;
        }

        internal PlayerActionChoice RequestPlayerChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {
            PlayerActionChoice choice = actions.ChooseBetween(gameState, acceptableChoice);

            if (!acceptableChoice(choice))
            {
                throw new Exception("Player made in invalid action choice");
            }

            return choice;
        }

        internal void RequestPlayerOverpayForCard(Card boughtCard, GameState gameState)
        {
            int overPayAmount = this.actions.GetCoinAmountToOverpayForCard(gameState, boughtCard);
            if (this.AvailableCoins < overPayAmount)
            {
                throw new Exception("Player requested to overpay by more than he can afford");
            }            
            if (overPayAmount > 0)
            {
                this.gameLog.PlayerOverpaidForCard(boughtCard, overPayAmount);
                this.gameLog.PushScope();
                this.AddCoins(-overPayAmount);
                boughtCard.OverpayOnPurchase(this, gameState, overPayAmount);
                this.gameLog.PopScope();
            }
        }

        internal void CleanupCardsToDiscard(GameState gameState)
        {
            this.MoveAllCardsToDiscard(this.cardsPlayed, gameState, DeckPlacement.Play);
            this.MoveAllCardsToDiscard(this.hand, gameState, DeckPlacement.Hand);
        }

        internal Card GuessCardTopOfDeck(GameState gameState)
        {
            Card cardType = this.actions.GuessCardTopOfDeck(gameState);
            if (cardType == null)
            {
                throw new Exception("Must name a card");
            }

            gameState.gameLog.PlayerNamedCard(this, cardType);

            return cardType;
        }

        internal Card RequestPlayerNameACard(GameState gameState)
        {
            Card cardType = this.actions.NameACard(gameState);
            if (cardType == null)
            {
                throw new Exception("Must name a card");
            }

            gameState.gameLog.PlayerNamedCard(this, cardType);
            return cardType;
        }        

        internal Card GainCardFromSupply(GameState gameState, Card cardType, DeckPlacement defaultLocation = DeckPlacement.Discard)
        {
            return gameState.PlayerGainCardFromSupply(cardType, this, defaultLocation);
        }

        internal void GainCardsFromSupply(GameState gameState, Card cardType, int count, DeckPlacement defaultLocation = DeckPlacement.Discard)
        {
            for (int i = 0; i < count; ++i)
                gameState.PlayerGainCardFromSupply(cardType, this, defaultLocation);
        }

        internal void GainCardsFromSupply<T>(GameState gameState, int count, DeckPlacement defaultLocation = DeckPlacement.Discard)
            where T: Card, new()
        {
            GainCardsFromSupply(gameState, Card.Type<T>(), count, defaultLocation);
        }

        internal bool GainCardFromSupply<T>(GameState gameState, DeckPlacement defaultLocation = DeckPlacement.Discard)
            where T: Card, new()
        {
            return gameState.PlayerGainCardFromSupply(Card.Type<T>(), this, defaultLocation:defaultLocation) != null;
        }        

        internal void GainCard(GameState gameState, Card card, DeckPlacement defaultPlacement = DeckPlacement.Discard, GainReason gainReason = GainReason.Gain)
        {
            if (gainReason == GainReason.Buy)
            {
                this.gameLog.PlayerBoughtCard(this, card);
            }
            else
            {
                this.gameLog.PlayerGainedCard(this, card);
            }

            this.gameLog.PushScope();

            bool wasCardMoved = false;
            foreach (Card cardInHand in this.Hand.ToArray())
            {
                DeckPlacement preferredPlacement = (gainReason == GainReason.Buy) ?
                    cardInHand.DoSpecializedActionOnBuyWhileInHand(this, gameState, card) : DeckPlacement.Default;

                if (!wasCardMoved && preferredPlacement == DeckPlacement.Default)
                {
                    preferredPlacement = cardInHand.DoSpecializedActionOnGainWhileInHand(this, gameState, card);
                }

                if (!wasCardMoved && preferredPlacement != DeckPlacement.Default)
                {
                    defaultPlacement = preferredPlacement;
                    wasCardMoved = true;
                }
            }

            foreach (Card cardInPlay in this.CardsInPlay)
            {
                DeckPlacement preferredPlacement = cardInPlay.DoSpecializedActionOnGainWhileInPlay(this, gameState, card);
                if (!wasCardMoved && preferredPlacement != DeckPlacement.Default)
                {
                    defaultPlacement = preferredPlacement;
                    wasCardMoved = true;
                }
            }

            // buys are also gains.
            {
                DeckPlacement preferredPlacement = card.DoSpecializedWhenGain(this, gameState);
                if (!wasCardMoved && preferredPlacement != DeckPlacement.Default)
                {
                    defaultPlacement = preferredPlacement;
                    wasCardMoved = true;
                }
            }

            if (gainReason == GainReason.Buy)
            {
                card.DoSpecializedWhenBuy(this, gameState);            
            }            
            
            this.PlaceCardFromPlacement(new CardPlacementPair(card, defaultPlacement), gameState);
            this.gameLog.PopScope();

            gameState.hasCurrentPlayerGainedCard |= true;

            this.ownsCardThatMightProvideDiscountWhileInPlay |= card.MightProvideDiscountWhileInPlay;
            this.ownsCardThatHasSpecializedCleanupAtStartOfCleanup |= card.HasSpecializedCleanupAtStartOfCleanup;
            this.ownsCardWithSpecializedActionOnBuyWhileInPlay |= card.HasSpecializedActionOnBuyWhileInPlay;
            this.ownsCardWithSpecializedActionOnTrashWhileInHand |= card.HasSpecializedActionOnTrashWhileInHand;
        }

        internal void MoveAllCards(GameState gameState, CardPredicate acceptableCard, DeckPlacement from, DeckPlacement to)
        {
            while (MoveOneCard(gameState, acceptableCard, from, to) != null)
            {
            }
        }

        internal Card MoveOneCard(GameState gameState, CardPredicate acceptableCard, DeckPlacement from, DeckPlacement to)
        {
            Card cardToMove = null;
            switch (from)
            {
                case DeckPlacement.Play: cardToMove = this.cardsPlayed.RemoveCard(acceptableCard); break;
                case DeckPlacement.Discard: cardToMove = this.discard.RemoveCard(acceptableCard); break;
                case DeckPlacement.Hand: cardToMove = this.hand.RemoveCard(acceptableCard); break;
                case DeckPlacement.Trash: cardToMove = this.hand.RemoveCard(acceptableCard); break;
                default: throw new InvalidOperationException();
            }

            if (cardToMove != null)
            {
                this.PlaceCardFromPlacement(new CardPlacementPair(cardToMove, to), gameState);
            }

            return cardToMove;
        }

        private void TriggerShuffleOfDiscardIntoDeck()
        {
            if (!this.deck.IsEmpty)
            {
                throw new Exception("Can not move discard to deck unless deck is empty");
            }
            foreach (Card card in this.discard)
            {
                deck.AddCardToTop(card);
            }

            discard.Clear();
            // TODO:  Place stash where u want it to go
            deck.Shuffle(this.random);

            // move Stash to where the user wants
        }

        private Card RevealCardFromHand(Card cardTypeToDiscard, GameState gameState)
        {
            Card cardToReveal = this.hand.RemoveCard(cardTypeToDiscard);
            if (cardToReveal == null)
            {
                throw new Exception("Could not reveal Card From Hand");
            }

            RevealCard(cardToReveal, DeckPlacement.Hand);            
            this.cardsBeingRevealed.AddCard(cardToReveal);

            return cardToReveal;
        }

        private Card MoveCardFromHandToDiscard(Card cardTypeToDiscard, GameState gameState)
        {
            Card cardToDiscard = this.hand.RemoveCard(cardTypeToDiscard);
            if (cardToDiscard == null)
            {
                throw new Exception("Could not remove Card From Hand");
            }
                        
            this.DiscardCard(cardToDiscard, gameState, DeckPlacement.Hand);

            return cardToDiscard;
        }

        internal void MoveDeckToDiscard(GameState gameState)
        {
            MoveAllCardsToDiscard(this.deck, gameState, DeckPlacement.Deck);
        }

        internal void MoveCardsFromPreviousTurnIntoHand()
        {
            foreach (Card card in this.cardsToReturnToHandAtStartOfTurn)
            {
                this.Hand.AddCard(card);
                this.gameLog.PlayerReturnedCardToHand(this, card);
            }
            this.cardsToReturnToHandAtStartOfTurn.Clear();
        }

        internal void MoveDurationCardsToInPlay()
        {
            foreach (Card card in this.durationCards)
            {
                this.cardsPlayed.AddCard(card);
            }
            this.durationCards.Clear();
        }

        internal void MoveLookedAtCardsToDiscard(GameState gameState)
        {
            this.MoveRevealedCardsToDiscard(gameState);
        }

        internal void MoveRevealedCardsToDiscard(GameState gameState)
        {
            // trigger discard effects
            MoveAllCardsToDiscard(this.cardsBeingRevealed, gameState, DeckPlacement.Revealed);
        }

        internal void MoveRevealedCardsToDiscard(CardPredicate predicate, GameState gameState)
        {            
            while (true)
            {
                Card cardFound = this.cardsBeingRevealed.RemoveCard(predicate);
                if (cardFound == null)
                {
                    break;
                }
                DiscardCard(cardFound, gameState, DeckPlacement.Revealed);
            }            
        }

        internal void DiscardCard(Card card, GameState gameState, DeckPlacement source)
        {
            this.cardBeingDiscarded = card;

            if (source != DeckPlacement.Deck)
            {
                this.gameLog.PlayerDiscardCard(this, card);
                this.gameLog.PushScope();
                if (gameState.players.CurrentPlayer.playPhase != PlayPhase.Cleanup)
                {                    
                    card.DoSpecializedDiscardNonCleanup(this, gameState);                    
                }
                
                if (source == DeckPlacement.Play)
                {
                    card.DoSpecializedDiscardFromPlay(this, gameState);
                }
                this.gameLog.PopScope();
            }

            if (this.cardBeingDiscarded != null)
            {
                this.discard.AddCard(card);
                this.cardBeingDiscarded = null;
            }
        }
       
        internal void MoveRevealedCardToDiscard(Card typeOfCard, GameState gameState)
        {
            Card card = this.cardsBeingRevealed.RemoveCard(typeOfCard);
            if (card == null)
            {
                throw new Exception("Revealed cards did not have the specified card");
            }
          
            this.DiscardCard(card, gameState, DeckPlacement.Revealed);
        }
        
        internal void MoveRevealedCardToTrash(Card typeOfCard, GameState gameState)
        {
            Card card = this.cardsBeingRevealed.RemoveCard(typeOfCard);
            if (card == null)
            {
                throw new Exception("Revealed cards did not have the specified card");
            }
            this.MoveCardToTrash(card, gameState);
        }

        internal void MoveLookedAtCardToTopOfDeck()
        {
            MoveRevealedCardToTopOfDeck();
        }

        internal void MoveRevealedCardToTopOfDeck()
        {
            if (this.cardsBeingRevealed.Any)
            {
                if (this.cardsBeingRevealed.Count > 1)
                {
                    throw new Exception("With more than one card in revealed cards it's ambiguous which order to move cards on top of deck");
                }

                Card card = this.cardsBeingRevealed.RemoveCard();
                this.gameLog.PlayerTopDeckedCard(this, card);
                this.deck.AddCardToTop(card);
            }
        }

        internal void MoveLookedAtCardToTopOfDeck(Card card)
        {
            MoveRevealedCardToTopOfDeck(card);
        }        

        internal void ReturnCardFromHandToSupply(Card typeOfCard, GameState gameState)
        {
            Card cardToReturn = this.hand.RemoveCard(typeOfCard);
            if (cardToReturn == null)
                throw new Exception("Could not return card as it is not in hand.");

            PileOfCards pile = gameState.GetPile(cardToReturn);
            if (pile == null)
                throw new Exception("Could not find supply pile");

            pile.AddCardToTop(cardToReturn);
        }

        internal void MoveRevealedCardToTopOfDeck(Card typeOfCard)
        {
            Card card = this.cardsBeingRevealed.RemoveCard(typeOfCard);
            if (card == null)
            {
                throw new Exception("Revealed cards did not have the specified card");
            }
            this.deck.AddCardToTop(card);
        }

        internal void MoveRevealedCardsToHand(CardPredicate acceptableCard)
        {
            Card card = this.MoveRevealedCardToHand(acceptableCard);
            while (card != null)
            {
                card = this.MoveRevealedCardToHand(acceptableCard);
            }
        }

        internal Card MoveRevealedCardToHand(CardPredicate acceptableCard)
        {
            Card card = this.cardsBeingRevealed.RemoveCard(acceptableCard);
            if (card == null)
            {
                return null;
            }

            this.cardsBeingRevealed.RemoveCard(card);
            this.hand.AddCard(card);
            return card;
        }             
        
        internal void MoveRevealedCardToHand(Card typeOfCard)
        {            
            Card card = this.cardsBeingRevealed.RemoveCard(typeOfCard);
            if (card == null)
            {
                throw new Exception("Revealed cards did not have the specified card");
            }
            this.gameLog.PlayerPutCardInHand(this, card);
            this.hand.AddCard(card);
        }

        private void MoveAllCardsToDiscard(CollectionCards cards, GameState gameState, DeckPlacement source)
        {
            foreach (Card card in cards)
            {
                this.DiscardCard(card, gameState, source);                
            }
            cards.Clear();
        }

        internal bool IsAffectedByAttacks(GameState gameState)
        {
            bool doesCancelAttack = false;

            bool didCardAffectAnything = true;

            while (didCardAffectAnything)
            {
                didCardAffectAnything = false;
                foreach (Card reactionCard in this.hand)
                {
                    bool didthisCardCancelAttack;
                    didCardAffectAnything = reactionCard.DoReactionToAttack(this, gameState, out didthisCardCancelAttack);
                    doesCancelAttack |= didthisCardCancelAttack;

                    if (didCardAffectAnything)
                        break;
                }
            }

            foreach (Card durationCard in this.CardsInPlay)
            {
                if (durationCard.DoReactionToAttackWhileInPlayAcrossTurns(this, gameState))
                {
                    doesCancelAttack = true;
                }
            }

            return !doesCancelAttack;
        }

        public IEnumerable<Card> CardsInPlay
        {
            get
            {
                foreach (Card card in this.durationCards)
                {
                    yield return card;
                }

                foreach (Card card in this.cardsPlayed)
                {
                    yield return card;
                }
            }
        }

        public IEnumerable<Card> CardsInDeckAndDiscard
        {
            get
            {
                foreach (Card card in this.deck)
                {
                    yield return card;
                }

                foreach (Card card in this.discard)
                {
                    yield return card;
                }
            }
        }

        public IEnumerable<Card> KnownCardsInDeck
        {
            get
            {
                return this.deck.KnownCards;
            }
        }

        public IEnumerable<Card> CardsInDeck
        {
            get
            {
                foreach (Card card in this.deck)
                {
                    yield return card;
                }                
            }
        }

        public IEnumerable<Card> AllOwnedCards
        {
            get
            {
                foreach (Card card in this.deck)
                {
                    yield return card;
                }

                foreach (Card card in this.hand)
                {
                    yield return card;
                }

                foreach (Card card in this.discard)
                {
                    yield return card;
                }

                foreach (Card card in this.cardsBeingRevealed)
                {
                    yield return card;
                }

                foreach (Card card in this.cardsBeingPlayed)
                {
                    yield return card;
                }

                foreach (Card card in this.durationCards)
                {
                    yield return card;
                }

                foreach (Card card in this.cardsToReturnToHandAtStartOfTurn)
                {
                    yield return card;
                }

                foreach (Card card in this.cardsPlayed)
                {
                    yield return card;
                }

                foreach (Card card in this.islandMat)
                {
                    yield return card;
                }

                foreach (Card card in this.nativeVillageMat)
                {
                    yield return card;
                }

                if (this.cardToPass != null)
                {
                    yield return this.cardToPass;
                }
            }
        }
    }    
}
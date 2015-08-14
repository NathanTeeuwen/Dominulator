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
        internal readonly PlayerActionWithSelf actions;
        internal readonly Game game;
        internal IGameLog gameLog { get { return this.game.GameLog; } }
        internal Random random { get { return this.game.random; } }

        private PlayPhase playPhase;

        internal PlayPhase PlayPhase
        {
            get
            {
                return this.playPhase;
            }
        }

        internal void EnterPhase(PlayPhase phase)
        {
            this.gameLog.EndPhase(this);
            this.playPhase = phase;
            this.gameLog.BeginPhase(this);
        }

        internal PlayerTurnCounters turnCounters;
        
        internal bool ownsCardThatMightProvideDiscountWhileInPlay;
        internal bool ownsCardThatHasSpecializedCleanupAtStartOfCleanup;
        internal bool ownsCardWithSpecializedActionOnBuyWhileInPlay;
        internal bool ownsCardWithSpecializedActionOnTrashWhileInHand;
        internal bool ownsCardWithSpecializedActionOnGainWhileInPlay;
        internal bool ownsCardWithSpecializedActionOnBuyWhileInHand;
        internal bool ownsCardWithSpecializedActionOnGainWhileInHand;
        internal bool ownsCardWithSpecializedActionToCardWhileInPlay;

        internal IEnumerator<Card> shuffleLuck = null;

        // all of the cards the player owns.  Always move from one list to the other
        internal ListOfCards deck;
        internal BagOfCards discard;
        internal BagOfCards cardsSetAside;
        internal ListOfCards cardsBeingPlayed;  // a stack for recursion
        internal BagOfCards cardsBeingRevealed;        
        internal BagOfCards hand;        
        internal BagOfCards cardsPlayed;
        internal BagOfCards durationCards;
        internal BagOfCards cardsToReturnToHandAtStartOfTurn;
        internal SingletonCardHolder cardToPass;
        internal ListOfCards cardBeingDiscarded; // a stack for recursion.
        internal BagOfCards islandMat;
        internal BagOfCards nativeVillageMat;

        internal List<GameStateMethod> actionsToExecuteAtBeginningOfNextTurn = new List<GameStateMethod>();

        // expose information for use by strategies
        public IPlayerAction Actions { get { return this.actions.actions; } }
        public int AvailablePotions { get { return this.turnCounters.AvailablePotions; } }
        public int AvailableCoins { get { return this.turnCounters.AvailableCoins; } }
        public int AvailableCoinTokens { get { return this.turnCounters.AvailableCoinTokens; } }
        public int AvailableActions { get { return this.turnCounters.AvailableActions; } }
        public int AvailableBuys { get { return this.turnCounters.AvailableBuys; } }        
        public CollectionCards Hand { get { return this.hand; } }
        public BagOfCards CardsBeingRevealed { get { return this.cardsBeingRevealed; } }
        public BagOfCards CardsBeingLookedAt { get { return this.cardsBeingRevealed; } }
        public BagOfCards Discard { get { return this.discard; } }        
        public CollectionCards CardsInDeck { get { return this.deck; } }
        public int TurnNumber { get { return this.numberOfTurnsPlayed; } }        
        public CollectionCards CardsBeingPlayed { get { return this.cardsPlayed; } }
        public BagOfCards CardsOnNativeVillageMat { get { return this.nativeVillageMat; } } 
        public int PlayerIndex { get { return this.playerIndex; } }
        public SetOfCards CardsBoughtThisTurn { get { return this.turnCounters.cardsBoughtThisTurn; } }
        public SetOfCards CardsGainedThisTurn { get { return this.turnCounters.cardsGainedThisTurn; } }

        public Game Game { get { return this.game; } }

        // Card specific state that strategies can query about
        internal int numberOfCardsToBeDrawn;
        public int NumberOfCardsToBeDrawn { get { return this.numberOfCardsToBeDrawn; } }

        public int ExpectedCoinValueAtEndOfTurn { get { return this.AvailableCoins + this.hand.Where(card => card.isTreasure).Select(card => card.plusCoin).Sum(); } }

        // counters and duplicates.
        internal BagOfCards allOwnedCards;
        internal BagOfCards cardsInPlay;
        internal BagOfCards cardsInPlayAtBeginningOfCleanupPhase;

        // persistent Counters
        private int victoryTokenCount;
        internal int VictoryTokenCount { get { return this.victoryTokenCount; } }

        internal int pirateShipTokenCount;

        internal PlayerState(IPlayerAction actions, int playerIndex, Game game)
        {
            this.game = game;

            CardGameSubset gameSubset = game.CardGameSubset;            
            this.actions = new PlayerActionWithSelf(actions, this);
            this.EnterPhase(PlayPhase.NotMyTurn);            
            this.playerIndex = playerIndex;

            // duplicates
            this.allOwnedCards = new BagOfCards(gameSubset);
            this.cardsInPlay = new BagOfCards(gameSubset, this.allOwnedCards);
            this.cardsInPlayAtBeginningOfCleanupPhase = new BagOfCards(gameSubset);

            // partition
            this.islandMat = new BagOfCards(gameSubset, this.allOwnedCards);
            this.nativeVillageMat = new BagOfCards(gameSubset, this.allOwnedCards);
            this.deck = new ListOfCards(gameSubset, this.allOwnedCards);
            this.discard = new BagOfCards(gameSubset, this.allOwnedCards);
            this.cardsBeingPlayed = new ListOfCards(gameSubset, this.allOwnedCards);  // a stack for recursion
            this.cardsBeingRevealed = new BagOfCards(gameSubset, this.allOwnedCards);
            this.hand = new BagOfCards(gameSubset, this.allOwnedCards);
            this.cardsPlayed = new BagOfCards(gameSubset, this.cardsInPlay);
            this.durationCards = new BagOfCards(gameSubset, this.cardsInPlay);
            this.cardsToReturnToHandAtStartOfTurn = new BagOfCards(gameSubset, this.allOwnedCards);
            this.cardToPass = new SingletonCardHolder(this.allOwnedCards);
            this.cardBeingDiscarded = new ListOfCards(gameSubset, this.allOwnedCards);
            this.cardsSetAside = new BagOfCards(gameSubset, this.allOwnedCards);            
            
            
            this.turnCounters = new PlayerTurnCounters(gameSubset);
        }

        internal void InitializeTurn()
        {            
            this.turnCounters.InitializeTurn();
        }        

        public int TotalScore()
        {
            int result = 0;

            foreach(Card card in this.game.CardGameSubset)
            {                
                if (!card.isVictory)
                    continue;
                result += this.AllOwnedCards.CountOf(card) * card.VictoryPoints(this);
            }

            result -= this.AllOwnedCards.CountOf(Cards.Curse);
            result += this.victoryTokenCount;
            return result;            
        }

        internal void DrawUntilCountInHand(int totalCount, GameState gameState)
        {
            while (hand.Count < totalCount)
            {
                if (!DrawOneCardIntoHand(gameState))
                {
                    return;
                }
            }
        }

        internal void DrawAdditionalCardsIntoHand(int count, GameState gameState)
        {
            for (int i = 0; i < count; ++i)
            {
                if (!DrawOneCardIntoHand(gameState))
                {
                    return;
                }
            }
        }

        internal Card TrashCardFromTopOfDeck(GameState gameState)
        {
            Card card = DrawOneCard(gameState);
            if (card == null)
            {
                return null;
            }

            MoveCardToTrash(card, gameState);

            return card;
        }

        internal bool DrawOneCardIntoHandCardOrderDestroyed(Card cardToDraw, GameState gameState)
        {
            Card card = this.DrawOneCardDeckOrderDestroyed(cardToDraw, gameState);
            if (card == null)
            {
                return false;
            }

            this.gameLog.DrewCardIntoHand(this, card);
            this.hand.AddCard(card);

            return true;
        }

        internal bool DrawOneCardIntoHand(GameState gameState)
        {
            Card card = this.DrawOneCard(gameState);
            if (card == null)
            {
                return false;
            }

            this.gameLog.DrewCardIntoHand(this, card);
            this.hand.AddCard(card);

            return true;
        }

        internal bool DrawCardsIntoHand(IEnumerable<CardCountPair> startingHand, GameState gameState)
        {
            foreach (CardCountPair pair in startingHand)
            {
                for (int i = 0; i < pair.Count; ++i)
                    if (!this.DrawOneCardIntoHandCardOrderDestroyed(pair.Card, gameState))
                        return false;
            }                       

            return true;
        }

        private Card DrawOneCard(GameState gameState)
        {
            if (this.deck.IsEmpty && !this.discard.IsEmpty)
            {                
                TriggerShuffleOfDiscardIntoDeck(gameState);
            }

            if (this.deck.IsEmpty)
                return null;

            if (this.shuffleLuck != null)
            {
                if (this.shuffleLuck.MoveNext())
                {
                    Card preferredCard = this.shuffleLuck.Current;
                    Card result = this.deck.FindAndRemoveCardOrderDestroyed(preferredCard);
                    if (result != null)
                        return result;
                }
                else
                {
                    this.shuffleLuck = null;
                }
            }
           
            Card card = this.deck.DrawCardFromTop();            
            return card;
        }

        private Card DrawOneCardDeckOrderDestroyed(Card cardToFind, GameState gameState)
        {
            if (this.deck.IsEmpty && !this.discard.IsEmpty)
            {
                TriggerShuffleOfDiscardIntoDeck(gameState);
            }

            Card card = this.deck.FindAndRemoveCardOrderDestroyed(cardToFind);
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

        internal void RevealCardsFromDeck(int cardCount, GameState gameState)
        {
            for (int i = 0; i < cardCount; ++i)
            {
                if (this.DrawAndRevealOneCardFromDeck(gameState) == null)
                {
                    break;
                }
            }
        }

        internal Card LookAtBottomCardFromDeck(GameState gameState)
        {
            if (this.deck.IsEmpty)
                this.TriggerShuffleOfDiscardIntoDeck(gameState);

            return this.deck.BottomCard();
        }

        internal void MoveCardFromBottomOfDeckToTop()
        {
            Card cardMoved = this.deck.MoveBottomCardToTop();
            if (cardMoved != null)
            {
                this.gameLog.PlayerTopDeckedCard(this, cardMoved);
            }            
        }        

        internal void LookAtCardsFromDeck(int cardCount, GameState gameState)
        {
            this.RevealCardsFromDeck(cardCount, gameState);
        }

        internal void RevealCard(Card card, DeckPlacement source)
        {
            this.gameLog.PlayerRevealedCard(this, card, DeckPlacement.TopOfDeck);            
        }

        internal Card DrawAndLookAtOneCardFromDeck(GameState gameState)
        {
            // TODO: separate reveal from look
            return DrawAndRevealOneCardFromDeck(gameState);
        }

        internal Card DrawAndRevealOneCardFromDeck(GameState gameState)
        {
            Card card = this.DrawOneCard(gameState);            
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
                return this.cardsBeingPlayed.Count + this.cardsPlayed.Count;
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

        internal void AddPotions(int potionAmount)
        {
            this.turnCounters.AddPotions(this, potionAmount);
        }

        internal void AddCoinTokens(int coinAmount)
        {
            this.turnCounters.AddCoinTokens(this, coinAmount);   
        }

        internal void AddVictoryTokens(int amount)
        {
            if (amount == 0)
                return;

            this.victoryTokenCount += amount;
            this.gameLog.PlayerGainedVictoryTokens(this, amount);
        }

        internal void DoPlayAction(Card currentCard, GameState gameState, int countTimes = 1)
        {
            if (!currentCard.isAction)
            {
                throw new Exception("Can't play a card that isn't a action");
            }
            
            this.cardsBeingPlayed.AddCardToTop(currentCard);            

            Card cardToPlayAs = currentCard.CardToMimick(this, gameState);
            gameState.cardContextStack.PushCardContext(this, cardToPlayAs, CardContextReason.CardBeingPlayed);

            if (cardToPlayAs != null)
            {
                for (int i = 0; i < countTimes; ++i)
                {
                    this.gameLog.PlayedCard(this, currentCard);
                    this.gameLog.PushScope();

                    this.AddActions(cardToPlayAs.plusAction);
                    this.AddBuys(cardToPlayAs.plusBuy);
                    this.AddCoins(cardToPlayAs.plusCoin);
                    this.AddVictoryTokens(cardToPlayAs.plusVictoryToken);
                    this.DrawAdditionalCardsIntoHand(cardToPlayAs.plusCard, gameState);

                    if (cardToPlayAs.isAttack && cardToPlayAs.isAttackBeforeAction)
                    {
                        AttackOtherPlayers(gameState, cardToPlayAs.DoSpecializedAttack);
                    }

                    cardToPlayAs.DoSpecializedAction(gameState.players.CurrentPlayer, gameState);

                    if (cardToPlayAs.isAttack && !cardToPlayAs.attackDependsOnPlayerChoice && !cardToPlayAs.isAttackBeforeAction)
                    {
                        AttackOtherPlayers(gameState, cardToPlayAs.DoSpecializedAttack);
                    }

                    if (this.ownsCardWithSpecializedActionToCardWhileInPlay)
                    {
                        foreach(var cardInPlay in this.cardsInPlay)
                        {
                            if (cardInPlay.HasSpecializedActionToCardWhileInPlay)
                            {
                                gameState.cardContextStack.PushCardContext(this, cardInPlay, CardContextReason.CardReacting);
                                cardInPlay.DoSpecializedActionToCardWhileInPlay(this, gameState, cardToPlayAs);
                                gameState.cardContextStack.Pop();
                            }
                        }
                    }
                    
                    this.gameLog.PopScope();
                }
            }

            gameState.cardContextStack.Pop();
            CardHasBeenPlayed(cardToPlayAs, countTimes);
        }

        internal delegate void AttackAction(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState);

        internal void AttackOtherPlayers(GameState gameState, AttackAction action)
        {
            Card currentAttackCard = gameState.cardContextStack.CurrentCard;
            foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
            {
                gameState.cardContextStack.PushCardContext(otherPlayer, currentAttackCard, CardContextReason.CardAttacking);
                if (otherPlayer.IsAffectedByAttacks(gameState))
                {
                    action(gameState.players.CurrentPlayer, otherPlayer, gameState);
                }
                gameState.cardContextStack.Pop();
            }
        }

        private void CardHasBeenPlayed(Card cardPlayedAs, int count)
        {
            Card cardAfterPlay = this.cardsBeingPlayed.DrawCardFromTop();
            if (cardAfterPlay != null)
            {
                if (cardAfterPlay.isDuration)
                {
                    this.durationCards.AddCard(cardAfterPlay);
                    for (int i = 1; i < count; ++i)
                    {
                        this.actionsToExecuteAtBeginningOfNextTurn.Add(delegate(PlayerState currentPlayer, GameState gameState)
                        {
                            gameState.cardContextStack.PushCardContext(this, cardPlayedAs, CardContextReason.CardFinishingDuration);
                            cardPlayedAs.DoSpecializedDurationActionAtBeginningOfTurn(currentPlayer, gameState);
                            gameState.cardContextStack.Pop();
                        });
                    }
                }
                else
                {
                    this.cardsPlayed.AddCard(cardAfterPlay);
                }

                if (cardPlayedAs != cardAfterPlay && cardPlayedAs != null)
                {
                    if (cardPlayedAs.isDuration)
                    {                        
                        this.actionsToExecuteAtBeginningOfNextTurn.Add( delegate(PlayerState currentPlayer, GameState gameState)
                        {
                            gameState.cardContextStack.PushCardContext(this, cardPlayedAs, CardContextReason.CardFinishingDuration);
                            cardPlayedAs.DoSpecializedDurationActionAtBeginningOfTurn(currentPlayer, gameState);
                            gameState.cardContextStack.Pop();
                        });
                    }
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
            gameState.cardContextStack.PushCardContext(this, currentCard, CardContextReason.CardBeingPlayed);

            this.AddBuys(currentCard.plusBuy);
            this.AddCoins(currentCard.plusCoin);
            if (currentCard == Cards.Copper)
            {
                this.AddCoins(this.turnCounters.copperAdditionalValue);
            }

            currentCard.DoSpecializedAction(gameState.players.CurrentPlayer, gameState);

            CardHasBeenPlayed(currentCard, 1);
            gameState.cardContextStack.Pop();
            this.gameLog.PopScope();
        }        

        private void PlaceCardFromPlacement(CardPlacementPair pair, GameState gameState, DeckPlacement originalSource)
        {
            gameLog.CardWentToLocation(pair.placement);
            switch (pair.placement)
            {
                case DeckPlacement.Discard: this.discard.AddCard(pair.card); break;
                case DeckPlacement.Hand: this.hand.AddCard(pair.card); break;
                case DeckPlacement.Trash: this.MoveCardToTrash(pair.card, gameState); break;
                case DeckPlacement.Play: this.PlayCard(pair.card, gameState); break;
                case DeckPlacement.TopOfDeck: this.deck.AddCardToTop(pair.card); break;
                case DeckPlacement.None:
                {
                    switch (originalSource)
                    {
                        case DeckPlacement.Supply: this.ReturnCardToSupply(pair.card, gameState); break;
                        case DeckPlacement.Trash: this.MoveCardToTrash(pair.card, gameState); break;
                        default: throw new NotImplementedException();
                    }
                    break;
                }
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

        internal Card TrashCardFromHandOfType(Card card, GameState gameState, bool guaranteeInHand)
        {
            return TrashCardFromHandOfType(gameState, card, guaranteeInHand);
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
                this.ReturnCardToSupply(cardInPlay, gameState);              
                wasReturned = true;                
            }

            this.cardsBeingPlayed.AddCardToTop(null);
            return wasReturned;
        }

        internal void MoveCardFromPlayedAreaToTrash(Card card, GameState gameState)
        {            
            Card cardInPlay = this.cardsPlayed.RemoveCard(card);
            if (cardInPlay != null)
            {
                MoveCardToTrash(cardInPlay, gameState);
            }                   
        }

        internal void MoveCardsFromPlayedAreaToTrash(CardPredicate acceptableCard, GameState gameState)
        {
            while (this.cardsPlayed.HasCard(acceptableCard))
            {
                Card cardRemoved = this.cardsPlayed.RemoveCard(acceptableCard);
                MoveCardToTrash(cardRemoved, gameState);
            }            
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
            Card cardBeingDiscarded = this.cardBeingDiscarded.DrawCardFromTop();
            if (cardBeingDiscarded != null)
            {
                MoveCardToTrash(cardBeingDiscarded, gameState);                
            }
            this.cardBeingDiscarded.AddCardToTop(null);
        }

        internal void MoveCardToTrash(Card card, GameState gameState)
        {            
            this.gameLog.PlayerTrashedCard(this, card);            
            this.gameLog.PushScope();
            gameState.cardContextStack.PushCardContext(this, card, CardContextReason.CardBeingTrashed);

            if (card.DoSpecializedTrash(this, gameState))
            {
                gameState.trash.AddCard(card);
            }

            // cards in hand react to trashing.
            if (this.ownsCardWithSpecializedActionOnTrashWhileInHand)
            {
                bool stateHasChanged = true;
                while (stateHasChanged)
                {
                    stateHasChanged = false;
                    foreach (Card cardInHand in this.hand)
                    {
                        gameState.cardContextStack.PushCardContext(this, cardInHand, CardContextReason.CardReacting);
                        stateHasChanged = cardInHand.DoSpecializedActionOnTrashWhileInHand(this, gameState, cardInHand);
                        gameState.cardContextStack.Pop();
                        if (stateHasChanged)
                            break;
                    }
                }
            }
            gameState.cardContextStack.Pop();
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

        internal Card DiscardCardFromTopOfDeck(GameState gameState)
        {
            Card card = this.deck.DrawCardFromTop();
            if (card != null)
            {
                this.DiscardCard(card, gameState, source:DeckPlacement.TopOfDeck);                
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
            Card cardBeingPlayed = this.cardsBeingPlayed.DrawCardFromTop();            
            if (cardBeingPlayed != null)
            {
                if (card != cardBeingPlayed)
                    throw new Exception("Expected card to be same as that was being played");

                this.islandMat.AddCard(cardBeingPlayed);
                this.gameLog.PlayerPlacedCardOnIslandMat(this, card);
            }
        }

        internal void MoveCardFromHandToIslandMat(Card cardType)
        {
            Card removedCard = this.hand.RemoveCard(cardType);
            if (removedCard != null)
            {                
                this.islandMat.AddCard(removedCard);
            }
        }

        internal void MoveNativeVillageMatToHand()
        {
            while (this.nativeVillageMat.Any())
            {
                Card card = this.nativeVillageMat.RemoveSomeCard();
                this.hand.AddCard(card);
            }
        }

        internal void PutOnNativeVillageMatCardFromTopOfDeck(GameState gameState)
        {
            Card card = this.DrawOneCard(gameState);
            if (card != null)
            {
                this.nativeVillageMat.AddCard(card);                
            }

            gameState.gameLog.PlayerPlacedCardOnNativeVillageMat(this, card);
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
            this.EnterPhase(PlayPhase.SpendCoinTokens);
            int coinToSpend = this.actions.GetCoinAmountToSpendInBuyPhase(gameState);
            if (coinToSpend > this.AvailableCoinTokens || coinToSpend < 0)
                throw new Exception("Can not spend that many coins");
            if (coinToSpend != 0)
            {
                this.AddCoinTokens(-coinToSpend);
                this.gameLog.PushScope();
                this.AddCoins(coinToSpend);
                this.gameLog.PopScope();
            }
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

        internal PileOfCards RequestPlayerEmbargoPileFromSupply(GameState gameState)
        {
            Card cardType = this.actions.GetCardFromSupplyToEmbargo(gameState);

            PileOfCards pile = gameState.GetPile(cardType);
            if (pile == null)
            {
                throw new Exception("Must choose pile from supply");
            }

            return pile;
        }

        internal Card RequestPlayerChooseCardFromSupplyToPlay(GameState gameState, CardPredicate acceptableCard)
        {
            if (!gameState.supplyPiles.Where(pile => acceptableCard(pile.ProtoTypeCard)).Any())
                return null;

            Card cardType = this.actions.GetCardFromSupplyToPlay(gameState, delegate(Card c)
            {
                if (!acceptableCard(c))
                    return false;
                
                PileOfCards pile = gameState.GetSupplyPile(c);
                if (pile == null || pile.Count == 0)
                {
                    return false;
                }

                return true;
            });

            if (!acceptableCard(cardType))
            {
                throw new Exception("Card did not meet constraint");
            }

            PileOfCards foundPile = gameState.GetSupplyPile(cardType);
            if (foundPile == null || foundPile.Count == 0)
            {
                throw new Exception("Must choose pile from supply");
            }

            return cardType;
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

        internal Card RequestPlayerGainCardFromTrash(GameState gameState, CardPredicate acceptableCard, string description, bool isOptional = false, DeckPlacement defaultLocation = DeckPlacement.Discard)
        {
            if (!gameState.trash.HasCard(acceptableCard))
                return null;

            Card cardType = this.actions.GetCardFromTrashToGain(gameState, acceptableCard, isOptional);
            if (cardType == null)
            {
                if (isOptional)
                {
                    return null;
                }
                throw new Exception("Must gain a card where " + description);
            }

            if (!acceptableCard(cardType))
            {
                throw new Exception("Card does not meet constraint: " + description);
            }

            Card cardFromTrash = gameState.trash.RemoveCard(cardType);
            if (cardFromTrash == null)
            {
                throw new Exception("Card requested wasnt in the trash");
            }

            this.GainCard(gameState, cardFromTrash, DeckPlacement.Trash, defaultLocation, GainReason.Gain);

            return cardFromTrash;            
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

            CardPredicate cardPredicate = card => gameState.GetSupplyPile(card) != null && acceptableCard(card);
            // how do you know which player you are gaining for?
            Card cardType = this.actions.GetCardFromSupplyToGain(gameState, cardPredicate, isOptional);
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

        internal CollectionCards RequestPlayerTrashCardsFromHand(GameState gameState, int cardCount, bool isOptional, bool allOrNone = false)
        {
            var trashedCards = new BagOfCards(gameState.CardGameSubset);
            CardPredicate acceptableCardsToTrash = card => true;
            while (trashedCards.Count < cardCount)
            {
                Card trashedCard = this.RequestPlayerTrashCardFromHandButDontTrash(gameState, acceptableCardsToTrash, isOptional);
                if (trashedCard == null)
                {
                    break;
                }

                if (allOrNone == true)
                    isOptional = false;

                trashedCards.AddCard(trashedCard);
                this.RemoveCardFromHand(trashedCard);
            }

            foreach (var trashedCard in trashedCards)
                this.MoveCardToTrash(trashedCard, gameState);

            return trashedCards;
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

        internal Card RequestPlayerTrashCardFromHandButDontTrash(GameState gameState, CardPredicate acceptableCardsToTrash, bool isOptional, CollectionCards cardsTrashedSoFar = null)
        {
            if (!this.hand.HasCard(acceptableCardsToTrash))
            {
                return null;
            }

            if (cardsTrashedSoFar == null)
                cardsTrashedSoFar = gameState.emptyCardCollection;

            Card cardTypeToTrash = this.actions.GetCardFromHandToTrash(gameState, acceptableCardsToTrash, isOptional, cardsTrashedSoFar);
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

            if (!acceptableCardsToTrash(cardTypeToTrash))
            {
                throw new Exception("Tried to trash a card that didn't match the constraint");
            }

            return cardTypeToTrash;
        }

        internal Card RequestPlayerTrashCardFromHand(GameState gameState, CardPredicate acceptableCardsToTrash, bool isOptional, CollectionCards cardsTrashedSoFar = null)
        {
            Card cardTypeToTrash = RequestPlayerTrashCardFromHandButDontTrash(gameState, acceptableCardsToTrash, isOptional, cardsTrashedSoFar);
            
            if (cardTypeToTrash != null)
                this.TrashCardFromHandOfType(gameState, cardTypeToTrash, guaranteeInHand: true);

            return cardTypeToTrash;
        }      

        internal Card RequestPlayerTrashOtherPlayersRevealedCard(GameState gameState, CardPredicate acceptableCard,  PlayerState otherPlayer)
        {
            Card cardtoTrash = null;
            if (otherPlayer.cardsBeingRevealed.HasCard(acceptableCard))
            {
                Card cardTypeToTrash = this.actions.GetCardFromOtherPlayersRevealedCardsToTrash(gameState, otherPlayer, acceptableCard);

                cardtoTrash = otherPlayer.cardsBeingRevealed.RemoveCard(cardTypeToTrash);
                if (cardtoTrash == null)
                {
                    throw new Exception("Must choose a revealed card to trash");
                }

                if (!acceptableCard(cardtoTrash))
                {
                    throw new Exception("Card constraint doesnt match.");
                }

                otherPlayer.MoveCardToTrash(cardtoTrash, gameState);
            }

            return cardtoTrash;
        }

        internal Card RequestPlayerTrashCardFromHandOrDiscard(GameState gameState, CardPredicate acceptableCardsToTrash, bool isOptional)
        {
            if (!this.hand.HasCard(acceptableCardsToTrash) &&
                !this.discard.HasCard(acceptableCardsToTrash))
            {
                return null;
            }

            DeckPlacement deckPlacement = DeckPlacement.Default;
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

        internal void RequestPlayerInspectTopOfDeckForDiscard(PlayerState decidingPlayer, GameState gameState, bool shouldReveal = true)
        {
            Card movedCard = shouldReveal ? this.DrawAndRevealOneCardFromDeck(gameState) : this.DrawAndLookAtOneCardFromDeck(gameState);
            if (movedCard == null)
                return;
            gameState.gameLog.PushScope();
            if (decidingPlayer.actions.ShouldPlayerDiscardCardFromDeck(gameState, this, movedCard))
            {
                if (shouldReveal)
                {
                    this.MoveRevealedCardToDiscard(movedCard, gameState);
                }
                else
                {
                    this.MoveLookedAtCardsToDiscard(gameState);
                }                
            }
            else
            {
                if (shouldReveal)
                {
                    this.MoveRevealedCardToTopOfDeck();
                }
                else
                {
                    this.MoveLookedAtCardToTopOfDeck();
                }                
            }
            gameState.gameLog.PopScope();
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

            Card cardTypeToDiscard = this.actions.GetCardFromHandToDiscard(gameState, acceptableCardsToDiscard, isOptional);
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
            otherPlayer.RevealHand();

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
                Card cardToPutOnTop = this.actions.GetCardFromRevealedCardsToPutOnDeck(gameState);
                if (cardToPutOnTop == null)
                {
                    throw new Exception("Player must choose a card to put on top of deck");
                }

                this.MoveRevealedCardToTopOfDeck(cardToPutOnTop);
            }
        }

        internal void RequestPlayerTrashLookedAtCard(GameState gameState)
        {
            RequestPlayerTrashRevealedCard(gameState, acceptableCard => true);
        }       

        internal Card RequestPlayerTrashRevealedCard(GameState gameState, CardPredicate acceptableCard)
        {
            if (!this.cardsBeingRevealed.AnyWhere(acceptableCard))
            {
                return null;
            }

            Card cardtoTrash = this.actions.GetCardFromRevealedCardsToTrash(gameState, acceptableCard);
            if (cardtoTrash == null)
            {
                throw new Exception("Player must choose a card to trash");
            }

            if (!acceptableCard(cardtoTrash))
            {
                throw new Exception("Card did not meet constraint");
            }

            this.MoveRevealedCardToTrash(cardtoTrash, gameState);

            return cardtoTrash;
        }

         internal Card RequestPlayerMoveRevealedCardToHand(GameState gameState, CardPredicate acceptableCard)
        {
            if (!this.cardsBeingRevealed.AnyWhere(acceptableCard))
            {
                return null;
            }

            Card cardToPutInHand = this.actions.GetCardFromRevealedCardsToPutInHand(gameState, acceptableCard);
            if (cardToPutInHand == null)
            {
                throw new Exception("Player must choose a card to put in hand");
            }

            if (!acceptableCard(cardToPutInHand))
            {
                throw new Exception("Card did not meet constraint");
            }

            this.MoveRevealedCardToHand(cardToPutInHand);

            return cardToPutInHand;
        }       

        internal void RequestPlayerDiscardRevealedCard(GameState gameState)
        {
            if (this.cardsBeingRevealed.Any)
            {
                Card cardToDiscard = this.actions.GetCardFromRevealedCardsToDiscard(gameState);
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
            return SetAsideCardFromHandToNextTurn(cardType);
        }

        internal Card SetAsideCardFromHandToNextTurn(Card cardType)
        {
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

        internal void RequestPlayerTopDeckCardFromCleanup(Card card, GameState gameState)
        {            
            if (this.actions.ShouldPutCardOnTopOfDeck(card, gameState))
            {
                var cardToTopDeck = this.cardsPlayed.RemoveCard(card);
                if (cardToTopDeck != null)
                {
                    this.gameLog.PlayerTopDeckedCard(this, cardToTopDeck);
                    this.deck.AddCardToTop(cardToTopDeck);
                }
            }            
        }        

        internal Card RequestPlayerTopDeckCardFromPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            if (!this.cardsPlayed.HasCard(acceptableCard))
            {
                return null;
            }

            Card cardTypeToTopDeck = this.actions.GetCardFromPlayToTopDeckDuringCleanup(gameState, acceptableCard, isOptional);
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
                this.MoveAllRevealedCardsToHand();
                return revealedCard;
            }

            return null;
        }

        internal Card RequestPlayerTopDeckCardFromRevealed(GameState gameState, bool isOptional)
        {
            Card cardTypeToTopDeck = this.actions.GetCardFromRevealedCardsToTopDeck(gameState, isOptional);
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

            Card cardTypeToTopDeck = this.actions.GetCardFromDiscardToTopDeck(gameState, isOptional);
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

        internal bool GainCardFromSupply(Card card, GameState gameState, DeckPlacement defaultLocation = DeckPlacement.Discard)
        {
            return gameState.PlayerGainCardFromSupply(card, this, defaultLocation:defaultLocation) != null;
        }        

        internal void GainCard(GameState gameState, Card card, DeckPlacement originalLocation, DeckPlacement defaultPlacement = DeckPlacement.Discard, GainReason gainReason = GainReason.Gain)
        {
            if (gainReason == GainReason.Buy)
            {
                this.gameLog.PlayerBoughtCard(this, card);
                this.turnCounters.cardsBoughtThisTurn.Add(card);
                gameState.cardContextStack.PushCardContext(this, card, CardContextReason.CardBeingBought);
            }
            else
            {
                this.gameLog.PlayerGainedCard(this, card);
                gameState.cardContextStack.PushCardContext(this, card, CardContextReason.CardBeingGained);
            }

            // should only include cards gained on the players turned, not cards gained as a side effect on some other players turn
            // important for smugglers ...
            if (this == gameState.players.CurrentPlayer)
            {
                this.turnCounters.cardsGainedThisTurn.Add(card);
            }

            this.gameLog.PushScope();

            // technically, the hovel reaction can cause hand size to change.  This is not a problem though
            // would only be a problem if cards were added that would subsequently needed to be enumerated.
            bool wasCardMoved = false;

            if (this.ownsCardWithSpecializedActionOnBuyWhileInHand ||
                this.ownsCardWithSpecializedActionOnGainWhileInHand)
            {
                foreach (Card cardInHand in this.Hand)
                {
                    gameState.cardContextStack.PushCardContext(this, cardInHand, CardContextReason.CardReacting);                    
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
                    gameState.cardContextStack.Pop();
                }                
            }

            if (this.ownsCardWithSpecializedActionOnGainWhileInPlay)
            {
                foreach (Card cardInPlay in this.CardsInPlay)
                {
                    gameState.cardContextStack.PushCardContext(this, cardInPlay, CardContextReason.CardReacting);
                    DeckPlacement preferredPlacement = cardInPlay.DoSpecializedActionOnGainWhileInPlay(this, gameState, card);
                    if (!wasCardMoved && preferredPlacement != DeckPlacement.Default)
                    {
                        defaultPlacement = preferredPlacement;
                        wasCardMoved = true;
                    }
                    gameState.cardContextStack.Pop();
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
                if (card.canOverpay)
                {
                    gameState.CurrentContext.PushCardContext(this, card, CardContextReason.CardReacting);
                    this.RequestPlayerOverpayForCard(card, gameState);
                    gameState.CurrentContext.Pop();
                }

                card.DoSpecializedWhenBuy(this, gameState);

                if (this.ownsCardWithSpecializedActionOnBuyWhileInPlay)
                {
                    foreach (Card cardInPlay in this.CardsInPlay)
                    {
                        gameState.cardContextStack.PushCardContext(this, card, CardContextReason.CardReacting);
                        gameLog.PushScope();
                        cardInPlay.DoSpecializedActionOnBuyWhileInPlay(this, gameState, card);
                        gameLog.PopScope();
                        gameState.cardContextStack.Pop();
                    }
                }                            
            }            
            
            this.PlaceCardFromPlacement(new CardPlacementPair(card, defaultPlacement), gameState, originalLocation);
            gameState.cardContextStack.Pop();
            this.gameLog.PopScope();

            gameState.hasCurrentPlayerGainedCard |= true;

            this.ownsCardThatMightProvideDiscountWhileInPlay |= card.MightProvideDiscountWhileInPlay;
            this.ownsCardThatHasSpecializedCleanupAtStartOfCleanup |= card.HasSpecializedCleanupAtStartOfCleanup;
            this.ownsCardWithSpecializedActionOnBuyWhileInPlay |= card.HasSpecializedActionOnBuyWhileInPlay;
            this.ownsCardWithSpecializedActionOnTrashWhileInHand |= card.HasSpecializedActionOnTrashWhileInHand;
            this.ownsCardWithSpecializedActionOnGainWhileInPlay |= card.HasSpecializedActionOnGainWhileInPlay;
            this.ownsCardWithSpecializedActionOnBuyWhileInHand |= card.HasSpecializedActionOnBuyWhileInHand;
            this.ownsCardWithSpecializedActionOnGainWhileInHand |= card.HasSpecializedActionOnGainWhileInHand;
            this.ownsCardWithSpecializedActionToCardWhileInPlay |= card.HasSpecializedActionToCardWhileInPlay;
        }           

        private void TriggerShuffleOfDiscardIntoDeck(GameState gameState)
        {
            this.gameLog.ReshuffledDiscardIntoDeck(this);

            if (!this.deck.IsEmpty)
            {
                throw new Exception("Can not move discard to deck unless deck is empty");
            }
            deck.AddAllCardsFromInSomeOrder(this.discard);            
            this.discard.Clear();
                                   
            this.deck.Shuffle(this.random);

            LetPlayerChooseToArrangeStash(gameState);                        
        }

        private void LetPlayerChooseToArrangeStash(GameState gameState)
        {
            int countStashInDeck = this.deck.CountOf(Cards.Stash);
            if (countStashInDeck > 0)
            {
                for (int i = 0; i < countStashInDeck; ++i)
                {
                    Card stashCard = this.deck.FindAndRemoveCardOrderDestroyed(Cards.Stash);
                    if (stashCard == null)
                        throw new Exception("Expected to find stash");
                }

                this.deck.AddNCardsToTop(Cards.Stash, countStashInDeck);
                int[] placements = CardTypes.Stash.GetStashPlacementBeginningOfDeck(countStashInDeck);
                this.actions.ChooseLocationForStashAfterShuffle(gameState, placements);
                CardTypes.Stash.VerifyStashPlacementInDeck(placements, countStashInDeck);
                for (int i = 0; i < countStashInDeck; ++i)
                {
                    this.deck.SwapCardsInPlace(i, placements[i]);
                }
                gameState.gameLog.PlayerChoseLocationForStash(this, placements);
            }
        }

        public void RevealAndReturnCardToHand(Card card, GameState gameState)
        {
            RevealCardFromHand(card, gameState);
            MoveRevealedCardToHand(card, shouldLog:false);
        }

        internal Card RevealCardFromHand(Card cardTypeToDiscard, GameState gameState)
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

        internal void MoveCardsFromPreviousTurnIntoHand(GameState gameState)
        {
            if (this.cardsToReturnToHandAtStartOfTurn.Any)
            {
                foreach (Card card in this.cardsToReturnToHandAtStartOfTurn)
                {
                    card.DoSpecializedActionOnReturnToHand(this, gameState);
                    this.hand.AddCard(card);
                    this.gameLog.PlayerReturnedCardToHand(this, card);
                }
                this.cardsToReturnToHandAtStartOfTurn.Clear();
            }
        }

        internal void MoveDurationCardsToInPlay()
        {
            if (this.durationCards.Any)
            {
                foreach (Card card in this.durationCards)
                {
                    this.cardsPlayed.AddCard(card);
                }
                this.durationCards.Clear();
            }
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

        internal void MoveRevealedCardsToSetAside()
        {
            this.cardsSetAside.MoveAllCardsFrom(this.cardsBeingRevealed);
        }

        internal void MoveRevealedCardsToDiscard(CardPredicate predicate, GameState gameState)
        {
            if (this.cardsBeingRevealed.Any)
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
        }

        internal void DiscardCard(Card card, GameState gameState, DeckPlacement source)
        {
            this.cardBeingDiscarded.AddCardToTop(card);

            if (source != DeckPlacement.Deck)
            {
                this.gameLog.PlayerDiscardCard(this, card, source);
                this.gameLog.PushScope();
                gameState.cardContextStack.PushCardContext(this, card, CardContextReason.CardBeingDiscarded);
                if (gameState.players.CurrentPlayer.PlayPhase != PlayPhase.Cleanup)
                {                    
                    card.DoSpecializedDiscardNonCleanup(this, gameState);
                }
                
                if (source == DeckPlacement.Play)
                {
                    card.DoSpecializedDiscardFromPlay(this, gameState);
                }
                gameState.cardContextStack.Pop();
                this.gameLog.PopScope();
            }

            Card cardBeingDiscarded = this.cardBeingDiscarded.DrawCardFromTop();
            if (cardBeingDiscarded != null)
            {
                this.discard.AddCard(card);                
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

                Card card = this.cardsBeingRevealed.RemoveSomeCard();
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

            ReturnCardToSupply(cardToReturn, gameState);
        }

        internal void ReturnCardToSupply(Card cardToReturn, GameState gameState)
        {
            this.gameLog.PlayerReturnedCardToPile(this, cardToReturn);
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

        internal void MoveAllRevealedCardsToHand()
        {            
            foreach (var card in this.cardsBeingRevealed)
                this.gameLog.DrewCardIntoHand(this, card);
            this.hand.MoveAllCardsFrom(this.cardsBeingRevealed);            
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
        
            this.gameLog.PlayerPutCardInHand(this, card);
            this.hand.AddCard(card);
            return card;
        }             
        
        internal void MoveRevealedCardToHand(Card typeOfCard, bool shouldLog = true)
        {            
            Card card = this.cardsBeingRevealed.RemoveCard(typeOfCard);
            if (card == null)
            {
                throw new Exception("Revealed cards did not have the specified card");
            }
            if (shouldLog)
            {
                this.gameLog.PlayerPutCardInHand(this, card);
            }
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
                foreach (Card reactionCard in this.hand.AllTypes)
                {
                    gameState.cardContextStack.PushCardContext(this, reactionCard, CardContextReason.CardReacting);
                    bool didthisCardCancelAttack;
                    didCardAffectAnything = reactionCard.DoReactionToAttackWhileInHand(this, gameState, out didthisCardCancelAttack);
                    doesCancelAttack |= didthisCardCancelAttack;
                    gameState.cardContextStack.Pop();

                    if (didCardAffectAnything)
                        break;
                }
            }

            foreach (Card durationCard in this.CardsInPlay)
            {
                gameState.cardContextStack.PushCardContext(this, durationCard, CardContextReason.CardReacting);
                if (durationCard.DoReactionToAttackWhileInPlayAcrossTurns(this, gameState))
                {                    
                    doesCancelAttack = true;
                }
                gameState.cardContextStack.Pop();
            }

            return !doesCancelAttack;
        }

        public BagOfCards CardsInPlay
        {
            get
            {
                return this.cardsInPlay;
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

        public BagOfCards AllOwnedCards
        {
            get
            {
                return this.allOwnedCards;
            }
        }
    }    
}
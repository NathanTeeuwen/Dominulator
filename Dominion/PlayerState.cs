using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    class PlayerTurnCounters
    {
        internal int availableActionCount;
        internal int availableBuys;
        internal int availableCoins;
        internal int cardCoinDiscount;
        internal HashSet<Type> cardsBannedFromPurchase = new HashSet<Type>();
        internal int copperAdditionalValue = 0;
               
        internal void InitializeTurn()
        {
            this.availableActionCount = 1;
            this.availableBuys = 1;
            this.cardCoinDiscount = 0;
            this.availableCoins = 0;
            this.copperAdditionalValue = 0;
            this.cardsBannedFromPurchase.Clear();
        }

        internal int AvailableCoins
        {
            get
            {
                return this.availableCoins;
            }
        }


        public void AddCoins(PlayerState playerState, int count)
        {
            if (count > 0)
            {
                this.availableCoins += count;
                playerState.gameLog.PlayerGainedCoin(playerState, count);
            }
        }
    }

    public class PlayerState
    {
        internal int numberOfTurnsPlayed;
        internal readonly IPlayerAction actions;
        internal readonly IGameLog gameLog;

        public IPlayerAction Actions { get { return this.actions; } }
        public int AvailableCoins { get { return this.turnCounters.AvailableCoins; } }
        public BagOfCards Hand { get { return this.hand; } }
        public BagOfCards CardsBeingRevealed { get { return this.cardsBeingRevealed; } }

        internal PlayerTurnCounters turnCounters = new PlayerTurnCounters();

        // all of the cards the player owns.  Always move from one list to the other
        internal ListOfCards deck = new ListOfCards();
        internal ListOfCards discard = new ListOfCards();
        internal ListOfCards cardsBeingPlayed = new ListOfCards();  // a stack for recursion
        internal BagOfCards cardsBeingRevealed = new BagOfCards();
        internal BagOfCards hand = new BagOfCards();
        internal BagOfCards playedCards = new BagOfCards();
        internal Card cardToPass = null;
        internal BagOfCards islandMat = new BagOfCards();
        internal BagOfCards nativeVillageMat = new BagOfCards();

        // persistent Counters
        internal int victoryTokenCount;
        internal int pirateShipTokenCount;

        internal PlayerState(IPlayerAction actions, IGameLog gameLog)
        {
            this.gameLog = gameLog;
            this.actions = actions;
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

        internal Card DrawAndRevealOneCardFromDeck()
        {
            Card card = this.DrawOneCard();
            RevealCard(card);
            if (card != null)
            {
                this.cardsBeingRevealed.AddCard(card);
            }
            return card;
        }

        internal void RevealCard(Card card)
        {

        }

        internal void RevealHand()
        {

        }

        internal int CountCardsPlayedThisTurn
        {
            get
            {
                return this.cardsBeingPlayed.Count + this.playedCards.Count;
            }
        }

        internal int CountCardsInPlay
        {
            get
            {
                return this.cardsBeingPlayed.Where(card => card != null).Count() + this.playedCards.Count;
            }
        }

        internal void AddCoins(int coinAmount)
        {
            this.turnCounters.AddCoins(this, coinAmount);
        }

        internal void DoPlayAction(Card currentCard, GameState gameState, int countTimes = 1)
        {
            if (!currentCard.isAction)
            {
                throw new Exception("Can't play a card that isn't a treasure");
            }

            this.gameLog.PlayedCard(this, currentCard);
            this.gameLog.BeginScope();
            this.cardsBeingPlayed.AddCardToTop(currentCard);
            
            for (int i = 0; i < countTimes; ++i)
            {
                this.turnCounters.availableActionCount += currentCard.plusAction;
                this.turnCounters.availableBuys += currentCard.plusBuy;
                this.AddCoins(currentCard.plusCoin);
                this.victoryTokenCount += currentCard.plusVictoryToken;
                this.DrawAdditionalCardsIntoHand(currentCard.plusCard);

                currentCard.DoSpecializedAction(gameState.players.CurrentPlayer, gameState);                
                if (currentCard.isAttack && currentCard.attackDependsOnPlayerChoice)
                {
                    foreach (PlayerState otherPlayer in gameState.players.OtherPlayers)
                    {
                        if (!otherPlayer.IsAffectedByAttacks(gameState))
                        {
                            continue;
                        }

                        currentCard.DoSpecializedAttack(gameState.players.CurrentPlayer, otherPlayer, gameState);
                    }
                }
            }

            Card cardAfterPlay = this.cardsBeingPlayed.DrawCardFromTop();
            if (cardAfterPlay != null)
            {
                this.playedCards.AddCard(cardAfterPlay);
            }

            this.gameLog.EndScope();
        }

        internal void DoPlayTreasure(Card currentCard, GameState gameState)
        {            
            if (!currentCard.isTreasure)
            {
                throw new Exception("Can't play a card that isn't a treasure");
            }

            this.gameLog.PlayedCard(this, currentCard);
            this.gameLog.BeginScope();
            this.cardsBeingPlayed.AddCardToTop(currentCard);

            this.turnCounters.availableBuys += currentCard.plusBuy;
            this.AddCoins(currentCard.plusCoin);
            if (currentCard.Is<CardTypes.Copper>())
            {
                this.AddCoins(this.turnCounters.copperAdditionalValue);
            }

            currentCard.DoSpecializedAction(gameState.players.CurrentPlayer, gameState);
            Card cardAfterPlay = this.cardsBeingPlayed.DrawCardFromTop();
            if (cardAfterPlay != null)
            {
                this.playedCards.AddCard(cardAfterPlay);
            }

            this.gameLog.EndScope();
        }

        /*
        internal void DoRevealCards(GameState gameState, CardPredicate shouldContinueReveal, MapCardToPlacement MapCardToPlacement)
        {
            DoRevealCards(
                 gameState, 
                 shouldContinueReveal,
                 placeCardsFromList: delegate(BagCards cards)
                 {
                     Card card = cards.RemoveCard();
                     DeckPlacement placement = MapCardToPlacement(card);
                     return new CardPlacementPair(card, placement);
                 });
        }

        internal void DoRevealCards(GameState gameState, CardPredicate shouldContinueReveal, PlaceCardsFromList placeCardsFromList)
        {
            BagCards currentRevealedCards = new BagCards();  // cards in list for further evaluation
            int revealedCardCount = 0;
            Card card = this.DrawOneCard();
            while (card != null && shouldContinueReveal(card))
            {
                this.cardsBeingRevealed.AddCardToTop(card);  //  make cards visible to player
                ++revealedCardCount;
                currentRevealedCards.AddCard(card);
            }

            if (placeCardsFromList != null)
            {
                CardPlacementPair pair = placeCardsFromList(currentRevealedCards);
                while (pair.card != null)
                {
                    PlaceCardFromPlacement(pair, gameState);
                    currentRevealedCards.RemoveCard(card.GetType());
                    pair = placeCardsFromList(currentRevealedCards);
                }
            }

            this.MoveAllCardsToDiscard(currentRevealedCards);
            this.cardsBeingRevealed.RemoveNCardsFromTop(revealedCardCount);
        }*/

        private void PlaceCardFromPlacement(CardPlacementPair pair, GameState gameState)
        {
            switch (pair.placement)
            {
                case DeckPlacement.Discard: this.discard.AddCardToTop(pair.card); break;
                case DeckPlacement.Hand: this.hand.AddCard(pair.card); break;
                case DeckPlacement.Trash: this.MoveCardToTrash(pair.card, gameState); break;
                case DeckPlacement.Play: this.PlayCard(pair.card, gameState); break;
                case DeckPlacement.TopOfDeck: this.deck.AddCardToTop(pair.card); break;
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

        internal Card TrashCardFromHandOfType(GameState gameState, Type cardType, bool guaranteeInHand)
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

        internal void MoveCardToTrash(Card card, GameState gameState)
        {
            // reaction to trashing?
            this.gameLog.PlayerTrashedCard(this, card);
            gameState.trash.AddCard(card);
            this.gameLog.BeginScope();
            card.DoSpecializedTrash(gameState.players.CurrentPlayer, gameState);
            this.gameLog.EndScope();            
        }

        internal Card RemoveCardFromHand(Type cardType)
        {
            return this.hand.RemoveCard(cardType);
        }

        internal void DiscardHandDownToCount(int count)
        {
            while (this.hand.Count > count)
            {
                // TODO:
            }
        }

        internal void DiscardHand()
        {
            // reactions;
            this.MoveAllCardsToDiscard(this.hand);
        }

        internal Card RequestPlayerChooseActionToRemoveFromHandForPlay(GameState gameState, bool isOptional)
        {
            if (!this.hand.HasCard(card => card.isAction))
            {
                return null;
            }

            Type cardTypeToPlay = this.actions.GetActionFromHandToPlay(gameState, isOptional);
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

        internal bool RequestPlayerPlayActionFromHand(GameState gameState, bool isOptional)
        {
            Card cardToPlay = RequestPlayerChooseActionToRemoveFromHandForPlay(gameState, isOptional);

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
            Type type = this.actions.GetCardFromSupplyToGain(gameState, acceptableCard, isOptional);
            if (type == null)
            {
                if (isOptional)
                {
                    return null;
                }
                throw new Exception("Must gain a card where " + description);
            }

            Card gainedCard = gameState.PlayerGainCardFromSupply(type, playerGainingCard, defaultLocation);
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

        internal int RequestPlayerTrashCardsFromHand(GameState gameState, int cardCount, bool isOptional)
        {
            CardPredicate acceptableCardsToTrash = card => true;
            int countCardTrashed = 0;
            while (countCardTrashed < cardCount)
            {
                if (this.RequestPlayerTrashCardFromHand(gameState, acceptableCardsToTrash, isOptional) == null)
                {
                    break;
                }

                ++countCardTrashed;
            }

            return countCardTrashed;
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
            Type cardTypeToTrash = this.actions.GetCardFromHandToTrash(gameState, acceptableCardsToTrash);
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

        internal void RequestPlayerDiscardCardsFromHand(GameState gameState, int count)
        {
            int cardDiscardedCount = 0;
            while (!this.hand.IsEmpty && cardDiscardedCount < count)
            {
                if (!this.RequestPlayerDiscardCardFromHand(gameState, acceptableCard => true, isOptional: false))
                {
                    break;
                }
                ++cardDiscardedCount;
            }
        }

        internal bool RequestPlayerDiscardCardFromHand(GameState gameState, CardPredicate acceptableCardsToTrash, bool isOptional)
        {
            if (!this.hand.HasCard(acceptableCardsToTrash))
            {
                return false;
            }

            Type cardTypeToDiscard = this.actions.GetCardFromHandToDiscard(gameState, isOptional);
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

            Card cardToDiscard = this.hand.RemoveCard(cardTypeToDiscard);
            if (cardToDiscard == null)
            {
                throw new Exception("Could not remove Card From Hand");
            }

            this.DiscardCard(cardToDiscard, gameState);

            return true;
        }

        internal void RequestPlayerPutRevealedCardsBackOnDeck(GameState gameState)
        {
            while (this.cardsBeingRevealed.Any)
            {
                Type cardToPutOnTop = this.actions.GetCardFromRevealedCardsToPutOnDeck(gameState);
                if (cardToPutOnTop == null)
                {
                    throw new Exception("Player must choose a card to put on top of deck");
                }

                this.MoveRevealedCardToTopOfDeck(cardToPutOnTop);
            }
        }        

        private void DiscardCard(Card cardToDiscard, GameState gameState)
        {
            this.gameLog.PlayerDiscardCard(this, cardToDiscard);
            this.discard.AddCardToTop(cardToDiscard);
        }

        internal Card RequestPlayerTopDeckCardFromHand(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            if (!this.hand.HasCard(acceptableCard))
            {
                return null;
            }

            Type cardTypeToTopDeck = this.actions.GetCardFromHandToTopDeck(gameState, acceptableCard);
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

            this.deck.AddCardToTop(cardToTopDeck);

            return cardToTopDeck;
        }

        internal bool RequestPlayerRevealCard(Card card, GameState gameState)
        {
            if (actions.ShouldRevealCard(gameState, card))
            {
                RevealCard(card);
                return true;
            }

            return false;
        }

        internal Card RequestPlayerTopDeckCardFromRevealed(GameState gameState)
        {
            Type cardTypeToTopDeck = this.actions.GetCardFromRevealedCarsToTopDeck(this.cardsBeingRevealed);
            if (cardTypeToTopDeck == null)
            {
                throw new Exception("Must choose a card to top deck");
            }

            Card cardToTopDeck = this.cardsBeingRevealed.RemoveCard(cardTypeToTopDeck);
            if (cardToTopDeck == null)
            {
                throw new Exception("Selected a card that wasn't being revealed");
            }

            this.deck.AddCardToTop(cardToTopDeck);
            return cardToTopDeck;
        }

        internal Card RequestPlayerGiveCardToPassLeft(GameState gameState)
        {
            if (this.hand.IsEmpty)
            {
                return null;
            }

            Type cardTypeToPassLeft = this.actions.GetCardFromHandToPassLeft(gameState);
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

        internal PlayerActionChoice RequestPlayerChooseAction(GameState gameState, IsValidChoice acceptableChoice)
        {
            PlayerActionChoice choice = actions.ChooseAction(gameState, acceptableChoice);

            if (!acceptableChoice(choice))
            {
                throw new Exception("Player made in invalid action choice");
            }

            return choice;
        }

        internal void CleanupPhase()
        {
            this.MoveAllCardsToDiscard(this.playedCards);
            this.MoveAllCardsToDiscard(this.hand);
        }

        internal Card GainCardFromSupply(GameState gameState, Type cardType, DeckPlacement defaultLocation = DeckPlacement.Discard)
        {
            return gameState.PlayerGainCardFromSupply(cardType, this, defaultLocation);
        }

        internal void GainCardsFromSupply(GameState gameState, Type cardType, int count, DeckPlacement defaultLocation = DeckPlacement.Discard)
        {
            for (int i = 0; i < count; ++i)
                gameState.PlayerGainCardFromSupply(cardType, this, defaultLocation);
        }

        internal void GainCard(GameState gameState, Card card, DeckPlacement defaultPlacement = DeckPlacement.Discard, GainReason gainReason = GainReason.Gain)
        {            
            // TODO: check if card in play reacts            
            // TODO: check if there is a reaction in hand for gaining a card
            
            if (gainReason == GainReason.Buy)
            {
                this.gameLog.PlayerBoughtCard(this, card);
            }
            else
            {
                this.gameLog.PlayerGainedCard(this, card);
            }

            this.gameLog.BeginScope();
            card.DoSpecializedWhenGain(this, gameState);
            this.gameLog.EndScope();

            this.PlaceCardFromPlacement(new CardPlacementPair(card, defaultPlacement), gameState);
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
            deck.Shuffle();

            // move Stash to where the user wants
        }

        internal void MoveDeckToDiscard()
        {
            MoveAllCardsToDiscard(this.deck);
        }

        internal void MoveRevealedCardsToDiscard()
        {
            // trigger discard effects
            MoveAllCardsToDiscard(this.cardsBeingRevealed);
        }

        internal void MoveRevealedCardToTrash(Card card, GameState gameState)
        {
            MoveRevealedCardToTrash(card.GetType(), gameState);
        }

        internal void MoveRevealedCardToTrash(Type typeOfCard, GameState gameState)
        {
            Card card = this.cardsBeingRevealed.RemoveCard(typeOfCard);
            if (card == null)
            {
                throw new Exception("Revealed cards did not have the specified card");
            }
            this.MoveCardToTrash(card, gameState);
        }

        internal void MoveRevealedCardToTopOfDeck(Card card)
        {
            MoveRevealedCardToTopOfDeck(card.GetType());
        }

        internal void MoveRevealedCardToTopOfDeck(Type typeOfCard)
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

        internal void MoveRevealedCardToHand(Card card)
        {
            MoveRevealedCardToHand(card.GetType());
        }

        internal void MoveRevealedCardToHand(Type typeOfCard)
        {
            Card card = this.cardsBeingRevealed.RemoveCard(typeOfCard);
            if (card == null)
            {
                throw new Exception("Revealed cards did not have the specified card");
            }
            this.hand.AddCard(card);
        }

        private void MoveAllCardsToDiscard(CollectionCards cards)
        {
            foreach (Card card in cards)
            {
                this.discard.AddCardToTop(card);
            }
            cards.Clear();
        }

        internal bool IsAffectedByAttacks(GameState gameState)
        {
            bool isAffected = true;
            foreach (Card reactionCard in this.hand)
            {
                if (reactionCard.DoReactionToAttack(this, gameState))
                {
                    isAffected = false;
                }
            }

            return isAffected;
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

                foreach (Card card in this.playedCards)
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
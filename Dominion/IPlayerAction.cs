using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public interface IPlayerAction
    {        
        int GetCountToReturnToSupply(Card card, GameState gameState);
        Card BanCardForCurrentPlayerRevealedCards(GameState gameState);
        Card BanCardForCurrentPlayerPurchase(GameState gameState);        
        Card ChooseCardToPlayFirst(GameState gameState, Card card1, Card card2);        
        Card GetTreasureFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GetCardFromSupplyToEmbargo(GameState gameState);
        Card GetCardFromSupplyToPlay(GameState gameState, CardPredicate acceptableCard);
        Card GetCardFromSupplyToBuy(GameState gameState, CardPredicate acceptableCard);
        Card GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GuessCardTopOfDeck(GameState gameState);
        Card NameACard(GameState gameState);
        Card GetCardFromTrashToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional);        
        Card GetCardFromPlayToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GetCardFromDiscardToTopDeck(GameState gameState, PlayerState player, bool isOptional);
        Card GetCardFromRevealedCardsToTopDeck(GameState gameState, PlayerState player);
        Card GetCardFromRevealedCardsToTrash(GameState gameState, CardPredicate acceptableCard);
        Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState, PlayerState player);
        Card GetCardFromRevealedCardsToDiscard(GameState gameState, PlayerState player);
        Card GetCardFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GetCardFromHandToPassLeft(GameState gameState);
        Card GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, PlayerState player, bool isOptional);
        Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard); // always optional
        Card GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional);
        Card GetCardFromHandToIsland(GameState gameState);
        Card GetCardFromHandToDeferToNextTurn(GameState gameState);
        Card GetCardFromHandOrDiscardToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional, out DeckPlacement deckPlacement);
        Card GetCardFromOtherPlayersHandToDiscard(GameState gameState, PlayerState otherPlayer);
        Card GetCardFromOtherPlayersRevealedCardsToTrash(GameState gameState, PlayerState otherPlayer, CardPredicate acceptableCard);
        int GetNumberOfCardsFromDiscardToPutInHand(GameState gameState, int maxNumber);
        bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card);
        bool ShouldPlayerDiscardCardFromHand(GameState gameState, PlayerState player, Card card);
        bool ShouldRevealCardFromHandForCard(GameState gameState, Card card, Card cardFor);
        bool ShouldRevealCardFromHand(GameState gameState, Card card);
        bool ShouldPutCardInHand(GameState gameState, Card card);
        bool WantToResign(GameState gameState);        
        bool ShouldPutDeckInDiscard(GameState gameState);
        bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState);
        bool ShouldTrashCard(GameState gameState, Card card);
        bool ShouldGainCard(GameState gameState, Card card);
        PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice);
        DeckPlacement ChooseBetweenTrashAndTopDeck(GameState gameState, Card card);
        string PlayerName { get; }        
        int GetCoinAmountToOverpayForCard(GameState gameState, Card card);
        int GetCoinAmountToSpendInBuyPhase(GameState gameState);
        int GetCoinAmountToUseInButcher(GameState gameState);
    }

    internal class PlayerActionWithSelf
        : IPlayerAction
    {
        private readonly IPlayerAction playerAction;
        private readonly PlayerState self;

        internal IPlayerAction actions
        {
            get
            {
                return this.playerAction;
            }
        }

        public PlayerActionWithSelf(IPlayerAction playerAction, PlayerState self)
        {
            this.playerAction = playerAction;
            this.self = self;
        }

        public int GetCountToReturnToSupply(Card card, GameState gameState)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCountToReturnToSupply(card, gameState);
            gameState.self = saved;
            return result;
        }

        public Card BanCardForCurrentPlayerRevealedCards(GameState gameState)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.BanCardForCurrentPlayerRevealedCards(gameState);
            gameState.self = saved;
            return result;
        }

        public Card BanCardForCurrentPlayerPurchase(GameState gameState)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.BanCardForCurrentPlayerPurchase(gameState);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromSupplyToEmbargo(GameState gameState)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromSupplyToEmbargo(gameState);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromSupplyToPlay(GameState gameState, CardPredicate acceptableCard)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromSupplyToPlay(gameState, acceptableCard);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromHandToPlay(gameState, acceptableCard, isOptional);
            gameState.self = saved;
            return result;
        }        

        public Card ChooseCardToPlayFirst(GameState gameState, Card card1, Card card2)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.ChooseCardToPlayFirst(gameState, card1, card2);
            gameState.self = saved;
            return result;
        }

        public Card GetTreasureFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetTreasureFromHandToPlay(gameState, acceptableCard, isOptional);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromSupplyToBuy(GameState gameState, CardPredicate acceptableCard)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromSupplyToBuy(gameState, acceptableCard);
            gameState.self = saved;
            return result;
        }

        public Card GuessCardTopOfDeck(GameState gameState)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GuessCardTopOfDeck(gameState);
            gameState.self = saved;
            return result;
        }

        public Card NameACard(GameState gameState)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.NameACard(gameState);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromTrashToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromTrashToGain(gameState, acceptableCard, isOptional);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromSupplyToGain(gameState, acceptableCard, isOptional);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromPlayToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromPlayToTopDeck(gameState, acceptableCard, isOptional);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromDiscardToTopDeck(GameState gameState, PlayerState player, bool isOptional)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromDiscardToTopDeck(gameState, player, isOptional);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromRevealedCardsToTopDeck(GameState gameState, PlayerState player)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromRevealedCardsToTopDeck(gameState, player);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromRevealedCardsToTrash(GameState gameState, CardPredicate acceptableCard)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromRevealedCardsToTrash(gameState, acceptableCard);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState, PlayerState player)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromRevealedCardsToPutOnDeck(gameState, player);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromRevealedCardsToDiscard(GameState gameState, PlayerState player)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromRevealedCardsToDiscard(gameState, player);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromHandToTopDeck(gameState, acceptableCard, isOptional);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromHandToPassLeft(GameState gameState)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromHandToPassLeft(gameState);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, PlayerState player, bool isOptional)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromHandToDiscard(gameState, acceptableCard, player, isOptional);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromHandToReveal(gameState, acceptableCard);
            gameState.self = saved;
            return result;
        }

        // always optional
        public Card GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromHandToTrash(gameState, acceptableCard, isOptional);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromHandToIsland(GameState gameState)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromHandToIsland(gameState);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromHandToDeferToNextTurn(GameState gameState)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromHandToDeferToNextTurn(gameState);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromHandOrDiscardToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional, out DeckPlacement deckPlacement)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromHandOrDiscardToTrash(gameState, acceptableCard, isOptional, out deckPlacement);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromOtherPlayersHandToDiscard(GameState gameState, PlayerState otherPlayer)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromOtherPlayersHandToDiscard(gameState, otherPlayer);
            gameState.self = saved;
            return result;
        }

        public Card GetCardFromOtherPlayersRevealedCardsToTrash(GameState gameState, PlayerState otherPlayer, CardPredicate acceptableCard)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCardFromOtherPlayersRevealedCardsToTrash(gameState, otherPlayer, acceptableCard);
            gameState.self = saved;
            return result;
        }

        public int GetNumberOfCardsFromDiscardToPutInHand(GameState gameState, int maxNumber)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetNumberOfCardsFromDiscardToPutInHand(gameState, maxNumber);
            gameState.self = saved;
            return result;
        }

        public bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.ShouldPlayerDiscardCardFromDeck(gameState, player, card);
            gameState.self = saved;
            return result;
        }

        public bool ShouldPlayerDiscardCardFromHand(GameState gameState, PlayerState player, Card card)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.ShouldPlayerDiscardCardFromHand(gameState, player, card);
            gameState.self = saved;
            return result;
        }

        public bool ShouldRevealCardFromHandForCard(GameState gameState, Card card, Card cardFor)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.ShouldRevealCardFromHandForCard(gameState, card, cardFor);
            gameState.self = saved;
            return result;
        }

        public bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.ShouldRevealCardFromHand(gameState, card);
            gameState.self = saved;
            return result;
        }

        public bool ShouldPutCardInHand(GameState gameState, Card card)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.ShouldPutCardInHand(gameState, card);
            gameState.self = saved;
            return result;
        }

        public bool WantToResign(GameState gameState)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.WantToResign(gameState);
            gameState.self = saved;
            return result;
        }

        public bool ShouldPutDeckInDiscard(GameState gameState)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.ShouldPutDeckInDiscard(gameState);
            gameState.self = saved;
            return result;
        }

        public bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.ShouldPutCardOnTopOfDeck(card, gameState);
            gameState.self = saved;
            return result;
        }

        public bool ShouldTrashCard(GameState gameState, Card card)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.ShouldTrashCard(gameState, card);
            gameState.self = saved;
            return result;
        }

        public bool ShouldGainCard(GameState gameState, Card card)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.ShouldGainCard(gameState, card);
            gameState.self = saved;
            return result;
        }

        public PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.ChooseBetween(gameState, acceptableChoice);
            gameState.self = saved;
            return result;
        }

        public DeckPlacement ChooseBetweenTrashAndTopDeck(GameState gameState, Card card)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.ChooseBetweenTrashAndTopDeck(gameState, card);
            gameState.self = saved;
            return result;
        }

        public string PlayerName
        {
            get
            {
                return this.playerAction.PlayerName;
            }
        }
        public int GetCoinAmountToOverpayForCard(GameState gameState, Card card)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCoinAmountToOverpayForCard(gameState, card);
            gameState.self = saved;
            return result;
        }

        public int GetCoinAmountToSpendInBuyPhase(GameState gameState)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCoinAmountToSpendInBuyPhase(gameState);
            gameState.self = saved;
            return result;
        }

        public int GetCoinAmountToUseInButcher(GameState gameState)
        {
            var saved = gameState.self;
            gameState.self = this.self;
            var result = this.playerAction.GetCoinAmountToUseInButcher(gameState);
            gameState.self = saved;
            return result;
        }
    }
}

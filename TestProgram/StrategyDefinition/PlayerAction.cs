using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public delegate bool GameStatePlayerActionPredicate(GameState gameState, PlayerAction playerAction);    

    public class PlayerAction
        : UnimplementedPlayerAction
    {
        internal readonly string name;        

        internal readonly ICardPicker purchaseOrder;
        internal ICardPicker actionOrder; // readonly
        internal readonly ICardPicker trashOrder;
        internal readonly ICardPicker treasurePlayOrder;
        internal readonly ICardPicker discardOrder;
        internal bool chooseDefaultActionOnNone;
        internal readonly ICardPicker gainOrder;

        internal readonly ICardPicker defaultActionOrder;

        private readonly PlayerActionFromCardResponses defaultCardResponses;
        private readonly MapOfCards<GameStatePlayerActionPredicate> defaultShouldPlay;

        public PlayerAction(
            string name,            
            ICardPicker purchaseOrder,
            ICardPicker actionOrder = null,
            bool chooseDefaultActionOnNone = true,
            ICardPicker treasurePlayOrder = null,
            ICardPicker discardOrder = null,
            ICardPicker trashOrder = null,
            ICardPicker gainOrder = null)
        {            
            this.purchaseOrder = purchaseOrder;
            this.actionOrder = actionOrder == null ? DefaultStrategies.DefaultActionPlayOrder(purchaseOrder) : actionOrder;
            this.discardOrder = discardOrder == null ? DefaultStrategies.DefaultDiscardOrder() : discardOrder;
            this.trashOrder = trashOrder == null ? DefaultStrategies.DefaultTrashOrder(purchaseOrder) : trashOrder;
            this.treasurePlayOrder = treasurePlayOrder == null ? DefaultStrategies.DefaultTreasurePlayOrder() : treasurePlayOrder;            
            this.gainOrder = gainOrder != null ? gainOrder : purchaseOrder;
            this.chooseDefaultActionOnNone = chooseDefaultActionOnNone;
            this.name = name;
            this.defaultActionOrder = DefaultStrategies.DefaultActionPlayOrder(purchaseOrder);

            this.defaultCardResponses = new PlayerActionFromCardResponses(DefaultPlayRules.DefaultResponses.GetCardResponses(this));
            this.defaultShouldPlay = DefaultPlayRules.DefaultResponses.GetCardShouldPlayDefaults(this);
        }                

        public override Card GetCardFromSupplyToBuy(GameState gameState, CardPredicate cardPredicate)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromSupplyToBuy(gameState, cardPredicate);
            }

            var self = gameState.Self;
            return this.purchaseOrder.GetPreferredCard(
                gameState,
                cardPredicate);
        }

        public override Card GetTreasureFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetTreasureFromHandToPlay(gameState, acceptableCard, isOptional);
            }

            var self = gameState.Self;
            return this.treasurePlayOrder.GetPreferredCard(
                gameState,
                card => self.Hand.HasCard(card) && acceptableCard(card));
        }

        private bool DefaultShouldPlayCard(GameState gameState, Card card)
        {
            var predicate = this.defaultShouldPlay[card];
            if (predicate != null)
            {
                return predicate(gameState, this);
            }

            return true;
        }

        public override Card GetCardFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromHandToPlay(gameState, acceptableCard, isOptional);
            }

            var self = gameState.Self;      

            Card result = this.actionOrder.GetPreferredCard(
                gameState,
                card => self.Hand.HasCard(card) && acceptableCard(card),
                card => DefaultShouldPlayCard(gameState, card));

            // only choose default actions that weren't explicitly mentioned in the play order
            if (result == null && this.chooseDefaultActionOnNone)
            {
                var candidateCards = new SetOfCards(gameState.CardGameSubset);
                
                foreach(Card card in self.Hand)
                {
                    if (acceptableCard(card) && DefaultShouldPlayCard(gameState, card))
                        candidateCards.Add(card);
                }

                if (candidateCards.Count > 0)
                {
                    foreach (Card card in this.actionOrder.GetNeededCards())
                    {
                        candidateCards.Remove(card);                        
                    }
                }

                if (candidateCards.Count > 0)
                {
                    result = this.defaultActionOrder.GetPreferredCard(
                        gameState,
                        card => candidateCards.Contains(card));
                }                
            }

            if (result == null && !isOptional)
            {
                result = this.defaultActionOrder.GetPreferredCard(
                    gameState,
                    card => self.Hand.HasCard(card) && acceptableCard(card));
            }

            return result;
        }        

        public override Card GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromHandToTrash(gameState, acceptableCard, isOptional);
            }

            return DefaultGetCardFromHandToTrash(gameState, acceptableCard, isOptional);           
        }

        public Card DefaultGetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var self = gameState.Self;
            Card result = this.trashOrder.GetPreferredCard(
                gameState,
                card => self.Hand.HasCard(card) && acceptableCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null && !isOptional)
            {
                result = self.Hand.Where(c => acceptableCard(c)).OrderBy(c => c, new DefaultPlayRules.CompareCardByFirstToTrash()).FirstOrDefault();
            }

            return result;
        }

        public override Card GetCardFromHandOrDiscardToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional, out DeckPlacement deckPlacement)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromHandOrDiscardToTrash(gameState, acceptableCard, isOptional, out deckPlacement);
            }

            var self = gameState.Self;
            Card result = this.trashOrder.GetPreferredCard(
                gameState,
                card => (self.Hand.HasCard(card) || self.Discard.HasCard(card)) && acceptableCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null && !isOptional)
            {
                result = self.Hand.OrderBy(c => c, new DefaultPlayRules.CompareCardByFirstToTrash()).FirstOrDefault();
            }

            deckPlacement = DeckPlacement.Discard;
            if (result != null)
            {
                if (self.Discard.HasCard(result))
                    deckPlacement = DeckPlacement.Discard;
                else if (self.Hand.HasCard(result))
                    deckPlacement = DeckPlacement.Hand;
                else
                    throw new Exception("Card should have been in hand or discard");
            }

            return result;
        }

        public override Card GetCardFromRevealedCardsToTrash(GameState gameState, CardPredicate acceptableCard)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromRevealedCardsToTrash(gameState, acceptableCard);
            }

            var selfPlayer = gameState.Self;
            Card result = this.trashOrder.GetPreferredCard(
                gameState,
                card => selfPlayer.CardsBeingRevealed.HasCard(card) && acceptableCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null)
            {
                result = selfPlayer.CardsBeingRevealed.Where(c => acceptableCard(c)).OrderBy(c => c, new DefaultPlayRules.CompareCardByFirstToTrash()).FirstOrDefault();                
            }

            return result;
        }

        public override Card GetCardFromRevealedCardsToDiscard(GameState gameState)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromRevealedCardsToDiscard(gameState);
            }

            var self = gameState.Self;
            Card result = this.discardOrder.GetPreferredCard(
                gameState,
                card => self.CardsBeingRevealed.HasCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null)
            {
                result = self.CardsBeingRevealed.OrderBy(c => c, new DefaultPlayRules.CompareCardByFirstToDiscard()).FirstOrDefault();                
            }

            return result;
        }        

        override public Card GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromHandToTopDeck(gameState, acceptableCard, isOptional);
            }

            Card result = this.discardOrder.GetPreferredCard(gameState, card => gameState.Self.Hand.HasCard(card) && acceptableCard(card));
            if (result != null)
            {
                return result;
            }

            if (result == null && !isOptional)
            {
                result = gameState.Self.Hand.Where(c => acceptableCard(c)).OrderBy(c => c, new DefaultPlayRules.CompareCardByFirstToDiscard()).FirstOrDefault();                
            }

            return result;
        }

        override public Card GetCardFromDiscardToTopDeck(GameState gameState, bool isOptional)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromDiscardToTopDeck(gameState, isOptional);
            }

            Card result = this.discardOrder.GetPreferredCardReverse(gameState, card => gameState.Self.Discard.HasCard(card));
            return result;
        }

        public override bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.ShouldPlayerDiscardCardFromDeck(gameState, player, card);
            }

            return !this.discardOrder.DoesCardPickerMatch(gameState, card);            
        }

        public override DeckPlacement ChooseBetweenTrashAndTopDeck(GameState gameState, Card card)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.ChooseBetweenTrashAndTopDeck(gameState, card);
            }

            if (this.purchaseOrder.DoesCardPickerMatch(gameState, card))
                return DeckPlacement.TopOfDeck;
            
            return DeckPlacement.Trash;            
        }

        public override Card GetCardFromOtherPlayersHandToDiscard(GameState gameState, PlayerState otherPlayer)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromOtherPlayersHandToDiscard(gameState, otherPlayer);
            }

            // discard the highest costing action or treasure.
            Card result = otherPlayer.Hand.Where(c => c.isAction || c.isTreasure).OrderByDescending(c => c.DefaultCoinCost).FirstOrDefault();

            // or just discard the highest costing card
            if (result == null)
                result = otherPlayer.Hand.OrderByDescending(c => c.DefaultCoinCost).FirstOrDefault();

            return result;
        }

        public override Card GetCardFromOtherPlayersRevealedCardsToTrash(GameState gameState, PlayerState otherPlayer, CardPredicate acceptableCard)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromOtherPlayersRevealedCardsToTrash(gameState, otherPlayer, acceptableCard);
            }

            // trash the highest costing matching card
            Card result = otherPlayer.CardsBeingRevealed.Where(c => acceptableCard(c)).OrderByDescending(c => c.DefaultCoinCost).FirstOrDefault();            

            return result;
        }        
        
        public override Card GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromHandToDiscard(gameState, acceptableCard, isOptional);
            }

            return DefaultGetCardFromHandToDiscard(gameState, acceptableCard, isOptional);            
        }

        public Card DefaultGetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            Card result = this.discardOrder.GetPreferredCard(
                gameState,
                card => gameState.Self.Hand.HasCard(card) && acceptableCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null && !isOptional)
            {
                result = gameState.Self.Hand.Where(c => acceptableCard(c))
                                            .OrderBy(c => c, new DefaultPlayRules.CompareCardByFirstToDiscard())
                                            .FirstOrDefault();
            }

            return result;
        }       

        public override Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromRevealedCardsToPutOnDeck(gameState);
            }

            Card result = this.discardOrder.GetPreferredCard(
                gameState,
                card => gameState.Self.CardsBeingRevealed.HasCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null)
            {
                result = gameState.Self.CardsBeingRevealed.OrderBy(c => c, new DefaultPlayRules.CompareCardByFirstToDiscard()).FirstOrDefault();                
            }

            return result;
        }

        public override Card GetCardFromTrashToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromTrashToGain(gameState, acceptableCard, isOptional);
            }

            var self = gameState.Self;

            Card result = this.gainOrder.GetPreferredCard(
                gameState,
                card => acceptableCard(card) && gameState.trash.HasCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null && !isOptional)
            {
                result = gameState.trash.Where(c => acceptableCard(c))
                                     .OrderBy(c => c, new DefaultPlayRules.CompareCardByFirstToGain())
                                     .FirstOrDefault();
            }

            return result;
        }

        public override Card GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromSupplyToGain(gameState, acceptableCard, isOptional);
            }

            return DefaultGetCardFromSupplyToGain(gameState, acceptableCard, isOptional);
        }

        public Card DefaultGetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var self = gameState.Self;

            Card result = this.gainOrder.GetPreferredCard(
                gameState,
                card => acceptableCard(card) && gameState.GetPile(card).Any);

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null && !isOptional)
            {
                result = gameState.supplyPiles.Where(supplyPile => !supplyPile.IsEmpty)
                                     .Select(pile => pile.ProtoTypeCard)
                                     .Where(c => acceptableCard(c))
                                     .OrderBy(c => c, new DefaultPlayRules.CompareCardByFirstToGain())
                                     .FirstOrDefault();
            }

            return result;
        }      
       
        public override string PlayerName
        {
            get
            {
                return this.name;
            }
        }

        public override bool ShouldGainCard(GameState gameState, Card card)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.ShouldGainCard(gameState, card);
            }

            return this.gainOrder.GetPreferredCard(gameState, c => c.Equals(card)) != null;
        }

        public override bool ShouldPlayerDiscardCardFromHand(GameState gameState, Card card)
        {
            if (this.defaultCardResponses.ShouldDeferForCard(card))
            {
                return this.defaultCardResponses.ShouldPlayerDiscardCardFromHand(gameState, card);
            }
            
            return base.ShouldPlayerDiscardCardFromHand(gameState, card);
        }

        public override int GetCoinAmountToUseInButcher(GameState gameState)
        {
            Card cardToTrash = Strategy.WhichCardFromInHand(this.trashOrder, gameState);
            if (cardToTrash == null)
                return 0;

            int cardCost = CostOfCard(cardToTrash, gameState);
            return GetCoinAmountToSpend(gameState, numberOfGains: 1, minCoins: cardCost, maxCoins: cardCost);
        }

        public override int GetCoinAmountToSpendInBuyPhase(GameState gameState)
        {
            PlayerState self = gameState.Self;
            int numberOfBuys = self.AvailableBuys;
            int availableCoins = self.AvailableCoins;

            return GetCoinAmountToSpend(gameState, numberOfBuys, minCoins:0, maxCoins:availableCoins);
        }

        public int GetCoinAmountToSpend(GameState gameState, int numberOfGains, int minCoins, int maxCoins)
        {
            PlayerState self = gameState.Self;
            int availableCoins = maxCoins;
            int coinTokensRemaining = self.AvailableCoinTokens;

            int result = 0;

            CardPredicate shouldGainCard = delegate(Card card)
            {
                int currentCardCost = card.CurrentCoinCost(self);
                
                return currentCardCost >= minCoins &&
                       currentCardCost <= availableCoins + coinTokensRemaining &&
                       this.gainOrder.AmountWillingtoOverPayFor(card, gameState) >= currentCardCost - availableCoins;
            };

            while (coinTokensRemaining > 0 && numberOfGains >= 1)
            {
                Card cardType = this.gainOrder.GetPreferredCard(gameState, shouldGainCard);                    
                if (cardType == null)
                    break;

                availableCoins -= CostOfCard(cardType, gameState); 
                if (availableCoins < 0)
                {
                    int coinSpent = -availableCoins;
                    availableCoins = 0;

                    coinTokensRemaining -= coinSpent;
                    result += coinSpent;                    
                    System.Diagnostics.Debug.Assert(coinTokensRemaining >= 0);                    
                }

                numberOfGains -= 1;
            }

            return result;
        }

        public override bool ShouldTrashCard(GameState gameState, Card card)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.ShouldTrashCard(gameState, card);
            }

            return this.trashOrder.DoesCardPickerMatch(gameState, card);
        }        
    
        public static int CostOfCard(Card cardType, GameState gameState)
        {
            return cardType.CurrentCoinCost(gameState.Self);
        }

        public static void SetKingdomCards(GameConfigBuilder builder, params PlayerAction[] players)
        {
            var allCards = new List<Card>();
            foreach (PlayerAction player in players)
            {
                AddCards(allCards, player.actionOrder);
                AddCards(allCards, player.purchaseOrder);
                AddCards(allCards, player.gainOrder);
            }

            builder.SetKingdomPiles(allCards);
        }

        private static void AddCards(List<Card> cardSet, ICardPicker matchingCards)
        {
            foreach (Card card in matchingCards.GetNeededCards())
            {                
                cardSet.Add(card);
            }
        }

        // default responses from cards
        
        public override int GetCountToReturnToSupply(Card card, GameState gameState)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCountToReturnToSupply(card, gameState);
            }

            return base.GetCountToReturnToSupply(card, gameState);
        }

        public override Card BanCardToDrawnIntoHandFromRevealedCards(GameState gameState)
        {            
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.BanCardToDrawnIntoHandFromRevealedCards(gameState);
            }

            return gameState.players.CurrentPlayer.CardsBeingRevealed.OrderBy(card => card, new DefaultPlayRules.CompareCardForBanningForDrawIntoHand(gameState)).FirstOrDefault();            
        }

        public override Card BanCardForCurrentPlayerPurchase(GameState gameState)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.BanCardForCurrentPlayerPurchase(gameState);
            }

            return Cards.Province;
        }

        public override Card ChooseCardToPlayFirst(GameState gameState, Card card1, Card card2)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.ChooseCardToPlayFirst(gameState, card1, card2);
            }

            return base.ChooseCardToPlayFirst(gameState, card1, card2);
        }        

        public override Card GetCardFromSupplyToEmbargo(GameState gameState)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromSupplyToEmbargo(gameState);
            }

            return base.GetCardFromSupplyToEmbargo(gameState);
        }

        public override Card GetCardFromSupplyToPlay(GameState gameState, CardPredicate acceptableCard)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromSupplyToPlay(gameState, acceptableCard);
            }

            return base.GetCardFromSupplyToPlay(gameState, acceptableCard);
        }              

        public override Card GuessCardTopOfDeck(GameState gameState)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GuessCardTopOfDeck(gameState);
            }

            return base.GuessCardTopOfDeck(gameState);
        }

        public override Card NameACard(GameState gameState)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.NameACard(gameState);
            }

            return base.NameACard(gameState);
        }        

        public override Card GetCardFromPlayToTopDeckDuringCleanup(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            if (this.defaultCardResponses.ShouldDeferForCardBeingCleanedUp(gameState))
            {
                return this.defaultCardResponses.GetCardFromPlayToTopDeckDuringCleanup(gameState, acceptableCard, isOptional);
            }

            return base.GetCardFromPlayToTopDeckDuringCleanup(gameState, acceptableCard, isOptional);
        }

        public override Card GetCardFromRevealedCardsToTopDeck(GameState gameState)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromRevealedCardsToTopDeck(gameState);
            }

            // should throw not implemented?
            return null;
        }

        public override Card GetCardFromHandToDeferToNextTurn(GameState gameState)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromHandToDeferToNextTurn(gameState);
            }

            return base.GetCardFromHandToDeferToNextTurn(gameState);
        }        

        public override Card GetCardFromHandToIsland(GameState gameState)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromHandToIsland(gameState);
            }

            return base.GetCardFromHandToIsland(gameState);
        }

        public override Card GetCardFromHandToPassLeft(GameState gameState)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromHandToPassLeft(gameState);
            }

            return base.GetCardFromHandToPassLeft(gameState);
        }       

        public override Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetCardFromHandToReveal(gameState, acceptableCard);
            }

            return base.GetCardFromHandToReveal(gameState, acceptableCard);
        }                                        

        public override int GetNumberOfCoppersToPutInHandForCountingHouse(GameState gameState, int maxNumber)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.GetNumberOfCoppersToPutInHandForCountingHouse(gameState, maxNumber);
            }

            return maxNumber;
        }

        public override bool ShouldRevealCardFromHandForCard(GameState gameState, Card card, Card cardFor)
        {
            if (this.defaultCardResponses.ShouldDeferForCard(card))
            {
                return this.defaultCardResponses.ShouldRevealCardFromHandForCard(gameState, card, cardFor);
            }            

            return base.ShouldRevealCardFromHand(gameState, card);
        }

        public override bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            if (this.defaultCardResponses.ShouldDeferForCard(card))
            {
                return this.defaultCardResponses.ShouldRevealCardFromHand(gameState, card);
            }

            return base.ShouldRevealCardFromHand(gameState, card);
        }        

        public override bool ShouldPutCardInHand(GameState gameState, Card card)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.ShouldPutCardInHand(gameState, card);
            }

            return base.ShouldPutCardInHand(gameState, card);
        }        

        public override bool ShouldPutDeckInDiscard(GameState gameState)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.ShouldPutDeckInDiscard(gameState);
            }

            return base.ShouldPutDeckInDiscard(gameState);
        }

        public override bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState)
        {
            if (this.defaultCardResponses.ShouldDeferForCard(card))
            {
                return this.defaultCardResponses.ShouldPutCardOnTopOfDeck(card, gameState);
            }

            return base.ShouldPutCardOnTopOfDeck(card, gameState);
        }      

        public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {
            if (this.defaultCardResponses.ShouldDeferForCardInPlay(gameState))
            {
                return this.defaultCardResponses.ChooseBetween(gameState, acceptableChoice);
            }

            return base.ChooseBetween(gameState, acceptableChoice);
        }
            

        public override int GetCoinAmountToOverpayForCard(GameState gameState, Card card)
        {
            if (this.defaultCardResponses.ShouldDeferForCard(card))
            {
                return this.defaultCardResponses.GetCoinAmountToOverpayForCard(gameState, card);
            }

            return base.GetCoinAmountToOverpayForCard(gameState, card);
        }        
    }

    public static class PlayerActionExtensionMethods
    {
        public static bool IsGainingCard(this PlayerAction playerAction, Card card, GameState gameState)
        {
            return playerAction.purchaseOrder.DoesCardPickerMatch(gameState, card);
        }
    }
}

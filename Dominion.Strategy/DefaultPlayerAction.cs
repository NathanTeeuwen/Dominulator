using Dominion;
using Dominion.Strategy.Description;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.Strategy
{
    public class DerivedPlayerAction
        : DerivedPlayerAction<DefaultPlayerAction>
    {
        public DerivedPlayerAction(DefaultPlayerAction playerAction)
            : base(playerAction)
        {            
        }
    }    

    public class DefaultPlayerAction
        : UnimplementedPlayerAction
    {
        public readonly string name;

        public readonly ICardPicker purchaseOrder;
        public ICardPicker actionOrder; // readonly
        public readonly ICardPicker trashOrder;
        public readonly ICardPicker treasurePlayOrder;
        public readonly ICardPicker discardOrder;
        public bool chooseDefaultActionOnNone;
        public bool enablePenultimateProvinceRule;
        public readonly ICardPicker gainOrder;

        public readonly ICardPicker defaultActionOrder;        
        private readonly MapOfCards<GameStatePlayerActionPredicate> defaultShouldPlay;

        public DefaultPlayerAction(
            string name,            
            ICardPicker purchaseOrder,
            ICardPicker actionOrder,
            bool chooseDefaultActionOnNone,
            bool enablePenultimateProvinceRule,
            ICardPicker treasurePlayOrder,
            ICardPicker discardOrder,
            ICardPicker trashOrder,
            ICardPicker gainOrder)
        {            
            this.purchaseOrder = purchaseOrder;
            this.actionOrder = actionOrder == null ? DefaultStrategies.DefaultActionPlayOrder(purchaseOrder) : actionOrder;
            this.discardOrder = discardOrder == null ? DefaultStrategies.DefaultDiscardOrder() : discardOrder;
            this.trashOrder = trashOrder == null ? DefaultStrategies.DefaultTrashOrder(purchaseOrder) : trashOrder;
            this.treasurePlayOrder = treasurePlayOrder == null ? DefaultStrategies.DefaultTreasurePlayOrder() : treasurePlayOrder;            
            this.gainOrder = gainOrder != null ? gainOrder : purchaseOrder;
            this.chooseDefaultActionOnNone = chooseDefaultActionOnNone;
            this.enablePenultimateProvinceRule = enablePenultimateProvinceRule;
            this.name = name;
            this.defaultActionOrder = DefaultStrategies.DefaultActionPlayOrder(purchaseOrder);            
            this.defaultShouldPlay = DefaultPlayRules.DefaultResponses.GetCardShouldPlayDefaults(this);
        }                

        public override Card GetCardFromSupplyToBuy(GameState gameState, CardPredicate cardPredicate)
        {        
            Card preferredCard = this.purchaseOrder.GetPreferredCard(
                gameState,
                cardPredicate);

            if (!this.enablePenultimateProvinceRule)
                return preferredCard;

            // implement some version of PPR
            if (preferredCard != Cards.Province)
            {
                return preferredCard;
            }

            int countProvincesRemaining = Strategy.CountOfPile(Cards.Province, gameState);
            if (countProvincesRemaining == 1)
            {
                // if you are down more than the points of a province
                // and you buy the last province, you will lose instantly.  don't do that.
                int curScoreDiff = gameState.SmallestScoreDifference(gameState.Self);
                if (curScoreDiff < 0 && (-curScoreDiff) > Cards.Province.VictoryPoints(gameState.Self))
                {
                    return this.purchaseOrder.GetPreferredCard(gameState, card => card != Cards.Province && cardPredicate(card));
                }
            }
            else if (countProvincesRemaining == 2)
            {
                // do not buy the last province if you are currently losing by just a little bit.  
                // the other play can buy the province and just win it.
                int curScoreDiff = gameState.SmallestScoreDifference(gameState.Self);
                if (curScoreDiff < 0 && (-curScoreDiff) < Cards.Province.VictoryPoints(gameState.Self))
                {
                    return this.purchaseOrder.GetPreferredCard(gameState, card => card != Cards.Province && cardPredicate(card));
                }
            }

            return preferredCard;
        }

        public override Card GetTreasureFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
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

        public override Card GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional, CollectionCards cardsTrashedSoFar)
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
            Card result = this.discardOrder.GetPreferredCardReverse(gameState, card => gameState.Self.Discard.HasCard(card));
            return result;
        }

        public override bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card)
        {            
            return !this.discardOrder.DoesCardPickerMatch(gameState, card);            
        }

        public override DeckPlacement ChooseBetweenTrashAndTopDeck(GameState gameState, Card card)
        {            
            if (this.purchaseOrder.DoesCardPickerMatch(gameState, card))
                return DeckPlacement.TopOfDeck;
            
            return DeckPlacement.Trash;            
        }

        public override Card GetCardFromOtherPlayersHandToDiscard(GameState gameState, PlayerState otherPlayer)
        {            
            // discard the highest costing action or treasure.
            Card result = otherPlayer.Hand.Where(c => c.isAction || c.isTreasure).OrderByDescending(c => c.DefaultCoinCost).FirstOrDefault();

            // or just discard the highest costing card
            if (result == null)
                result = otherPlayer.Hand.OrderByDescending(c => c.DefaultCoinCost).FirstOrDefault();

            return result;
        }

        public override Card GetCardFromOtherPlayersRevealedCardsToTrash(GameState gameState, PlayerState otherPlayer, CardPredicate acceptableCard)
        {            
            // trash the highest costing matching card
            Card result = otherPlayer.CardsBeingRevealed.Where(c => acceptableCard(c)).OrderByDescending(c => c.DefaultCoinCost).FirstOrDefault();            

            return result;
        }        
        
        public override Card GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, bool isOptional)
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
            return this.gainOrder.GetPreferredCard(gameState, c => c.Equals(card)) != null;
        }

        public override bool ShouldPlayerDiscardCardFromHand(GameState gameState, Card card)
        {                        
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
            return this.trashOrder.DoesCardPickerMatch(gameState, card);
        }        
    
        public static int CostOfCard(Card cardType, GameState gameState)
        {
            return cardType.CurrentCoinCost(gameState.Self);
        }                

        // default responses from cards
        
        public override int GetCountToReturnToSupply(Card card, GameState gameState)
        {            
            return base.GetCountToReturnToSupply(card, gameState);
        }

        public override Card BanCardToDrawnIntoHandFromRevealedCards(GameState gameState)
        {                        
            return gameState.players.CurrentPlayer.CardsBeingRevealed.OrderBy(card => card, new DefaultPlayRules.CompareCardForBanningForDrawIntoHand(gameState)).FirstOrDefault();            
        }

        public override Card BanCardForCurrentPlayerPurchase(GameState gameState)
        {            
            return Cards.Province;
        }

        public override Card ChooseCardToPlayFirst(GameState gameState, Card card1, Card card2)
        {            
            return base.ChooseCardToPlayFirst(gameState, card1, card2);
        }        

        public override Card GetCardFromSupplyToEmbargo(GameState gameState)
        {            
            return base.GetCardFromSupplyToEmbargo(gameState);
        }

        public override Card GetCardFromSupplyToPlay(GameState gameState, CardPredicate acceptableCard)
        {            
            return base.GetCardFromSupplyToPlay(gameState, acceptableCard);
        }              

        public override Card GuessCardTopOfDeck(GameState gameState)
        {            
            return base.GuessCardTopOfDeck(gameState);
        }

        public override Card NameACard(GameState gameState)
        {            
            return base.NameACard(gameState);
        }        

        public override Card GetCardFromPlayToTopDeckDuringCleanup(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {            
            return base.GetCardFromPlayToTopDeckDuringCleanup(gameState, acceptableCard, isOptional);
        }

        public override Card GetCardFromRevealedCardsToTopDeck(GameState gameState, bool isOptional)
        {
            if (isOptional)
                return null;

            return gameState.Self.CardsBeingRevealed.SomeCard();
        }

        public override Card GetCardFromHandToDeferToNextTurn(GameState gameState)
        {            
            return base.GetCardFromHandToDeferToNextTurn(gameState);
        }        

        public override Card GetCardFromHandToIsland(GameState gameState)
        {            
            return base.GetCardFromHandToIsland(gameState);
        }

        public override Card GetCardFromHandToPassLeft(GameState gameState)
        {            
            return base.GetCardFromHandToPassLeft(gameState);
        }       

        public override Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
        {         
            return base.GetCardFromHandToReveal(gameState, acceptableCard);
        }                                        

        public override int GetNumberOfCoppersToPutInHandForCountingHouse(GameState gameState, int maxNumber)
        {            
            return maxNumber;
        }

        public override bool ShouldRevealCardFromHandForCard(GameState gameState, Card card, Card cardFor)
        {            
            return base.ShouldRevealCardFromHand(gameState, card);
        }

        public override bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {            
            return base.ShouldRevealCardFromHand(gameState, card);
        }        

        public override bool ShouldPutCardInHand(GameState gameState, Card card)
        {            
            return base.ShouldPutCardInHand(gameState, card);
        }        

        public override bool ShouldPutDeckInDiscard(GameState gameState)
        {            
            return base.ShouldPutDeckInDiscard(gameState);
        }

        public override bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState)
        {            
            return base.ShouldPutCardOnTopOfDeck(card, gameState);
        }      

        public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {                        
            return base.ChooseBetween(gameState, acceptableChoice);
        }

        public override DeckPlacement ChooseBetweenTrashTopDeckDiscard(GameState gameState, Card card)
        {                
            return base.ChooseBetweenTrashTopDeckDiscard(gameState, card);
        }

        public override int GetCoinAmountToOverpayForCard(GameState gameState, Card card)
        {            
            return base.GetCoinAmountToOverpayForCard(gameState, card);
        }        
    }

    public static class PlayerActionExtensionMethods
    {
        public static bool IsGainingCard(this PlayerAction playerAction, Card card, GameState gameState)
        {
            return playerAction.purchaseOrder.DoesCardPickerMatch(gameState, card);
        }

        public static bool IsGainingCard(this DefaultPlayerAction playerAction, Card card, GameState gameState)
        {
            return playerAction.purchaseOrder.DoesCardPickerMatch(gameState, card);
        }
    }
}

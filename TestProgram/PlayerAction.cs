using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public class PlayerAction
        : DefaultPlayerAction
    {
        internal readonly string name;
        internal readonly int playerIndex = -1;

        protected readonly ICardPicker purchaseOrder;
        protected readonly ICardPicker actionOrder;
        protected readonly ICardPicker trashOrder;
        protected readonly ICardPicker treasurePlayOrder;
        protected readonly ICardPicker discardOrder;
        protected bool chooseDefaultActionOnNone;
        protected readonly ICardPicker gainOrder;

        protected readonly ICardPicker defaultActionOrder;

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
            this.actionOrder = actionOrder == null ? Strategies.Default.DefaultActionPlayOrder(purchaseOrder) : actionOrder;
            this.discardOrder = discardOrder == null ? Strategies.Default.DefaultDiscardOrder() : discardOrder;
            this.trashOrder = trashOrder == null ? Strategies.Default.DefaultTrashOrder() : trashOrder;
            this.treasurePlayOrder = treasurePlayOrder == null ? Strategies.Default.DefaultTreasurePlayOrder() : treasurePlayOrder;            
            this.gainOrder = gainOrder != null ? gainOrder : purchaseOrder;
            this.chooseDefaultActionOnNone = chooseDefaultActionOnNone;
            this.name = name;
            this.defaultActionOrder = Strategies.Default.DefaultActionPlayOrder(purchaseOrder);
        }

        public static int PlayIndexfor(PlayerState playerState)
        {
            return ((PlayerAction)playerState.Actions).playerIndex;
        }

        public override Card GetCardFromSupplyToBuy(GameState gameState, CardPredicate cardPredicate)
        {
            var self = gameState.Self;
            return this.purchaseOrder.GetPreferredCard(
                gameState,
                cardPredicate);
        }

        public override Card GetTreasureFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var self = gameState.Self;
            return this.treasurePlayOrder.GetPreferredCard(
                gameState,
                card => self.Hand.HasCard(card) && acceptableCard(card));
        }

        public override Card ChooseCardToPlayFirst(GameState gameState, Card card1, Card card2)
        {
            Card result = this.actionOrder.GetPreferredCard(
                gameState,
                card => card == card1 || card == card2);

            // choose a reasonable default
            if (result == null)
            {
                result = this.defaultActionOrder.GetPreferredCard(
                    gameState,
                    card => card == card1 || card == card2);
            }

            return result;
        }

        public override Card GetCardFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var self = gameState.Self;      

            Card result = this.actionOrder.GetPreferredCard(
                gameState,
                card => self.Hand.HasCard(card) && acceptableCard(card));

            if (result == null && this.chooseDefaultActionOnNone)
            {
                var candidateCards = new SetOfCards(gameState.CardGameSubset);
                
                foreach(Card card in self.Hand)
                {
                    if (acceptableCard(card))
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

        public override Card BanCardForCurrentPlayerPurchase(GameState gameState)
        {
            return Cards.Province;
        }        

        public override Card BanCardForCurrentPlayerRevealedCards(GameState gameState)
        {
            return gameState.players.CurrentPlayer.CardsBeingRevealed.OrderBy(card => card, new CompareCardForBanning(gameState)).FirstOrDefault();
        }

        class CompareCardForBanning
            : IComparer<Card>
        {
            GameState gameState;

            public CompareCardForBanning(GameState gameState)
            {
                this.gameState = gameState;
            }

            public int Compare(Card first, Card second)
            {
                // no available actions.  Prefer to ban actions over non actions.
                if (gameState.players.CurrentPlayer.AvailableActions == 0)
                {                                        
                    if (first.isAction ^ second.isAction)
                    {
                        return first.isAction ? -1 : 1;
                    }
                }

                bool firstIsUseful = first.isTreasure || first.isAction;
                bool secondIsUseful = second.isTreasure || second.isAction;
                // only one of the cards is useful
                if (firstIsUseful ^ secondIsUseful)
                {
                    return firstIsUseful ? -1 : 1;
                }                                

                int firstCost = first.CurrentCoinCost(gameState.players.CurrentPlayer);
                int secondCost = second.CurrentCoinCost(gameState.players.CurrentPlayer);
                if (firstCost != secondCost)
                {
                    // ban most expensive card first.
                    return firstCost > secondCost ? -1 : 1;
                }

                // dont care on order
                return 0;
            }
        }

        public override Card GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var self = gameState.Self;
            Card result = this.trashOrder.GetPreferredCard(
                gameState,
                card => self.Hand.HasCard(card) && acceptableCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null && !isOptional)
            {
                result = self.Hand.Where(c => acceptableCard(c)).OrderBy(c => c, new CompareCardByFirstToTrash()).FirstOrDefault();                
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
                result = self.Hand.OrderBy(c => c, new CompareCardByFirstToTrash()).FirstOrDefault();
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
                result = selfPlayer.CardsBeingRevealed.Where(c => acceptableCard(c)).OrderBy(c => c, new CompareCardByFirstToTrash()).FirstOrDefault();                
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
                result = self.CardsBeingRevealed.OrderBy(c => c, new CompareCardByFirstToDiscard()).FirstOrDefault();                
            }

            return result;
        }

        public override Card GetCardFromRevealedCardsToTopDeck(GameState gameState)
        {
            BagOfCards revealedCards = gameState.Self.CardsBeingRevealed;
            // good for cartographer, not sure about anyone else.
            foreach (Card card in revealedCards)
            {
                bool shouldDiscard = card.isVictory || card == Cards.Copper;
                if (!shouldDiscard)
                {
                    return card;
                }
            }

            return null;
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
                result = gameState.Self.Hand.Where(c => acceptableCard(c)).OrderBy(c => c, new CompareCardByFirstToDiscard()).FirstOrDefault();                
            }

            return result;
        }

        override public Card GetCardFromDiscardToTopDeck(GameState gameState, bool isOptional)
        {
            Card result = this.discardOrder.GetPreferredCardReverse(gameState, card => gameState.Self.Discard.HasCard(card));
            return result;
        }

        public override bool ShouldPutCardInHand(GameState gameState, Card card)
        {
            return !DoesCardPickerMatch(this.discardOrder, gameState, card);            
        }

        public override bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card)
        {
            return !DoesCardPickerMatch(this.discardOrder, gameState, card);            
        }

        public override DeckPlacement ChooseBetweenTrashAndTopDeck(GameState gameState, Card card)
        {
            if (DoesCardPickerMatch(this.purchaseOrder, gameState, card))
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

        public override bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            if (card == Cards.Watchtower)
                return true;
            if (card == Cards.HorseTraders)
                return true;

            return base.ShouldRevealCardFromHand(gameState, card);
        }

        public override bool ShouldRevealCardFromHandForCard(GameState gameState, Card card, Card cardFor)
        {
            if (card == Cards.Trader)
            {
                return DoesCardPickerMatch(this.trashOrder, gameState, cardFor) && !DoesCardPickerMatch(this.purchaseOrder, gameState, cardFor);
            }

            return base.ShouldRevealCardFromHand(gameState, card);
        }

        struct CompareCardByFirstToTrash
            : IComparer<Card>
        {
            public int Compare(Card x, Card y)
            {
                if (x.isCurse ^ y.isCurse)
                {
                    return x.isCurse ? -1 : 1;
                }

                if (x.isAction ^ y.isAction)
                {
                    return x.isAction ? -1 : 1;
                }

                if (x.isTreasure ^ y.isTreasure)
                {
                    return x.isTreasure ? -1 : 1;
                }

                return x.DefaultCoinCost < y.DefaultCoinCost ? -1 :
                       x.DefaultCoinCost > y.DefaultCoinCost ? 1 :
                       0;
            }
        }

        public override Card GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            Card result = this.discardOrder.GetPreferredCard(
                gameState,
                card => gameState.Self.Hand.HasCard(card) && acceptableCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null)
            {
                result = gameState.Self.Hand.Where(c => acceptableCard(c))
                                            .OrderBy(c => c, new CompareCardByFirstToDiscard())
                                            .FirstOrDefault();
            }

            return result;
        }

        struct CompareCardByFirstToDiscard
            : IComparer<Card>
        {
            public int Compare(Card x, Card y)
            {
                if (x.isCurse ^ y.isCurse)
                {
                    return x.isCurse ? -1 : 1;
                }

                if (x.isAction ^ y.isAction)
                {
                    return x.isAction ? 1 : -1;
                }

                if (x.isVictory ^ y.isVictory)
                {
                    return x.isVictory ? -1 : 1;
                }

                return x.DefaultCoinCost < y.DefaultCoinCost ? -1 :
                       x.DefaultCoinCost > y.DefaultCoinCost ? 1 :
                       0;
            }
        }

        public override Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState)
        {
            Card result = this.discardOrder.GetPreferredCard(
                gameState,
                card => gameState.Self.CardsBeingRevealed.HasCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null)
            {
                result = gameState.Self.CardsBeingRevealed.OrderBy(c => c, new CompareCardByFirstToDiscard()).FirstOrDefault();                
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
                                     .OrderBy(c => c, new CompareCardByFirstToGain())
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

            // do a reasonable default for gainning copper on ill gotten gain
            if (result == null &&
                acceptableCard(Cards.Copper) &&
                Strategies.CardBeingPlayedIs(Cards.IllGottenGains, gameState))
            {
                if (ShouldGainCopper(gameState, this.purchaseOrder))
                {
                    result = Cards.Copper;
                }
            }

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null && !isOptional)
            {
                result = gameState.supplyPiles.Where(supplyPile => !supplyPile.IsEmpty)
                                     .Select(pile => pile.ProtoTypeCard)
                                     .Where(c => acceptableCard(c))
                                     .OrderBy(c => c, new CompareCardByFirstToGain())
                                     .FirstOrDefault();                
            }

            return result;
        }        

        private static bool ShouldGainCopper(GameState gameState, ICardPicker gainOrder)
        {
            PlayerState self = gameState.Self;

            int minValue = self.ExpectedCoinValueAtEndOfTurn;
            int maxValue = minValue + Strategies.CountInHand(Cards.IllGottenGains, gameState);

            if (maxValue == minValue)
                return false;

            CardPredicate shouldGainCard = delegate(Card card)
            {
                int currentCardCost = card.CurrentCoinCost(self);

                return currentCardCost >= minValue &&
                        currentCardCost <= maxValue;
            };

            Card cardType = gainOrder.GetPreferredCard(gameState, shouldGainCard);
            if (cardType == null)
                return false;

            int coppersToGain = PlayerAction.CostOfCard(cardType, gameState) - minValue;

            return (coppersToGain > 0);
        }

        private static bool DoesCardPickerMatch(ICardPicker pickOrder, GameState gameState, Card card)
        {
            return pickOrder.GetPreferredCard(gameState, c => c == card) != null;
        }

        struct CompareCardByFirstToGain
            : IComparer<Card>
        {
            public int Compare(Card x, Card y)
            {
                if (x.isCurse ^ y.isCurse)
                {
                    return x.isCurse ? 1 : -1;
                }

                if (x.isRuins ^ y.isRuins)
                {
                    return x.isRuins ? 1 : -1;
                }

                if (x.isTreasure ^ y.isTreasure)
                {
                    return x.isTreasure ? -1 : 1;
                }

                return x.DefaultCoinCost > y.DefaultCoinCost ? -1 :
                       x.DefaultCoinCost < y.DefaultCoinCost ? 1 :
                       0;
            }
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
            if (card == Cards.MarketSquare)
                return true;

            return base.ShouldPlayerDiscardCardFromHand(gameState, card);
        }

        public override int GetCoinAmountToUseInButcher(GameState gameState)
        {
            Card cardToTrash = Strategies.WhichCardFromInHand(this.trashOrder, gameState);
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
    }    
}

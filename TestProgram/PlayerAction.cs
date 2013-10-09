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
        internal readonly int playerIndex;

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
            int playerIndex,
            ICardPicker purchaseOrder,
            ICardPicker actionOrder = null,
            bool chooseDefaultActionOnNone = true,
            ICardPicker treasurePlayOrder = null,
            ICardPicker discardOrder = null,
            ICardPicker trashOrder = null,
            ICardPicker gainOrder = null)
        {
            this.playerIndex = playerIndex;
            this.purchaseOrder = purchaseOrder;
            this.actionOrder = actionOrder == null ? Strategies.Default.ActionPlayOrder(purchaseOrder) : actionOrder;
            this.discardOrder = discardOrder == null ? Strategies.Default.DefaultDiscardOrder() : discardOrder;
            this.trashOrder = trashOrder == null ? Strategies.Default.DefaultTrashOrder() : trashOrder;
            this.treasurePlayOrder = treasurePlayOrder == null ? Strategies.Default.TreasurePlayOrder() : treasurePlayOrder;            
            this.gainOrder = gainOrder != null ? gainOrder : purchaseOrder;
            this.chooseDefaultActionOnNone = chooseDefaultActionOnNone;
            this.name = name;
            this.defaultActionOrder = Strategies.Default.ActionPlayOrder(purchaseOrder);
        }

        public static int PlayIndexfor(IPlayerAction playerAction)
        {
            return ((PlayerAction)playerAction).playerIndex;
        }

        public override Card GetCardFromSupplyToBuy(GameState gameState, CardPredicate cardPredicate)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            return this.purchaseOrder.GetPreferredCard(
                gameState,
                card => currentPlayer.AvailableCoins >= card.CurrentCoinCost(currentPlayer) &&
                gameState.GetPile(card).Any());
        }

        public override Card GetTreasureFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            return this.treasurePlayOrder.GetPreferredCard(
                gameState,
                card => currentPlayer.Hand.HasCard(card) && acceptableCard(card));
        }

        public override Card GetCardFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var currentPlayer = gameState.players.CurrentPlayer;

            if (!(currentPlayer.Hand.HasCard(acceptableCard)))
            {
                return null;
            }

            Card result = this.actionOrder.GetPreferredCard(
                gameState,
                card => currentPlayer.Hand.HasCard(card) && acceptableCard(card));

            if (result == null && this.chooseDefaultActionOnNone)
            {
                var candidateCards = new SetOfCards(gameState.CardGameSubset);
                
                foreach(Card card in currentPlayer.Hand)
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
                    card => currentPlayer.Hand.HasCard(card) && acceptableCard(card));
            }

            return result;
        }

        public override Card GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            Card result = this.trashOrder.GetPreferredCard(
                gameState,
                card => currentPlayer.Hand.HasCard(card) && acceptableCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null && !isOptional)
            {
                result = currentPlayer.Hand.OrderBy(c => c, new CompareCardByFirstToTrash()).FirstOrDefault();                
            }

            return result;
        }

        public override Card GetCardFromHandOrDiscardToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional, out DeckPlacement deckPlacement)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            Card result = this.trashOrder.GetPreferredCard(
                gameState,
                card => (currentPlayer.Hand.HasCard(card) || currentPlayer.Discard.HasCard(card)) && acceptableCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null && !isOptional)
            {
                result = currentPlayer.Hand.OrderBy(c => c, new CompareCardByFirstToTrash()).FirstOrDefault();
            }

            deckPlacement = DeckPlacement.Discard;
            if (result != null)
            {
                if (currentPlayer.Discard.HasCard(result))
                    deckPlacement = DeckPlacement.Discard;
                else if (currentPlayer.Hand.HasCard(result))
                    deckPlacement = DeckPlacement.Hand;
                else
                    throw new Exception("Card should have been in hand or discard");
            }

            return result;
        }

        public override Card GetCardFromRevealedCardsToTrash(GameState gameState, PlayerState player, CardPredicate acceptableCard)
        {            
            var currentPlayer = player;
            Card result = this.trashOrder.GetPreferredCard(
                gameState,
                card => currentPlayer.CardsBeingRevealed.HasCard(card) && acceptableCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null)
            {
                result = currentPlayer.CardsBeingRevealed.Where(c => acceptableCard(c)).OrderBy(c => c, new CompareCardByFirstToTrash()).FirstOrDefault();                
            }

            return result;
        }

        public override Card GetCardFromRevealedCardsToDiscard(GameState gameState, PlayerState player)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            Card result = this.discardOrder.GetPreferredCard(
                gameState,
                card => currentPlayer.CardsBeingRevealed.HasCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null)
            {
                result = currentPlayer.CardsBeingRevealed.OrderBy(c => c, new CompareCardByFirstToDiscard()).FirstOrDefault();                
            }

            return result;
        }

        public override Card GetCardFromRevealedCardsToTopDeck(GameState gameState, PlayerState player)
        {
            BagOfCards revealedCards = player.CardsBeingRevealed;
            // good for cartographer, not sure about anyone else.
            foreach (Card card in revealedCards)
            {
                bool shouldDiscard = card.isVictory || card.Is<CardTypes.Copper>();
                if (!shouldDiscard)
                {
                    return card;
                }
            }

            return null;
        }

        override public Card GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            Card result = this.discardOrder.GetPreferredCard(gameState, card => gameState.players.CurrentPlayer.Hand.HasCard(card) && acceptableCard(card));
            if (result != null)
            {
                return result;
            }

            if (result == null && !isOptional)
            {
                result = gameState.players.CurrentPlayer.Hand.Where(c => acceptableCard(c)).OrderBy(c => c, new CompareCardByFirstToDiscard()).FirstOrDefault();                
            }

            return result;
        }

        override public Card GetCardFromDiscardToTopDeck(GameState gameState, PlayerState currentPlayer, bool isOptional)
        {
            Card result = this.discardOrder.GetPreferredCardReverse(gameState, card => currentPlayer.Discard.HasCard(card));
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
            if (DoesCardPickerMatch(this.trashOrder, gameState, card))
                return DeckPlacement.Trash;

            return DeckPlacement.TopOfDeck;
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

        public override bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            if (card.Is<CardTypes.Watchtower>())
                return true;

            return base.ShouldRevealCardFromHand(gameState, card);
        }

        public override bool ShouldRevealCardFromHandForCard(GameState gameState, Card card, Card cardFor)
        {
            if (card.Is<CardTypes.Trader>())
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

        public override Card GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, PlayerState player, bool isOptional)
        {
            Card result = this.discardOrder.GetPreferredCard(
                gameState,
                card => player.Hand.HasCard(card) && acceptableCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null)
            {
                result = player.Hand.Where(c => acceptableCard(c))
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

        public override Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState, PlayerState player)
        {
            Card result = this.discardOrder.GetPreferredCard(
                gameState,
                card => player.CardsBeingRevealed.HasCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null)
            {
                result = player.CardsBeingRevealed.OrderBy(c => c, new CompareCardByFirstToDiscard()).FirstOrDefault();                
            }

            return result;
        }

        public override Card GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            Card result = this.gainOrder.GetPreferredCard(
                gameState,
                card => acceptableCard(card) && gameState.GetPile(card).Any());

            if (result == null &&
                acceptableCard(Card.Type<CardTypes.Copper>()) &&
                Strategies.CardBeingPlayedIs<CardTypes.IllGottenGains>(gameState))
            {
                if (ShouldGainCopper(gameState, this.purchaseOrder))
                {
                    result = Card.Type<CardTypes.Copper>();
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
            PlayerState currentPlayer = gameState.players.CurrentPlayer;

            int minValue = gameState.players.CurrentPlayer.ExpectedCoinValueAtEndOfTurn;
            int maxValue = minValue + Strategies.CountInHand<CardTypes.IllGottenGains>(gameState);

            if (maxValue == minValue)
                return false;

            CardPredicate shouldGainCard = delegate(Card card)
            {
                int currentCardCost = card.CurrentCoinCost(currentPlayer);

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
            return pickOrder.GetPreferredCard(gameState, c => c.Is(card)) != null;
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

        public override bool ShouldPlayerDiscardCardFromHand(GameState gameState, PlayerState playerState, Card card)
        {
            if (card.Is<CardTypes.MarketSquare>())
                return true;

            return base.ShouldPlayerDiscardCardFromHand(gameState, playerState, card);
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
            PlayerState currentPlayer = gameState.players.CurrentPlayer;
            int numberOfBuys = currentPlayer.AvailableBuys;
            int availableCoins = currentPlayer.AvailableCoins;

            return GetCoinAmountToSpend(gameState, numberOfBuys, minCoins:0, maxCoins:availableCoins);
        }

        public int GetCoinAmountToSpend(GameState gameState, int numberOfGains, int minCoins, int maxCoins)
        {
            PlayerState currentPlayer = gameState.players.CurrentPlayer;
            int availableCoins = maxCoins;
            int coinTokensRemaining = currentPlayer.AvailableCoinTokens;

            int result = 0;

            CardPredicate shouldGainCard = delegate(Card card)
            {
                int currentCardCost = card.CurrentCoinCost(currentPlayer);
                
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
            return cardType.CurrentCoinCost(gameState.players.CurrentPlayer);
        }

        public static Card[] GetKingdomCards(PlayerAction playerAction1, PlayerAction playerAction2)
        {
            var cards = new HashSet<Card>();

            AddCards(cards, playerAction1.actionOrder);
            AddCards(cards, playerAction1.purchaseOrder);
            AddCards(cards, playerAction1.gainOrder);
            AddCards(cards, playerAction2.actionOrder);
            AddCards(cards, playerAction2.purchaseOrder);
            AddCards(cards, playerAction2.gainOrder);

            var cardsToRemove = new Card[] { 
                // treaures
                Card.Type<CardTypes.Platinum>(),
                Card.Type<CardTypes.Gold>(),
                Card.Type<CardTypes.Silver>(),
                Card.Type<CardTypes.Copper>(),
                Card.Type<CardTypes.Colony>(),
                Card.Type<CardTypes.Potion>(),
                // victory
                Card.Type<CardTypes.Province>(),
                Card.Type<CardTypes.Duchy>(),
                Card.Type<CardTypes.Estate>(),
                Card.Type<CardTypes.Curse>(),                
                // ruins
                Card.Type<CardTypes.AbandonedMine>(),
                Card.Type<CardTypes.RuinedLibrary>(),
                Card.Type<CardTypes.RuinedVillage>(),
                Card.Type<CardTypes.RuinedMarket>(),
                Card.Type<CardTypes.Survivors>(),
                // shelters
                Card.Type<CardTypes.OvergrownEstate>(),
                Card.Type<CardTypes.Hovel>(),
                Card.Type<CardTypes.Necropolis>(),
                // non-supply piles
                Card.Type<CardTypes.Spoils>(),                
                Card.Type<CardTypes.Madman>(),
                Card.Type<CardTypes.Mercenary>()
            };

            foreach (Card card in cardsToRemove)
            {
                cards.Remove(card);
            }

            return cards.ToArray();
        }

        private static void AddCards(HashSet<Card> cardSet, ICardPicker matchingCards)
        {
            foreach (Card card in matchingCards.GetNeededCards())
            {                
                cardSet.Add(card);
            }
        }
    }    
}

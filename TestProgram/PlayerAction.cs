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
        internal readonly ICardPicker purchaseOrder;
        internal readonly ICardPicker actionOrder;
        internal readonly ICardPicker trashOrder;
        internal readonly ICardPicker treasurePlayOrder;
        internal readonly ICardPicker discardOrder;
        internal readonly ICardPicker gainOrder;

        public PlayerAction(
            string name,
            int playerIndex,
            ICardPicker purchaseOrder,
            ICardPicker actionOrder = null,
            ICardPicker treasurePlayOrder = null,
            ICardPicker discardOrder = null,
            ICardPicker trashOrder = null,
            ICardPicker gainOrder = null)
        {
            this.playerIndex = playerIndex;
            this.purchaseOrder = purchaseOrder;
            this.actionOrder = actionOrder == null ? Strategies.Default.ActionPlayOrder(this.purchaseOrder) : actionOrder;
            this.discardOrder = discardOrder == null ? Strategies.Default.DefaultDiscardOrder() : discardOrder;
            this.trashOrder = trashOrder == null ? Strategies.Default.DefaultTrashOrder() : trashOrder;
            this.treasurePlayOrder = treasurePlayOrder == null ? Strategies.Default.TreasurePlayOrder() : treasurePlayOrder;            
            this.gainOrder = gainOrder != null ? gainOrder : purchaseOrder;
            this.name = name;
        }

        public static int PlayIndexfor(IPlayerAction playerAction)
        {
            return ((PlayerAction)playerAction).playerIndex;
        }

        public override Type GetCardFromSupplyToBuy(GameState gameState, CardPredicate cardPredicate)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            return this.purchaseOrder.GetPreferredCard(
                gameState,
                card => currentPlayer.AvailableCoins >= card.CurrentCoinCost(currentPlayer) &&
                gameState.GetPile(card).Any());
        }

        public override Type GetTreasureFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            return this.treasurePlayOrder.GetPreferredCard(
                gameState,
                card => currentPlayer.Hand.HasCard(card.GetType()) && acceptableCard(card));
        }

        public override Type GetCardFromHandToPlay(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            return this.actionOrder.GetPreferredCard(
                gameState,
                card => currentPlayer.Hand.HasCard(card.GetType()) && acceptableCard(card));
        }

        public override Type GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            Type result = this.trashOrder.GetPreferredCard(
                gameState,
                card => currentPlayer.Hand.HasCard(card.GetType()) && acceptableCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null && !isOptional)
            {
                Card card = currentPlayer.Hand.OrderBy(c => c, new CompareCardByFirstToTrash()).FirstOrDefault();
                return card != null ? card.GetType() : null;
            }

            return result;
        }

        public override Type GetCardFromRevealedCardsToTrash(GameState gameState, PlayerState player, CardPredicate acceptableCard)
        {            
            var currentPlayer = player;
            Type result = this.trashOrder.GetPreferredCard(
                gameState,
                card => currentPlayer.CardsBeingRevealed.HasCard(card.GetType()) && acceptableCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null)
            {
                Card card = currentPlayer.CardsBeingRevealed.Where(c => acceptableCard(c)).OrderBy(c => c, new CompareCardByFirstToTrash()).FirstOrDefault();                
                return card != null ? card.GetType() : null;
            }

            return result;
        }

        public override Type GetCardFromRevealedCardsToDiscard(GameState gameState, PlayerState player)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            Type result = this.discardOrder.GetPreferredCard(
                gameState,
                card => currentPlayer.CardsBeingRevealed.HasCard(card.GetType()));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null)
            {
                Card card = currentPlayer.CardsBeingRevealed.OrderBy(c => c, new CompareCardByFirstToDiscard()).FirstOrDefault();
                return card != null ? card.GetType() : null;
            }

            return result;
        }

        public override Type GetCardFromRevealedCardsToTopDeck(GameState gameState, PlayerState player)
        {
            BagOfCards revealedCards = player.CardsBeingRevealed;
            // good for cartographer, not sure about anyone else.
            foreach (Card card in revealedCards)
            {
                bool shouldDiscard = card.isVictory || card.Is<CardTypes.Copper>();
                if (!shouldDiscard)
                {
                    return card.GetType();
                }
            }

            return null;
        }

        public override bool ShouldPutCardInHand(GameState gameState, Card card)
        {
            return this.discardOrder.GetPreferredCard(gameState, testCard => testCard.Is(card.GetType())) == null;
        }

        public override bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card)
        {
            return this.discardOrder.GetPreferredCard(gameState, testCard => testCard.Is(card.GetType())) != null;
        }

        public override DeckPlacement ChooseBetweenTrashAndTopDeck(GameState gameState, Card card)
        {
            if (this.trashOrder.GetPreferredCard(gameState, c => c.Equals(card)) != null)
                return DeckPlacement.Trash;

            return DeckPlacement.TopOfDeck;
        }

        public override Type GetCardFromOtherPlayersHandToDiscard(GameState gameState, PlayerState otherPlayer)
        {
            // discard the highest costing action or treasure.
            Card result = otherPlayer.Hand.Where(c => c.isAction || c.isTreasure).OrderByDescending(c => c.DefaultCoinCost).FirstOrDefault();

            // or just discard the highest costing card
            if (result == null)
                result = otherPlayer.Hand.OrderByDescending(c => c.DefaultCoinCost).FirstOrDefault();

            return result.GetType();
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

        public override Type GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, PlayerState player, bool isOptional)
        {
            Type result = this.discardOrder.GetPreferredCard(
                gameState,
                card => player.Hand.HasCard(card.GetType()) && acceptableCard(card));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null)
            {
                Card card = player.Hand.Where(c => acceptableCard(c))
                                       .OrderBy(c => c, new CompareCardByFirstToDiscard())
                                       .FirstOrDefault();

                return card != null ? card.GetType() : null;
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

        public override Type GetCardFromRevealedCardsToPutOnDeck(GameState gameState, PlayerState player)
        {
            Type result = this.discardOrder.GetPreferredCard(
                gameState,
                card => player.CardsBeingRevealed.HasCard(card.GetType()));

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null)
            {
                Card card = player.CardsBeingRevealed.OrderBy(c => c, new CompareCardByFirstToDiscard()).FirstOrDefault();
                return card != null ? card.GetType() : null;
            }

            return result;
        }

        public override Type GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            var currentPlayer = gameState.players.CurrentPlayer;
            Type result = this.gainOrder.GetPreferredCard(
                gameState,
                card => acceptableCard(card) && gameState.GetPile(card).Any());

            if (result == null &&
                acceptableCard(Strategies.Example<CardTypes.Copper>.Card) &&
                Strategies.CardBeingPlayedIs<CardTypes.IllGottenGains>(gameState))
            {
                if (ShouldGainCopper(gameState, this.purchaseOrder))
                    result = typeof(CardTypes.Copper);
            }

            // warning, strategy didnt' include what to, try to do a reasonable default.
            if (result == null && !isOptional)
            {
                Card card = gameState.supplyPiles.Where(supplyPile => !supplyPile.IsEmpty)
                                     .Select(pile => pile.ProtoTypeCard)
                                     .Where(c => acceptableCard(c))
                                     .OrderBy(c => c, new CompareCardByFirstToGain())
                                     .FirstOrDefault();

                return card != null ? card.GetType() : null;
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

            Type cardType = gainOrder.GetPreferredCard(gameState, shouldGainCard);
            if (cardType == null)
                return false;

            int coppersToGain = PlayerAction.CostOfCard(cardType, gameState) - minValue;

            return (coppersToGain > 0);
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

                if (x.isRuin ^ y.isRuin)
                {
                    return x.isRuin ? 1 : -1;
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

        public override int GetCoinAmountToUseInButcher(GameState gameState)
        {
            Type cardToTrash = Strategies.WhichCardFromInHand(this.trashOrder, gameState);
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
                Type cardType = this.gainOrder.GetPreferredCard(gameState, shouldGainCard);                    
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
    
        public static int CostOfCard(Type cardType, GameState gameState)
        {
            return gameState.GetPile(cardType).ProtoTypeCard.CurrentCoinCost(gameState.players.CurrentPlayer);
        }

        public static Card[] GetKingdomCards(PlayerAction playerAction1, PlayerAction playerAction2)
        {
            var cards = new HashSet<Card>(new CompareCardByType());

            AddCards(cards, playerAction1.actionOrder);
            AddCards(cards, playerAction1.purchaseOrder);
            AddCards(cards, playerAction1.gainOrder);
            AddCards(cards, playerAction2.actionOrder);
            AddCards(cards, playerAction2.purchaseOrder);
            AddCards(cards, playerAction2.gainOrder);

            var cardsToRemove = new Card[] { 
                new CardTypes.Platinum(),
                new CardTypes.Gold(),
                new CardTypes.Silver(),
                new CardTypes.Copper(),
                new CardTypes.Colony(),
                new CardTypes.Province(),
                new CardTypes.Duchy(),
                new CardTypes.Estate(),
                new CardTypes.Curse(),
                new CardTypes.Potion(),
                new CardTypes.RuinedLibrary(),
                new CardTypes.RuinedVillage(),
                new CardTypes.RuinedMarket(),
                new CardTypes.Survivors(),
                new CardTypes.Curse(),
                new CardTypes.Spoils(),
                new CardTypes.OvergrownEstate(),
                new CardTypes.Hovel(),
                new CardTypes.Necropolis(),
                new CardTypes.Madman(),
                new CardTypes.Mercenary()
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

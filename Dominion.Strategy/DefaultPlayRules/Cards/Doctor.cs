using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Doctor
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Doctor(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override int GetCoinAmountToOverpayForCard(GameState gameState, Card card)
        {
            return gameState.Self.AvailableCoins;
        }

        public override Card NameACard(GameState gameState)
        {
            return GetCardTypeToTrash(gameState);
        }

        public override DeckPlacement ChooseBetweenTrashTopDeckDiscard(GameState gameState, Card card)
        {
            if (gameState.Self.CurrentCardBeingBought != Dominion.Cards.Doctor)
                throw new System.Exception();

            var cardToDecide = gameState.Self.CardsBeingLookedAt.SomeCard();

            if (this.playerAction.trashOrder.DoesCardPickerMatch(gameState, cardToDecide))
                return DeckPlacement.Trash;

            if (this.playerAction.discardOrder.DoesCardPickerMatch(gameState, cardToDecide))
                return DeckPlacement.Discard;

            return DeckPlacement.Deck;
        }

        public override Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState)
        {
            return gameState.Self.CardsBeingRevealed.SomeCard();
        }

        static Card GetCardTypeToTrash(GameState gameState)
        {
            PlayerState self = gameState.Self;

            if (self.CardsInDeck.Count <= 3 &&
                Strategy.CountInDeck(Dominion.Cards.Estate, gameState) > 0)
            {
                return Dominion.Cards.Estate;
            }

            int countCopper = Strategy.CountMightDraw(Dominion.Cards.Copper, gameState, 3);
            int countEstate = Strategy.CountMightDraw(Dominion.Cards.Estate, gameState, 3);

            if (DefaultStrategies.ShouldBuyProvinces(gameState))
                countEstate = 0;

            if (countCopper + countEstate == 0)
                return Dominion.Cards.Curse;

            return countCopper > countEstate ? (Card)Dominion.Cards.Copper : Dominion.Cards.Estate;
        }
    }

}

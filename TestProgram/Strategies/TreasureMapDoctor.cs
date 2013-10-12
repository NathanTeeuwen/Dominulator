using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public static partial class Strategies
    {
        public static class TreasureMapDoctor
        {            
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("TreasureMapDoctor",
                        playerNumber,
                        purchaseOrder: PurchaseOrder(),                        
                        actionOrder: ActionOrder(),
                        trashOrder: TrashAndDiscardOrder(),
                        discardOrder: TrashAndDiscardOrder())
                {
                }
               
                override public int GetCoinAmountToOverpayForCard(GameState gameState, Card card)
                {
                    return gameState.Self.AvailableCoins;
                }

                public override Card NameACard(GameState gameState)
                {
                    return GetCardTypeToTrash(gameState);
                }

                public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {
                    if (gameState.Self.CardsBeingRevealed.HasCard(CardTypes.Silver.card))
                        return PlayerActionChoice.Discard;
                    return PlayerActionChoice.Trash;
                }

                public override Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState, PlayerState player)
                {
                    return player.CardsBeingRevealed.FirstOrDefault();
                }

                static Card GetCardTypeToTrash(GameState gameState)
                {
                    PlayerState self = gameState.Self;

                    if (self.CardsInDeck.Count() <= 3 &&
                        CountInDeck(CardTypes.Estate.card, gameState) > 0)
                    {
                        return CardTypes.Estate.card;
                    }

                    int countCopper = CountMightDraw(CardTypes.Copper.card, gameState, 3);
                    int countEstate = CountMightDraw(CardTypes.Estate.card, gameState, 3);

                    if (Default.ShouldBuyProvinces(gameState))
                        countEstate = 0;

                    if (countCopper + countEstate == 0)
                        return CardTypes.Estate.card;

                    return countCopper > countEstate ? (Card) CardTypes.Copper.card : CardTypes.Estate.card;
                }
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Doctor.card, gameState => CountAllOwned(CardTypes.Doctor.card, gameState) < 1 && gameState.Self.AvailableCoins >= 5),
                           CardAcceptance.For(CardTypes.Province.card),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 5),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 2),
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(CardTypes.TreasureMap.card, gameState => CountAllOwned(CardTypes.Gold.card, gameState) == 0),
                           //CardAcceptance.For(CardTypes.Doctor.card, gameState => CountAllOwned(CardTypes.Doctor.card, gameState) == 0),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 4),
                           CardAcceptance.For(CardTypes.Silver.card));                           
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.TreasureMap.card, gameState => CountInHand(CardTypes.TreasureMap.card, gameState) == 2 || CountAllOwned(CardTypes.Gold.card, gameState) > 0),
                           CardAcceptance.For(CardTypes.Doctor.card));
            }

            private static CardPickByPriority TrashAndDiscardOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Estate.card),
                           CardAcceptance.For(CardTypes.Copper.card),
                           CardAcceptance.For(CardTypes.Silver.card),
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(CardTypes.Province.card),
                           CardAcceptance.For(CardTypes.Duchy.card),
                           CardAcceptance.For(CardTypes.Estate.card),
                           CardAcceptance.For(CardTypes.TreasureMap.card));
            }
        }
    }
}

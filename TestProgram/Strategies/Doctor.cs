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
        public static class Doctor
        {              
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("Doctor",
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
                    return PlayerActionChoice.Trash;
                }

                public override Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState, PlayerState player)
                {
                    return player.CardsBeingRevealed.FirstOrDefault();
                }
            }

            static bool ShouldPlayerDoctor(GameState gameState)
            {
                return GetCardTypeToTrash(gameState) != null;
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
                    return null;
                
                return countCopper > countEstate ? (Card) CardTypes.Copper.card : CardTypes.Estate.card;
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Province.card, Default.ShouldBuyProvinces),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 4),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 2),
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 2),
                           CardAcceptance.For(CardTypes.Doctor.card, gameState => CountAllOwned(CardTypes.Doctor.card, gameState) < 1),                           
                           CardAcceptance.For(CardTypes.Silver.card));
            }            

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Doctor.card, ShouldPlayerDoctor));
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
                           CardAcceptance.For(CardTypes.Estate.card));
            }        
        }
    }
}

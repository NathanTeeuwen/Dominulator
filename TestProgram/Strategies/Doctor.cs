using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program;

namespace Strategies
{
    public class Doctor
        : Strategy
    {              
        public static PlayerAction Player()
        {
            return new MyPlayerAction();
        }

        class MyPlayerAction
            : PlayerAction
        {
            public MyPlayerAction()
                : base("Doctor",
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

            public override Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState)
            {
                return gameState.Self.CardsBeingRevealed.FirstOrDefault();
            }
        }

        static bool ShouldPlayerDoctor(GameState gameState)
        {
            return GetCardTypeToTrash(gameState) != null;
        }

        static Card GetCardTypeToTrash(GameState gameState)
        {
            PlayerState self = gameState.Self;

            if (self.CardsInDeck.Count <= 3 &&
                CountInDeck(Cards.Estate, gameState) > 0)
            {
                return Cards.Estate;
            }

            int countCopper = CountMightDraw(Cards.Copper, gameState, 3);
            int countEstate = CountMightDraw(Cards.Estate, gameState, 3);

            if (Default.ShouldBuyProvinces(gameState))
                countEstate = 0;

            if (countCopper + countEstate == 0)
                return null;
                
            return countCopper > countEstate ? (Card) Cards.Copper : Cards.Estate;
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, Default.ShouldBuyProvinces),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.Doctor, gameState => CountAllOwned(Cards.Doctor, gameState) < 1),                           
                        CardAcceptance.For(Cards.Silver));
        }            

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Doctor, ShouldPlayerDoctor));
        }                        

        private static CardPickByPriority TrashAndDiscardOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.Copper),                           
                        CardAcceptance.For(Cards.Silver),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy),
                        CardAcceptance.For(Cards.Estate));
        }        
    }    
}

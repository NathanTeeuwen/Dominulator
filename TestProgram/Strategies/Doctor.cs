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

                override public int GetAmountToOverpayForCard(GameState gameState, Card card)
                {
                    return gameState.players.CurrentPlayer.AvailableCoins;
                }

                public override Type NameACard(GameState gameState)
                {                    
                    return GetCardTypeToTrash(gameState);
                }

                public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {
                    return PlayerActionChoice.Trash;
                }

                public override Type GetCardFromRevealedCardsToPutOnDeck(GameState gameState, PlayerState player)
                {
                    return player.CardsBeingRevealed.FirstOrDefault().GetType();
                }
            }

            static bool ShouldPlayerDoctor(GameState gameState)
            {
                return GetCardTypeToTrash(gameState) != null;
            }

            static Type GetCardTypeToTrash(GameState gameState)
            {
                PlayerState currentPlayer = gameState.players.CurrentPlayer;

                if (currentPlayer.CardsInDeck.Count() <= 3 &&
                    CountInDeck<CardTypes.Estate>(gameState) > 0)
                {
                    return typeof(CardTypes.Estate);
                }

                int countCopper = CountMightDraw<CardTypes.Copper>(gameState, 3);
                int countEstate = CountMightDraw<CardTypes.Estate>(gameState, 3);

                if (Default.ShouldBuyProvinces(gameState))
                    countEstate = 0;

                if (countCopper + countEstate == 0)
                    return null;
                
                return countCopper > countEstate ? typeof(CardTypes.Copper) : typeof(CardTypes.Estate);
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(Default.ShouldBuyProvinces),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 2),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 2),
                           CardAcceptance.For<CardTypes.Doctor>(gameState => CountAllOwned<CardTypes.Doctor>(gameState) < 1),                           
                           CardAcceptance.For<CardTypes.Silver>());
            }            

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Doctor>(ShouldPlayerDoctor));
            }                        

            private static CardPickByPriority TrashAndDiscardOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Estate>(),
                           CardAcceptance.For<CardTypes.Copper>(),                           
                           CardAcceptance.For<CardTypes.Silver>(),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Province>(),
                           CardAcceptance.For<CardTypes.Duchy>(),
                           CardAcceptance.For<CardTypes.Estate>());
            }        
        }
    }
}

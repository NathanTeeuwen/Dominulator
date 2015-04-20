using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Strategies
{
    // for kingdom:  altar, bandit camp, city, masterpiece, menagerie, mountebank, stables, stash, steward, university
    public class AltarStewardCityMountebank
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
                : base("AltarStewardCityMountebank",
                    purchaseOrder: PurchaseOrder(),
                    actionOrder: ActionOrder(),
                    trashOrder: TrashOrder())
            {
            }

            /*
            public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
            {
                if (gameState.CurrentContext.CurrentCard != Cards.Steward)
                    return base.ChooseBetween(gameState, acceptableChoice);

                if (CountInHand(Cards.Estate, gameState) + CountInHand(Cards.Curse, gameState) + CountInHand(Cards.Copper, gameState) >= 2)
                    return PlayerActionChoice.Trash;
                if (gameState.Self.AvailableActions > 0 && gameState.Self.CardsInDeck.Count + gameState.Self.Discard.Count >= 2)
                    return PlayerActionChoice.PlusCard;
                else
                    return PlayerActionChoice.PlusCoin;
            }*/

            

            public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
            {
                if (gameState.CurrentContext.CurrentCard != Cards.Steward)
                    return base.ChooseBetween(gameState, acceptableChoice);

                if (CountInHand(Cards.Estate, gameState) + CountInHand(Cards.Curse, gameState) >= 2)
                    return PlayerActionChoice.Trash;
                if (CountInHand(Cards.Copper, gameState) >= 2 && gameState.Self.ExpectedCoinValueAtEndOfTurn >= 7)
                    return PlayerActionChoice.Trash;
               // if (CountAllOwned(Cards.Copper, gameState) >= 7)
                   // return PlayerActionChoice.Trash;
                if (gameState.Self.AvailableActions > 0 && gameState.Self.CardsInDeck.Count + gameState.Self.Discard.Count >= 2)
                    return PlayerActionChoice.PlusCard;
                else
                    return PlayerActionChoice.PlusCoin;
            }
        }

        private static ICardPicker PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Altar, 2),
                        CardAcceptance.For(Cards.Province),
                        //CardAcceptance.For(Cards.Mountebank, gameState => (CountAllOwned(Cards.Altar, gameState) == 0) && CountAllOwned(Cards.Mountebank, gameState) < 1),
                        //CardAcceptance.For(Cards.BanditCamp, gameState => (CountAllOwned(Cards.Altar, gameState) >= 1) && CountAllOwned(Cards.BanditCamp, gameState) == 0),
                        CardAcceptance.For(Cards.Mountebank, 1),
                        //CardAcceptance.For(Cards.Stables, 1),
                        CardAcceptance.For(Cards.City),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 5),
                        CardAcceptance.For(Cards.Stables, 3),
                        CardAcceptance.For(Cards.Duchy),
                        CardAcceptance.For(Cards.BanditCamp),
                        CardAcceptance.For(Cards.Steward, 1),
                        CardAcceptance.For(Cards.Silver, 1),
                        CardAcceptance.For(Cards.Menagerie));
        }

        private static ICardPicker ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Menagerie, gameState => !gameState.Self.Hand.HasDuplicatesExceptMenagerie()),
                        //CardAcceptance.For(Cards.Menagerie),
                        CardAcceptance.For(Cards.BanditCamp),
                        CardAcceptance.For(Cards.City),
                        CardAcceptance.For(Cards.Stables),
                        CardAcceptance.For(Cards.Menagerie),
                        CardAcceptance.For(Cards.Mountebank),
                        CardAcceptance.For(Cards.Altar),
                        CardAcceptance.For(Cards.Steward));
        }

        private static ICardPicker TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Curse),
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.Copper),
                        CardAcceptance.For(Cards.Spoils));
        }         
    }

    public class UnivesityStewardCityMountebank
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
                : base("UniversityStewardCityMountebank",
                    purchaseOrder: PurchaseOrder(),
                    actionOrder: ActionOrder(),
                    trashOrder: TrashOrder())
            {
            }

            
            public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
            {
                if (gameState.CurrentContext.CurrentCard != Cards.Steward)
                    return base.ChooseBetween(gameState, acceptableChoice);

                if (CountInHand(Cards.Estate, gameState) + CountInHand(Cards.Curse, gameState) + CountInHand(Cards.Copper, gameState) >= 2)
                    return PlayerActionChoice.Trash;
                if (gameState.Self.AvailableActions > 0 && gameState.Self.CardsInDeck.Count + gameState.Self.Discard.Count >= 2)
                    return PlayerActionChoice.PlusCard;
                else
                    return PlayerActionChoice.PlusCoin;
            }
            /*
            public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
            {
                if (gameState.CurrentContext.CurrentCard != Cards.Steward)
                    return base.ChooseBetween(gameState, acceptableChoice);

                if (CountInHand(Cards.Estate, gameState) + CountInHand(Cards.Curse, gameState) >= 2)
                    return PlayerActionChoice.Trash;          
                if (CountInHand(Cards.Copper, gameState) >= 2 && gameState.Self.ExpectedCoinValueAtEndOfTurn >= 7)
                    return PlayerActionChoice.Trash;
                // if (CountAllOwned(Cards.Copper, gameState) >= 7)
                // return PlayerActionChoice.Trash;
                if (gameState.Self.AvailableActions > 0 && gameState.Self.CardsInDeck.Count + gameState.Self.Discard.Count >= 2)
                    return PlayerActionChoice.PlusCard;
                else
                    return PlayerActionChoice.PlusCoin;
            }*/
        }

        private static ICardPicker PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        //CardAcceptance.For(Cards.Altar, 1),
                        CardAcceptance.For(Cards.University, 3),
                        CardAcceptance.For(Cards.Mountebank, 1),                                               
                        CardAcceptance.For(Cards.City),
                        CardAcceptance.For(Cards.Mountebank, 2),
                        //CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 5),
                        CardAcceptance.For(Cards.Duchy),
                        CardAcceptance.For(Cards.BanditCamp),
                        CardAcceptance.For(Cards.Stables),
                        CardAcceptance.For(Cards.Potion, 1),
                        CardAcceptance.For(Cards.Steward, 4),                        
                        //CardAcceptance.For(Cards.Silver, 1),
                        CardAcceptance.For(Cards.Menagerie));
        }

        private static ICardPicker ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Menagerie, gameState => !gameState.Self.Hand.HasDuplicatesExceptMenagerie()),
                        CardAcceptance.For(Cards.University),                        
                        CardAcceptance.For(Cards.City),
                        CardAcceptance.For(Cards.Stables),
                        CardAcceptance.For(Cards.BanditCamp),
                        CardAcceptance.For(Cards.Menagerie),
                        CardAcceptance.For(Cards.Steward, gameState => gameState.Self.AvailableActions >= 1),
                        CardAcceptance.For(Cards.Mountebank),
                        CardAcceptance.For(Cards.Altar),
                        CardAcceptance.For(Cards.Steward));
        }

        private static ICardPicker TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Curse),
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.Potion, gameState => CountAllOwned(Cards.University, gameState) >= 3),
                        CardAcceptance.For(Cards.Copper),
                        CardAcceptance.For(Cards.Spoils));
        }
    }
}
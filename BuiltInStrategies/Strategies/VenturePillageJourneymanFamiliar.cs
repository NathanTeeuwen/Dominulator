using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Strategies
{
    public class VenturePillageJourneyman
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
                : base( "VenturePillageJourneyman",
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder())
           {

           }

            public override Card NameACard(GameState gameState)
            {
                if (gameState.CurrentContext.CurrentCard != Cards.Journeyman)
                    return base.NameACard(gameState);

                return Cards.Curse;
            }

            public override Card GetCardFromOtherPlayersHandToDiscard(GameState gameState, PlayerState otherPlayer)
            {
                if (gameState.CurrentContext.CurrentCard != Cards.Pillage)
                    return base.GetCardFromOtherPlayersHandToDiscard(gameState, otherPlayer);

                if (otherPlayer.ExpectedCoinValueAtEndOfTurn >= 5)
                {
                    var returned = otherPlayer.Hand.Where(card => card != Cards.Familiar && card.isTreasure).OrderByDescending(c => c.DefaultCoinCost).FirstOrDefault();
                    if (returned == null)
                        throw new Exception();
                    return returned;
                }
                /*
                Card discard = otherPlayer.Hand.Where(card => card != Cards.Familiar && card.DefaultCoinCost >= 3).OrderByDescending(c => c.DefaultCoinCost).FirstOrDefault();
                if (discard != null)
                    return discard;*/

                if (otherPlayer.Hand.HasCard(Cards.Familiar))
                    return Cards.Familiar;

                return base.GetCardFromOtherPlayersHandToDiscard(gameState, otherPlayer);
            }
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) + CountAllOwned(Cards.Venture, gameState) > 2),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.Gold, 1),
                        //CardAcceptance.For(Cards.Venture, 1),
                        CardAcceptance.For(Cards.Pillage, ShouldBuyPillage),                                               
                        CardAcceptance.For(Cards.Gold),                        
                        CardAcceptance.For(Cards.Journeyman, 1),
                        //CardAcceptance.For(Cards.Pillage, ShouldBuyPillage),
                        CardAcceptance.For(Cards.Venture),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        CardAcceptance.For(Cards.Silver));
        }

        private static bool ShouldBuyPillage(GameState gameState)
        {
            return CountAllOwned(Cards.Pillage, gameState) +
                   CountAllOwned(Cards.Gold, gameState) +
                   CountAllOwned(Cards.Venture, gameState) +
                   CountAllOwned(Cards.Journeyman, gameState) +
                   CountAllOwned(Cards.Spoils, gameState) < 2;
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For(Cards.Journeyman),
                CardAcceptance.For(Cards.Pillage)                
                );
        }
    }

    public class VenturePillageJourneymanFamiliar
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
                : base("VenturePillageJourneymanFamiliar",
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder())
            {

            }

            public override Card NameACard(GameState gameState)
            {
                if (gameState.CurrentContext.CurrentCard != Cards.Journeyman)
                    return base.NameACard(gameState);

                return Cards.Curse;
            }

            public override Card GetCardFromOtherPlayersHandToDiscard(GameState gameState, PlayerState otherPlayer)
            {
                if (gameState.CurrentContext.CurrentCard != Cards.Pillage)
                    return base.GetCardFromOtherPlayersHandToDiscard(gameState, otherPlayer);

                if (otherPlayer.Hand.HasCard(Cards.Familiar))
                    return Cards.Familiar;

                return base.GetCardFromOtherPlayersHandToDiscard(gameState, otherPlayer);
            }
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) + CountAllOwned(Cards.Venture, gameState) > 2),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.Gold, 1),
                        CardAcceptance.For(Cards.Familiar, 3),
                        //CardAcceptance.For(Cards.Venture, 1),
                        //CardAcceptance.For(Cards.Pillage, ShouldBuyPillage),
                        CardAcceptance.For(Cards.Gold),
                        //CardAcceptance.For(Cards.Journeyman, 2),
                        CardAcceptance.For(Cards.Venture),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        CardAcceptance.For(Cards.Potion, 1),
                        CardAcceptance.For(Cards.Silver));
        }

        private static bool ShouldBuyPillage(GameState gameState)
        {
            return CountAllOwned(Cards.Pillage, gameState) +
                   CountAllOwned(Cards.Gold, gameState) +
                   CountAllOwned(Cards.Venture, gameState) +
                   CountAllOwned(Cards.Journeyman, gameState) +
                   CountAllOwned(Cards.Spoils, gameState) < 2;
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For(Cards.Familiar),
                CardAcceptance.For(Cards.Pillage),
                CardAcceptance.For(Cards.Journeyman)
                );
        }
    }

    public class VenturePillageJourneymanFamiliarDelayed
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
                : base("VenturePillageJourneymanFamiliarDelayed",
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder())
            {

            }

            public override Card NameACard(GameState gameState)
            {
                if (gameState.CurrentContext.CurrentCard != Cards.Journeyman)
                    return base.NameACard(gameState);

                return Cards.Curse;
            }

            public override Card GetCardFromOtherPlayersHandToDiscard(GameState gameState, PlayerState otherPlayer)
            {
                if (gameState.CurrentContext.CurrentCard != Cards.Pillage)
                    return base.GetCardFromOtherPlayersHandToDiscard(gameState, otherPlayer);

                if (otherPlayer.Hand.HasCard(Cards.Familiar))
                    return Cards.Familiar;

                return base.GetCardFromOtherPlayersHandToDiscard(gameState, otherPlayer);
            }
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) + CountAllOwned(Cards.Venture, gameState) > 2),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        //CardAcceptance.For(Cards.Gold, 1),
                        CardAcceptance.For(Cards.Familiar, 2),
                //CardAcceptance.For(Cards.Venture, 1),
                        //CardAcceptance.For(Cards.Pillage, ShouldBuyPillage),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Journeyman, 1),
                        CardAcceptance.For(Cards.Venture),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        CardAcceptance.For(Cards.Silver, 2),
                        CardAcceptance.For(Cards.Potion, 1),
                        CardAcceptance.For(Cards.Silver));
        }

        private static bool ShouldBuyPillage(GameState gameState)
        {
            return CountAllOwned(Cards.Pillage, gameState) +
                   CountAllOwned(Cards.Gold, gameState) +
                   CountAllOwned(Cards.Venture, gameState) +
                   CountAllOwned(Cards.Journeyman, gameState) +
                   CountAllOwned(Cards.Spoils, gameState) < 1;
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For(Cards.Familiar),
                CardAcceptance.For(Cards.Pillage),
                CardAcceptance.For(Cards.Journeyman)
                );
        }
    }

    public class VenturePillageJourneymanGraverobber
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
                : base("VenturePillageJourneymanGraverobber",
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder())
            {

            }

            public override Card NameACard(GameState gameState)
            {
                if (gameState.CurrentContext.CurrentCard != Cards.Journeyman)
                    return base.NameACard(gameState);

                return Cards.Curse;
            }

            public override Card GetCardFromOtherPlayersHandToDiscard(GameState gameState, PlayerState otherPlayer)
            {
                if (gameState.CurrentContext.CurrentCard != Cards.Pillage)
                    return base.GetCardFromOtherPlayersHandToDiscard(gameState, otherPlayer);

                if (otherPlayer.Hand.HasCard(Cards.Familiar))
                    return Cards.Familiar;

                return base.GetCardFromOtherPlayersHandToDiscard(gameState, otherPlayer);
            }

            public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
            {
                if (gameState.CurrentContext.CurrentCard != Cards.Graverobber)
                    return base.ChooseBetween(gameState, acceptableChoice);

                if (gameState.Self.Hand.HasCard(c => c.isAction))
                {
                    return PlayerActionChoice.Trash;
                }
                else
                    return PlayerActionChoice.GainCard;                
            }
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) + CountAllOwned(Cards.Venture, gameState) + CountAllOwned(Cards.Graverobber, gameState) + CountAllOwned(Cards.Province, gameState) > 2),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                //CardAcceptance.For(Cards.Gold, 1),
                        //CardAcceptance.For(Cards.Familiar, 2),
                //CardAcceptance.For(Cards.Venture, 1),
                //CardAcceptance.For(Cards.Pillage, ShouldBuyPillage),
                        CardAcceptance.For(Cards.Gold, 1),
                        CardAcceptance.For(Cards.Journeyman, gameState => CountAllOwned(Cards.Graverobber, gameState) + CountAllOwned(Cards.Journeyman, gameState) == 0),
                        CardAcceptance.For(Cards.Graverobber),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        //CardAcceptance.For(Cards.Silver, 2),
                        //CardAcceptance.For(Cards.Potion),
                        CardAcceptance.For(Cards.Silver));
        }

        private static bool ShouldBuyPillage(GameState gameState)
        {
            return CountAllOwned(Cards.Pillage, gameState) +
                   CountAllOwned(Cards.Gold, gameState) +
                   CountAllOwned(Cards.Venture, gameState) +
                   CountAllOwned(Cards.Journeyman, gameState) +
                   CountAllOwned(Cards.Spoils, gameState) < 1;
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For(Cards.Familiar),
                CardAcceptance.For(Cards.Graverobber),
                CardAcceptance.For(Cards.Pillage),
                CardAcceptance.For(Cards.Journeyman)
                );
        }

        private static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For(Cards.Graverobber),                
                CardAcceptance.For(Cards.Journeyman)
                );
        }
    }
}
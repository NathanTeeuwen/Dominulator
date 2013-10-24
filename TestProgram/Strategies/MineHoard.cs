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
        public static class MineHoard
        {

            public static PlayerAction Player()
            {
                return new PlayerAction(
                            "MineHoard",
                            purchaseOrder: PurchaseOrder(),
                            trashOrder: TrashOrder());
            }

            public static ICardPicker PurchaseOrder()
            {
                return new CardPickByPriority(
                           //CardAcceptance.For(Cards.Colony),
                           CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) + CountAllOwned(Cards.Hoard, gameState) > 1 || gameState.Self.CardsInPlay.Contains(Cards.Hoard)),
                           CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4 || gameState.Self.CardsInPlay.Contains(Cards.Hoard)),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2 || gameState.Self.CardsInPlay.Contains(Cards.Hoard)),
                           CardAcceptance.For(Cards.Hoard),
                           CardAcceptance.For(Cards.Mine, 3),
                           CardAcceptance.For(Cards.Gold),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),
                           CardAcceptance.For(Cards.Silver));
            }

            public static ICardPicker TrashOrder()
            {
                return new CardPickByPriority(                           
                           CardAcceptance.For(Cards.Curse),                           
                           CardAcceptance.For(Cards.Silver),
                           CardAcceptance.For(Cards.Copper));                           
            }
        }
    }

    public static partial class Strategies
    {
        public static class ColonyFamiliarMine
        {

            public static PlayerAction Player()
            {
                return new PlayerAction(
                            "ColonyFamiliarMine",
                            purchaseOrder: PurchaseOrder());
            }

            public static ICardPicker PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Colony),                           
                           CardAcceptance.For(Cards.Platinum),
                           CardAcceptance.For(Cards.Gold),
                           CardAcceptance.For(Cards.Familiar, 3),                           
                           CardAcceptance.For(Cards.Potion, 1),
                           CardAcceptance.For(Cards.Mine, 2),
                           CardAcceptance.For(Cards.Silver));
            }
        }
    }

    public static partial class Strategies
    {
        public static class FamiliarPotionSilverOpenning
        {

            public static PlayerAction Player()
            {
                return new PlayerAction(
                            "FamiliarPotionSilverOpenning",
                            purchaseOrder: PurchaseOrder(),
                            trashOrder: TrashOrder());
            }

            public static ICardPicker PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 2),
                           CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                           CardAcceptance.For(Cards.Gold),
                           CardAcceptance.For(Cards.Familiar, 3),
                           CardAcceptance.For(Cards.Potion, 1),                           
                           CardAcceptance.For(Cards.Silver));
            }

            public static ICardPicker TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Curse),
                           CardAcceptance.For(Cards.Silver),
                           CardAcceptance.For(Cards.Copper));
            }
        }

        public static class FamiliarSilverSilverOpenning
        {

            public static PlayerAction Player()
            {
                return new PlayerAction(
                            "FamiliarSilverSilverOpenning",
                            purchaseOrder: PurchaseOrder(),
                            trashOrder: TrashOrder());
            }

            public static ICardPicker PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 2),
                           CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                           CardAcceptance.For(Cards.Gold),
                           CardAcceptance.For(Cards.Familiar, 3),
                           CardAcceptance.For(Cards.Silver, 2),                           
                           CardAcceptance.For(Cards.Potion, 1),
                           CardAcceptance.For(Cards.Silver));
            }

            public static ICardPicker TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Curse),
                           CardAcceptance.For(Cards.Silver),
                           CardAcceptance.For(Cards.Copper));
            }
        }
    }
}
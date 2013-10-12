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
        public static class GardensBeggarIronworks
        {
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "GardensBeggarIronworks",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),             
                            gainOrder: PurchaseOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder());
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(                           
                           CardAcceptance.For<CardTypes.IronWorks>(),
                           CardAcceptance.For<CardTypes.Silver>(gameState => gameState.Self.Hand.CountWhere(c => c.isAction) == 0 &&
                                                                gameState.Self.Hand.CountOf(CardTypes.Copper.card) == 3),
                           CardAcceptance.For<CardTypes.Beggar>(gameState => gameState.Self.Hand.CountWhere(c => c.isAction) > 0),                           
                           CardAcceptance.For<CardTypes.Gardens>(),
                           CardAcceptance.For<CardTypes.SilkRoad>(),
                           CardAcceptance.For<CardTypes.Beggar>(ShouldByLastCard<CardTypes.Beggar>),
                           CardAcceptance.For<CardTypes.Estate>(),                           
                           CardAcceptance.For<CardTypes.Copper>());
            }

            private static bool ShouldByLastCard<T>(GameState gameState)
                where T : Card, new()
            {
                if (CountOfPile<T>(gameState) != 1)
                    return true;
                
                return CountOfPile<CardTypes.Province>(gameState) == 1;                
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(                           
                           CardAcceptance.For<CardTypes.IronWorks>(),
                           CardAcceptance.For<CardTypes.NobleBrigand>(),
                           CardAcceptance.For<CardTypes.YoungWitch>(),
                           CardAcceptance.For<CardTypes.Beggar>());
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Gold>(),
                    CardAcceptance.For<CardTypes.Silver>());
            }
        }
    }
}

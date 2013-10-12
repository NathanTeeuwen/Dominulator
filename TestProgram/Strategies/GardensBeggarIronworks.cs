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
                           CardAcceptance.For(CardTypes.IronWorks.card),
                           CardAcceptance.For(CardTypes.Silver.card, gameState => gameState.Self.Hand.CountWhere(c => c.isAction) == 0 &&
                                                                gameState.Self.Hand.CountOf(CardTypes.Copper.card) == 3),
                           CardAcceptance.For(CardTypes.Beggar.card, gameState => gameState.Self.Hand.CountWhere(c => c.isAction) > 0),                           
                           CardAcceptance.For(CardTypes.Gardens.card),
                           CardAcceptance.For(CardTypes.SilkRoad.card),
                           CardAcceptance.For(CardTypes.Beggar.card, gameState => ShouldByLastCard(CardTypes.Beggar.card, gameState)),
                           CardAcceptance.For(CardTypes.Estate.card),                           
                           CardAcceptance.For(CardTypes.Copper.card));
            }

            private static bool ShouldByLastCard(Card card, GameState gameState)
            {
                if (CountOfPile(card, gameState) != 1)
                    return true;
                
                return CountOfPile(CardTypes.Province.card, gameState) == 1;                
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(                           
                           CardAcceptance.For(CardTypes.IronWorks.card),
                           CardAcceptance.For(CardTypes.NobleBrigand.card),
                           CardAcceptance.For(CardTypes.YoungWitch.card),
                           CardAcceptance.For(CardTypes.Beggar.card));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(CardTypes.Gold.card),
                    CardAcceptance.For(CardTypes.Silver.card));
            }
        }
    }
}

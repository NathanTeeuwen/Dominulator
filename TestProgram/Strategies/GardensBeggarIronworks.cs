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
                           CardAcceptance.For(Cards.IronWorks),
                           CardAcceptance.For(Cards.Silver, gameState => gameState.Self.Hand.CountWhere(c => c.isAction) == 0 &&
                                                                gameState.Self.Hand.CountOf(Cards.Copper) == 3),
                           CardAcceptance.For(Cards.Beggar, gameState => gameState.Self.Hand.CountWhere(c => c.isAction) > 0),                           
                           CardAcceptance.For(Cards.Gardens),
                           CardAcceptance.For(Cards.SilkRoad),
                           CardAcceptance.For(Cards.Beggar, gameState => ShouldByLastCard(Cards.Beggar, gameState)),
                           CardAcceptance.For(Cards.Estate),                           
                           CardAcceptance.For(Cards.Copper));
            }

            private static bool ShouldByLastCard(Card card, GameState gameState)
            {
                if (CountOfPile(card, gameState) != 1)
                    return true;
                
                return CountOfPile(Cards.Province, gameState) == 1;                
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(                           
                           CardAcceptance.For(Cards.IronWorks),
                           CardAcceptance.For(Cards.NobleBrigand),
                           CardAcceptance.For(Cards.YoungWitch),
                           CardAcceptance.For(Cards.Beggar));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(Cards.Gold),
                    CardAcceptance.For(Cards.Silver));
            }
        }
    }
}

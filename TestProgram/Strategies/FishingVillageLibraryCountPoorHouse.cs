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
        public static class FishingVillageLibraryCountPoorHouse
        {
            
            public static PlayerAction Player()
            {
                return new MyPlayerAction();
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction()
                    : base( "FishingVillageLibraryCountPoorHouse",                            
                            purchaseOrder: PurchaseOrder(TrashOrder()),                                                    
                            trashOrder: TrashOrder(),
                            discardOrder: DiscardOrder())
                {
                    this.actionOrder = ActionOrder(this);
                }                

                public override bool ShouldPutCardInHand(GameState gameState, Card card)
                {
                    if (!this.IsGainingCard(Cards.Province,gameState) &&
                        gameState.Self.Hand.CountOf(Cards.Count) > 0)
                    {
                        return false;
                    }
                    return true;
                }

                public bool ShouldPlayLibraryBeforeCount(GameState gameState)
                {
                    int countToTrash = CountAllOwned(TrashOrder(), gameState);
                    return countToTrash >= 3 ? true : false;
                }

                public bool ShouldPlayLibrary(GameState gameState)
                {
                    if (!ShouldPlayAction(gameState))
                    {
                        return false;
                    }

                    if (gameState.Self.Hand.CountWhere(card => card.isAction && card != Cards.Library) > 0 &&
                        gameState.Self.AvailableActions == 1)
                    {
                        return false;
                    }

                    return true;
                }

                public bool ShouldPlayPoorHouse(GameState gameState)
                {
                    if (!ShouldPlayAction(gameState))
                    {
                        return false;
                    }

                    return gameState.Self.Hand.Where(card => card.isTreasure).Count() <= 3;
                }

                public bool ShouldPlayAction(GameState gameState)
                {
                    return !ShouldTopDeckAndTrash(gameState);
                }

                public bool ShouldTopDeckAndTrash(GameState gameState)
                {
                    return HasExactlyOneActionOtherThanCount(gameState) && !this.IsGainingCard(Cards.Province, gameState);
                }

                public static bool HasExactlyOneActionOtherThanCount(GameState gameState)
                {
                    var self = gameState.Self;
                    if (!self.Hand.HasCard(Cards.Count))
                    {
                        return false;
                    }

                    if (self.Hand.CountWhere(card => card.isAction) != 2)
                    {
                        return false;
                    }

                    if (self.Hand.HasCard(Cards.Library) && self.AvailableActions >= 2)
                    {
                        return false;
                    }

                    return true;
                }
            }

            private static ICardPicker PurchaseOrder(ICardPicker trashOrder)
            {
                var highPriority = new CardPickByPriority(
                           CardAcceptance.For(Cards.Province, gameState => CountAllOwned(trashOrder, gameState) <= 3),
                           CardAcceptance.For(Cards.Library, gameState => CountAllOwned(Cards.Library, gameState) < 1),
                           CardAcceptance.For(Cards.Count, gameState => CountAllOwned(Cards.Count, gameState) < 1));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(Cards.FishingVillage),
                    CardAcceptance.For(Cards.Library),
                    CardAcceptance.For(Cards.Count),
                    CardAcceptance.For(Cards.Library));

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For(Cards.PoorHouse, gameState => CountAllOwned(Cards.PoorHouse, gameState) < 2 &&
                                                                                CountAllOwned(Cards.Count, gameState) >= 1),
                           CardAcceptance.For(Cards.FishingVillage));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static CardPickByPriority ActionOrder(MyPlayerAction playerAction)
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.FishingVillage, playerAction.ShouldPlayAction),
                           CardAcceptance.For(Cards.PoorHouse, playerAction.ShouldPlayPoorHouse),
                           CardAcceptance.For(Cards.Library, playerAction.ShouldPlayLibraryBeforeCount),
                           CardAcceptance.For(Cards.Count),
                           CardAcceptance.For(Cards.Library, playerAction.ShouldPlayLibrary));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Estate),
                           CardAcceptance.For(Cards.Copper),
                           CardAcceptance.For(Cards.Silver));
            }

            private static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Province),
                           CardAcceptance.For(Cards.Duchy),
                           CardAcceptance.For(Cards.Copper),
                           CardAcceptance.For(Cards.Estate),
                           CardAcceptance.For(Cards.PoorHouse),
                           CardAcceptance.For(Cards.Library),
                           CardAcceptance.For(Cards.FishingVillage),
                           CardAcceptance.For(Cards.Count));
            }                                          
        }
    }
}

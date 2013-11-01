using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program;

namespace Program.Kingdoms
{
    static class ShouldMounteBankHoardOrVineyard
    {        
        public static void Run()
        {
            GameConfig gameConfig = GameConfigBuilder.Create(
                StartingCardSplit.Split43,
                Cards.Bishop,
                Cards.FarmingVillage,
                Cards.GrandMarket,
                Cards.Hamlet,
                Cards.Hoard,
                Cards.Monument,
                Cards.Mountebank,
                Cards.PhilosophersStone,
                Cards.ScryingPool,
                Cards.Vineyard
                );

            Program.ComparePlayers(Strategies.MountebankMonumentHamletVineyard.Player(), Strategies.BigMoney.Player(), gameConfig);
            Program.ComparePlayers(Strategies.MountebankHoard.Player(), Strategies.BigMoney.Player(), gameConfig);
            Program.ComparePlayers(Strategies.MountebankMonumentHamletVineyard.Player(), Strategies.MountebankHoard.Player(), gameConfig);            
        }
    }
}

namespace Strategies
{
    public class MountebankMonumentHamletVineyard
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
                : base("MountebankMonumentHamletVineyard",
                    purchaseOrder: PurchaseOrder(),
                    actionOrder: ActionOrder())
            {
            }
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Vineyard, gameState => CountOfPile(Cards.Province, gameState) < 4 || CountOfPile(Cards.Duchy, gameState) < 4 || CountOfPile(Cards.Vineyard, gameState) < 4),
                        CardAcceptance.For(Cards.Province),                           
                        CardAcceptance.For(Cards.ScryingPool),                                                      
                        CardAcceptance.For(Cards.Vineyard),                           
                        CardAcceptance.For(Cards.GrandMarket),
                        CardAcceptance.For(Cards.Mountebank, 2),                           
                        CardAcceptance.For(Cards.Monument, 1),                           
                        CardAcceptance.For(Cards.Silver, 1),                                                      
                        CardAcceptance.For(Cards.Potion, 4),
                        CardAcceptance.For(Cards.Mountebank),                           
                        CardAcceptance.For(Cards.Hamlet, gameState =>CountOfPile(Cards.Hamlet, gameState) >= 2),                           
                        //CardAcceptance.For(Cards.Hamlet),                           
                        CardAcceptance.For(Cards.FarmingVillage));
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.ScryingPool),
                        CardAcceptance.For(Cards.GrandMarket),
                        CardAcceptance.For(Cards.Hamlet),
                        CardAcceptance.For(Cards.Mountebank),
                        CardAcceptance.For(Cards.Monument));
        }
    }
}

namespace Strategies
{
    public class MountebankHoard
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
                : base("MountebankHoard",
                    purchaseOrder: PurchaseOrder(),
                    actionOrder: ActionOrder(),
                    discardOrder: DiscardOrder())
            {
            }
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(                               
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy, gameState => gameState.Self.CardsInPlay.HasCard(Cards.Hoard)),
                        CardAcceptance.For(Cards.Estate, gameState => gameState.Self.CardsInPlay.HasCard(Cards.Hoard)),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 7 || CountOfPile(Cards.Vineyard, gameState) <= 7),
                        CardAcceptance.For(Cards.Hamlet, gameState => CountOfPile(Cards.Duchy, gameState) == 0 && CountOfPile(Cards.Curse, gameState) == 0),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 3 || CountOfPile(Cards.Vineyard, gameState) <= 3 || CountOfPile(Cards.Duchy, gameState) <= 2),
                        CardAcceptance.For(Cards.GrandMarket),
                        CardAcceptance.For(Cards.Mountebank, 2),                            
                        CardAcceptance.For(Cards.Hoard),                            
                        CardAcceptance.For(Cards.Monument, 1),
                        CardAcceptance.For(Cards.Silver),
                        CardAcceptance.For(Cards.Hamlet));
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.GrandMarket),
                        CardAcceptance.For(Cards.Hamlet),
                        CardAcceptance.For(Cards.Mountebank),
                        CardAcceptance.For(Cards.Monument));
        }

        private static CardPickByPriority DiscardOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy),
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.Curse));
        }
    }
}

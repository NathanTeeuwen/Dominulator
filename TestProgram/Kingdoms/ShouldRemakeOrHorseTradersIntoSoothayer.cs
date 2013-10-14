using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Kingdoms
{
    static class ShouldRemakeOrHorseTradersIntoSoothayer
    {
        // forum post:  http://forum.dominionstrategy.com/index.php?topic=9602.0
        /*  any attempt to get more than one remake, or more than 2 sooth sayers results in a loss ....  (but in real game, the plan was 3 remakes and 3 soothsayers ...)
         * 
         * 
         * */        

        public static void Run()
        {
            GameConfig gameConfig = GameConfigBuilder.Create(
                StartingCardSplit.Split43,
                Cards.Butcher,
                Cards.GreatHall,
                Cards.HornOfPlenty,
                Cards.HorseTraders,
                Cards.Minion,
                Cards.Pawn,
                Cards.Remake,
                Cards.Soothsayer,
                Cards.StoneMason,
                Cards.Swindler
                );

            //Program.ComparePlayers(Strategies.HorseTraderSoothsayerMinionGreatHall.Player(1), Strategies.HorseTraderSoothsayerMinionGreatHall.Player(2, false), gameConfig);
            Program.ComparePlayers(Strategies.HorseTraderSoothsayerMinionGreatHall.Player(), Strategies.BigMoney.Player(), gameConfig);
            Program.ComparePlayers(Strategies.RemakeSoothsayer.Player(), Strategies.BigMoney.Player(), gameConfig);
            Program.ComparePlayers(Strategies.RemakeSoothsayer.Player(), Strategies.HorseTraderSoothsayerMinionGreatHall.Player(), gameConfig);
        }
    }
}

namespace Program
{
    public static partial class Strategies
    {
        public static class HorseTraderSoothsayerMinionGreatHall
        {
            public static PlayerAction Player()
            {
                return new MyPlayerAction();
            }           

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction()
                    : base("HorseTraderSoothsayerMinionGreatHall",                        
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder())
                {
                }

                public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {
                    if (gameState.CurrentCardBeingPlayed == Cards.Minion)
                    {
                        if (CountInHand(Cards.Minion, gameState) >= 2)
                            return PlayerActionChoice.PlusCoin;

                        if (HasCardInHand(Cards.Butcher, gameState) && HasCardInHand(Cards.Gold, gameState))
                            return PlayerActionChoice.PlusCoin;

                        if(gameState.Self.ExpectedCoinValueAtEndOfTurn + CountInHand(Cards.Minion, gameState) * 2 >= 6)
                            return PlayerActionChoice.PlusCoin;

                        if (HasCardInHand(Cards.Soothsayer, gameState))
                        {                            
                            return PlayerActionChoice.PlusCoin;                            
                        }

                        return PlayerActionChoice.Discard;
                    }

                    if (gameState.CurrentCardBeingPlayed == Cards.Pawn)
                    {
                        if (acceptableChoice(PlayerActionChoice.PlusAction))
                            return PlayerActionChoice.PlusAction;
                        return PlayerActionChoice.PlusCard;
                    }

                    return base.ChooseBetween(gameState, acceptableChoice);
                }
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Province, CardAcceptance.AlwaysMatch, CardAcceptance.OverPayMaxAmount),
                           CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 6, CardAcceptance.OverPayMaxAmount),
                           CardAcceptance.For(Cards.Soothsayer, 1),
                           CardAcceptance.For(Cards.Butcher, 1, gameState => CountAllOwned(Cards.Minion, gameState) >= 3),
                           CardAcceptance.For(Cards.Gold, gameState => CountAllOwned(Cards.Minion, gameState) >= 3),                           
                           CardAcceptance.For(Cards.Minion),
                           CardAcceptance.For(Cards.HorseTraders, 1),
                           CardAcceptance.For(Cards.Silver, 1),
                           CardAcceptance.For(Cards.GreatHall, gameState => CountOfPile(Cards.GreatHall, gameState) >= 2),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                           CardAcceptance.For(Cards.Silver),
                           CardAcceptance.For(Cards.Estate, gameState => CardBeingPlayedIs(Cards.Butcher, gameState))
                           );
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.GreatHall),
                           CardAcceptance.For(Cards.Minion),
                           CardAcceptance.For(Cards.Butcher),
                           CardAcceptance.For(Cards.Soothsayer),
                           CardAcceptance.For(Cards.HorseTraders));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                        CardAcceptance.For(Cards.Gold, gameState => CardBeingPlayedIs(Cards.Butcher, gameState)),
                        CardAcceptance.For(Cards.Soothsayer, gameState => CardBeingPlayedIs(Cards.Butcher, gameState)),
                        CardAcceptance.For(Cards.HorseTraders, gameState => CardBeingPlayedIs(Cards.Butcher, gameState)),
                        CardAcceptance.For(Cards.Curse),                        
                        CardAcceptance.For(Cards.Copper, gameState => gameState.Self.ExpectedCoinValueAtEndOfTurn < 8),
                        CardAcceptance.For(Cards.Estate));
            }
        }
    }

    public static partial class Strategies
    {
        public static class RemakeSoothsayer
        {
            public static PlayerAction Player()
            {
                return new MyPlayerAction();
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction()
                    : base("RemakeSoothsayer",                        
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder())
                {
                }
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Province),
                           CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                           CardAcceptance.For(Cards.Soothsayer, 1),                                                      
                           CardAcceptance.For(Cards.Gold, 1),
                           CardAcceptance.For(Cards.Soothsayer, 2),
                           CardAcceptance.For(Cards.Gold),
                           CardAcceptance.For(Cards.Duchy, gameState => CardBeingPlayedIs(Cards.Remake, gameState)),
                           CardAcceptance.For(Cards.Remake, 1),
                           //CardAcceptance.For(Cards.Remake, gameState => ((double)CountAllOwned(TrashOrderWithoutRemake(), gameState)) / gameState.Self.AllOwnedCards.Count > 0.4),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                           CardAcceptance.For(Cards.GreatHall, gameState => CountAllOwned(Cards.Silver, gameState) >= 2),
                           CardAcceptance.For(Cards.Silver)
                           );
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.GreatHall),
                           CardAcceptance.For(Cards.Soothsayer),
                           CardAcceptance.For(Cards.Remake, ShouldPlayRemake));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                        CardAcceptance.For(Cards.Curse),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) > 2),
                        CardAcceptance.For(Cards.Remake),
                        CardAcceptance.For(Cards.Copper));
            }

            private static CardPickByPriority TrashOrderWithoutRemake()
            {
                return new CardPickByPriority(
                        CardAcceptance.For(Cards.Curse),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) > 2),                        
                        CardAcceptance.For(Cards.Copper));
            }

            private static bool ShouldPlayRemake(GameState gameState)
            {                
                return CountInHandFrom(TrashOrderWithoutRemake(), gameState) >= 2;
            }
        }
    }
}

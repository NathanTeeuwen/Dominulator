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
                Card.Type<CardTypes.Butcher>(),
                Card.Type<CardTypes.GreatHall>(),
                Card.Type<CardTypes.HornOfPlenty>(),
                Card.Type<CardTypes.HorseTraders>(),
                Card.Type<CardTypes.Minion>(),
                Card.Type<CardTypes.Pawn>(),
                Card.Type<CardTypes.Remake>(),
                Card.Type<CardTypes.Soothsayer>(),
                Card.Type<CardTypes.StoneMason>(),
                Card.Type<CardTypes.Swindler>()
                );

            //Program.ComparePlayers(Strategies.HorseTraderSoothsayerMinionGreatHall.Player(1), Strategies.HorseTraderSoothsayerMinionGreatHall.Player(2, false), gameConfig);
            Program.ComparePlayers(Strategies.HorseTraderSoothsayerMinionGreatHall.Player(1), Strategies.BigMoney.Player(2), gameConfig);
            Program.ComparePlayers(Strategies.RemakeSoothsayer.Player(1), Strategies.BigMoney.Player(2), gameConfig);
            Program.ComparePlayers(Strategies.RemakeSoothsayer.Player(1), Strategies.HorseTraderSoothsayerMinionGreatHall.Player(2), gameConfig);
        }
    }
}

namespace Program
{
    public static partial class Strategies
    {
        public static class HorseTraderSoothsayerMinionGreatHall
        {
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }           

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("HorseTraderSoothsayerMinionGreatHall",
                        playerNumber,
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder())
                {
                }

                public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {
                    if (gameState.CurrentCardBeingPlayed.Is<CardTypes.Minion>())
                    {
                        if (CountInHand<CardTypes.Minion>(gameState) >= 2)
                            return PlayerActionChoice.PlusCoin;

                        if (HasCardInHand<CardTypes.Butcher>(gameState) && HasCardInHand<CardTypes.Gold>(gameState))
                            return PlayerActionChoice.PlusCoin;

                        if(gameState.Self.ExpectedCoinValueAtEndOfTurn + CountInHand<CardTypes.Minion>(gameState) * 2 >= 6)
                            return PlayerActionChoice.PlusCoin;

                        if (HasCardInHand<CardTypes.Soothsayer>(gameState))
                        {                            
                            return PlayerActionChoice.PlusCoin;                            
                        }

                        return PlayerActionChoice.Discard;
                    }

                    if (gameState.CurrentCardBeingPlayed.Is<CardTypes.Pawn>())
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
                           CardAcceptance.For<CardTypes.Province>    (CardAcceptance.AlwaysMatch, CardAcceptance.OverPayMaxAmount),
                           CardAcceptance.For<CardTypes.Duchy>       (gameState => CountOfPile<CardTypes.Province>(gameState) <= 6, CardAcceptance.OverPayMaxAmount),
                           CardAcceptance.For<CardTypes.Soothsayer>  (1),
                           CardAcceptance.For<CardTypes.Butcher>     (1, gameState => CountAllOwned<CardTypes.Minion>(gameState) >= 3),
                           CardAcceptance.For<CardTypes.Gold>        (gameState => CountAllOwned<CardTypes.Minion>(gameState) >= 3),                           
                           CardAcceptance.For<CardTypes.Minion>      (),
                           CardAcceptance.For<CardTypes.HorseTraders>(1),
                           CardAcceptance.For<CardTypes.Silver>      (1),
                           CardAcceptance.For<CardTypes.GreatHall>   (gameState => CountOfPile<CardTypes.GreatHall>(gameState) >= 2),
                           CardAcceptance.For<CardTypes.Estate>      (gameState => CountOfPile<CardTypes.Province>(gameState) <= 4),
                           CardAcceptance.For<CardTypes.Silver>      (),
                           CardAcceptance.For<CardTypes.Estate>      (gameState => CardBeingPlayedIs<CardTypes.Butcher>(gameState))
                           );
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.GreatHall>(),
                           CardAcceptance.For<CardTypes.Minion>(),
                           CardAcceptance.For<CardTypes.Butcher>(),
                           CardAcceptance.For<CardTypes.Soothsayer>(),
                           CardAcceptance.For<CardTypes.HorseTraders>());
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                        CardAcceptance.For<CardTypes.Gold>(gameState => CardBeingPlayedIs<CardTypes.Butcher>(gameState)),
                        CardAcceptance.For<CardTypes.Soothsayer>(gameState => CardBeingPlayedIs<CardTypes.Butcher>(gameState)),
                        CardAcceptance.For<CardTypes.HorseTraders>(gameState => CardBeingPlayedIs<CardTypes.Butcher>(gameState)),
                        CardAcceptance.For<CardTypes.Curse>(),                        
                        CardAcceptance.For<CardTypes.Copper>(gameState => gameState.Self.ExpectedCoinValueAtEndOfTurn < 8),
                        CardAcceptance.For<CardTypes.Estate>());
            }
        }
    }

    public static partial class Strategies
    {
        public static class RemakeSoothsayer
        {
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("RemakeSoothsayer",
                        playerNumber,
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder())
                {
                }
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 4),
                           CardAcceptance.For<CardTypes.Soothsayer>(1),                                                      
                           CardAcceptance.For<CardTypes.Gold>(1),
                           CardAcceptance.For<CardTypes.Soothsayer>(2),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CardBeingPlayedIs<CardTypes.Remake>(gameState)),
                           CardAcceptance.For<CardTypes.Remake>(1),
                           //CardAcceptance.For<CardTypes.Remake>(gameState => ((double)CountAllOwned(TrashOrderWithoutRemake(), gameState)) / gameState.Self.AllOwnedCards.Count > 0.4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 2),
                           CardAcceptance.For<CardTypes.GreatHall>(gameState => CountAllOwned<CardTypes.Silver>(gameState) >= 2),
                           CardAcceptance.For<CardTypes.Silver>()
                           );
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.GreatHall>(),
                           CardAcceptance.For<CardTypes.Soothsayer>(),
                           CardAcceptance.For<CardTypes.Remake>(ShouldPlayRemake));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                        CardAcceptance.For<CardTypes.Curse>(),
                        CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) > 2),
                        CardAcceptance.For<CardTypes.Remake>(),
                        CardAcceptance.For<CardTypes.Copper>());
            }

            private static CardPickByPriority TrashOrderWithoutRemake()
            {
                return new CardPickByPriority(
                        CardAcceptance.For<CardTypes.Curse>(),
                        CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) > 2),                        
                        CardAcceptance.For<CardTypes.Copper>());
            }

            private static bool ShouldPlayRemake(GameState gameState)
            {                
                return CountInHandFrom(TrashOrderWithoutRemake(), gameState) >= 2;
            }
        }
    }
}

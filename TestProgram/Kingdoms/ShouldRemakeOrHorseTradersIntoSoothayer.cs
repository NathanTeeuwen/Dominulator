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
                CardTypes.Butcher.card,
                CardTypes.GreatHall.card,
                CardTypes.HornOfPlenty.card,
                CardTypes.HorseTraders.card,
                CardTypes.Minion.card,
                CardTypes.Pawn.card,
                CardTypes.Remake.card,
                CardTypes.Soothsayer.card,
                CardTypes.StoneMason.card,
                CardTypes.Swindler.card
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
                    if (gameState.CurrentCardBeingPlayed == CardTypes.Minion.card)
                    {
                        if (CountInHand(CardTypes.Minion.card, gameState) >= 2)
                            return PlayerActionChoice.PlusCoin;

                        if (HasCardInHand(CardTypes.Butcher.card, gameState) && HasCardInHand(CardTypes.Gold.card, gameState))
                            return PlayerActionChoice.PlusCoin;

                        if(gameState.Self.ExpectedCoinValueAtEndOfTurn + CountInHand(CardTypes.Minion.card, gameState) * 2 >= 6)
                            return PlayerActionChoice.PlusCoin;

                        if (HasCardInHand(CardTypes.Soothsayer.card, gameState))
                        {                            
                            return PlayerActionChoice.PlusCoin;                            
                        }

                        return PlayerActionChoice.Discard;
                    }

                    if (gameState.CurrentCardBeingPlayed == CardTypes.Pawn.card)
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
                           CardAcceptance.For(CardTypes.Province.card, CardAcceptance.AlwaysMatch, CardAcceptance.OverPayMaxAmount),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 6, CardAcceptance.OverPayMaxAmount),
                           CardAcceptance.For(CardTypes.Soothsayer.card, 1),
                           CardAcceptance.For(CardTypes.Butcher.card, 1, gameState => CountAllOwned(CardTypes.Minion.card, gameState) >= 3),
                           CardAcceptance.For(CardTypes.Gold.card, gameState => CountAllOwned(CardTypes.Minion.card, gameState) >= 3),                           
                           CardAcceptance.For(CardTypes.Minion.card),
                           CardAcceptance.For(CardTypes.HorseTraders.card, 1),
                           CardAcceptance.For(CardTypes.Silver.card, 1),
                           CardAcceptance.For(CardTypes.GreatHall.card, gameState => CountOfPile(CardTypes.GreatHall.card, gameState) >= 2),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 4),
                           CardAcceptance.For(CardTypes.Silver.card),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CardBeingPlayedIs(CardTypes.Butcher.card, gameState))
                           );
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.GreatHall.card),
                           CardAcceptance.For(CardTypes.Minion.card),
                           CardAcceptance.For(CardTypes.Butcher.card),
                           CardAcceptance.For(CardTypes.Soothsayer.card),
                           CardAcceptance.For(CardTypes.HorseTraders.card));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                        CardAcceptance.For(CardTypes.Gold.card, gameState => CardBeingPlayedIs(CardTypes.Butcher.card, gameState)),
                        CardAcceptance.For(CardTypes.Soothsayer.card, gameState => CardBeingPlayedIs(CardTypes.Butcher.card, gameState)),
                        CardAcceptance.For(CardTypes.HorseTraders.card, gameState => CardBeingPlayedIs(CardTypes.Butcher.card, gameState)),
                        CardAcceptance.For(CardTypes.Curse.card),                        
                        CardAcceptance.For(CardTypes.Copper.card, gameState => gameState.Self.ExpectedCoinValueAtEndOfTurn < 8),
                        CardAcceptance.For(CardTypes.Estate.card));
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
                           CardAcceptance.For(CardTypes.Province.card),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 4),
                           CardAcceptance.For(CardTypes.Soothsayer.card, 1),                                                      
                           CardAcceptance.For(CardTypes.Gold.card, 1),
                           CardAcceptance.For(CardTypes.Soothsayer.card, 2),
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CardBeingPlayedIs(CardTypes.Remake.card, gameState)),
                           CardAcceptance.For(CardTypes.Remake.card, 1),
                           //CardAcceptance.For(CardTypes.Remake.card, gameState => ((double)CountAllOwned(TrashOrderWithoutRemake(), gameState)) / gameState.Self.AllOwnedCards.Count > 0.4),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 2),
                           CardAcceptance.For(CardTypes.GreatHall.card, gameState => CountAllOwned(CardTypes.Silver.card, gameState) >= 2),
                           CardAcceptance.For(CardTypes.Silver.card)
                           );
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.GreatHall.card),
                           CardAcceptance.For(CardTypes.Soothsayer.card),
                           CardAcceptance.For(CardTypes.Remake.card, ShouldPlayRemake));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                        CardAcceptance.For(CardTypes.Curse.card),
                        CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) > 2),
                        CardAcceptance.For(CardTypes.Remake.card),
                        CardAcceptance.For(CardTypes.Copper.card));
            }

            private static CardPickByPriority TrashOrderWithoutRemake()
            {
                return new CardPickByPriority(
                        CardAcceptance.For(CardTypes.Curse.card),
                        CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) > 2),                        
                        CardAcceptance.For(CardTypes.Copper.card));
            }

            private static bool ShouldPlayRemake(GameState gameState)
            {                
                return CountInHandFrom(TrashOrderWithoutRemake(), gameState) >= 2;
            }
        }
    }
}

using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{   
    public class FeodumTraderCountingHouse
        : Strategy
    {
            
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "FeodumTraderCountingHouse",                            
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder());
        }

        private static ICardPicker PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Trader, 2),
                        CardAcceptance.For(Cards.Feodum, ShouldGainFeodum),
                        //CardAcceptance.For(Cards.CountingHouse, 1),
                        //CardAcceptance.For(Cards.Duchy),
                        CardAcceptance.For(Cards.Silver),
                        CardAcceptance.For(Cards.Estate));
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.CountingHouse),
                        CardAcceptance.For(Cards.Trader, ShouldPlayTrader));
        }

        private static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Feodum, ShouldTrashFeodum),
                        CardAcceptance.For(Cards.Estate, ShouldTrashEstate),
                        CardAcceptance.For(Cards.Silver, gameState => CountOfPile(Cards.Silver, gameState) >=2 ),
                        CardAcceptance.For(Cards.Curse),
                        CardAcceptance.For(Cards.RuinedLibrary),
                        CardAcceptance.For(Cards.RuinedMarket),
                        CardAcceptance.For(Cards.RuinedVillage),
                        CardAcceptance.For(Cards.AbandonedMine),
                        CardAcceptance.For(Cards.Survivors));
        }

        private static bool ShouldPlayTrader(GameState gameState)
        {
            return true;
        }

        private static bool ShouldTrashEstate(GameState gameState)
        {
            return true;
        }

        private static bool ShouldTrashFeodum(GameState gameState)
        {
            int countFeodumRemaining = CountOfPile(Cards.Feodum, gameState);

            int countSilvers = CountAllOwned(Cards.Silver, gameState);
            int countFeodum = CountAllOwned(Cards.Feodum, gameState);

            if (countSilvers < 12)
            {
                return true;
            }

            int scoreTrashNothing = CardTypes.Feodum.VictoryCountForSilver(countSilvers) * countFeodum;
            int scoreTrashFeodum = CardTypes.Feodum.VictoryCountForSilver((countSilvers + 4)) * (countFeodum - 1);

            return scoreTrashFeodum > scoreTrashNothing;
        }

        private static bool ShouldGainFeodum(GameState gameState)
        {
            int countFeodumRemaining = CountOfPile(Cards.Feodum, gameState);

            int countSilvers = CountAllOwned(Cards.Silver, gameState);
            int countFeodum = CountAllOwned(Cards.Feodum, gameState);

            if (countSilvers < 1)
            {
                return false;
            }

            int scoreGainFeodum = CardTypes.Feodum.VictoryCountForSilver(countSilvers) * (countFeodum + 1);
            int scoreGainSilver = CardTypes.Feodum.VictoryCountForSilver((countSilvers + 1)) * (countFeodum);

            return scoreGainFeodum > scoreGainSilver;
        }
    }
}

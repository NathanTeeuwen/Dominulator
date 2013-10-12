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
        public static class FeodumDevelop
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "FeodumDevelop",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder(),                            
                            gainOrder: GainOrder(),
                            chooseDefaultActionOnNone:false);
            }

            private static ICardPicker PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Province),
                           CardAcceptance.For(Cards.Develop, ShouldGainDevelop),
                           CardAcceptance.For(Cards.Feodum, ShouldGainFeodum),
                           CardAcceptance.For(Cards.Silver));
            }

            private static ICardPicker GainOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Develop, ShouldGainDevelop),
                           CardAcceptance.For(Cards.Feodum, ShouldGainFeodum),
                           CardAcceptance.For(Cards.Silver),
                           CardAcceptance.For(Cards.Duchy),
                           CardAcceptance.For(Cards.Feodum),
                           CardAcceptance.For(Cards.Develop));
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Develop, ShouldPlayDevelop));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Duchy),
                           CardAcceptance.For(Cards.Feodum, ShouldTrashFeodum),
                           CardAcceptance.For(Cards.Estate),
                           CardAcceptance.For(Cards.Copper));
            }

            private static bool ShouldGainDevelop(GameState gameState)
            {
                return CountAllOwned(Cards.Develop, gameState) < 2 &&
                       CountAllOwned(Cards.Feodum, gameState) >= CountAllOwned(Cards.Develop, gameState);
            }

            private static bool ShouldPlayDevelop(GameState gameState)
            {
                var self = gameState.Self;

                Card result;
                if (self.Hand.CountOf(Cards.Develop) > 1)
                {
                    result = TrashOrder().GetPreferredCard(gameState, card => self.Hand.HasCard(card));
                }
                else
                {
                    result = TrashOrder().GetPreferredCard(gameState, card => self.Hand.HasCard(card) && card != Cards.Develop);
                }

                return result != null;
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
}

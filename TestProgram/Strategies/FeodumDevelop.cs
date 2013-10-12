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
                           CardAcceptance.For(CardTypes.Province.card),
                           CardAcceptance.For(CardTypes.Develop.card, ShouldGainDevelop),
                           CardAcceptance.For(CardTypes.Feodum.card, ShouldGainFeodum),
                           CardAcceptance.For(CardTypes.Silver.card));
            }

            private static ICardPicker GainOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Develop.card, ShouldGainDevelop),
                           CardAcceptance.For(CardTypes.Feodum.card, ShouldGainFeodum),
                           CardAcceptance.For(CardTypes.Silver.card),
                           CardAcceptance.For(CardTypes.Duchy.card),
                           CardAcceptance.For(CardTypes.Feodum.card),
                           CardAcceptance.For(CardTypes.Develop.card));
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Develop.card, ShouldPlayDevelop));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Duchy.card),
                           CardAcceptance.For(CardTypes.Feodum.card, ShouldTrashFeodum),
                           CardAcceptance.For(CardTypes.Estate.card),
                           CardAcceptance.For(CardTypes.Copper.card));
            }

            private static bool ShouldGainDevelop(GameState gameState)
            {
                return CountAllOwned(CardTypes.Develop.card, gameState) < 2 &&
                       CountAllOwned(CardTypes.Feodum.card, gameState) >= CountAllOwned(CardTypes.Develop.card, gameState);
            }

            private static bool ShouldPlayDevelop(GameState gameState)
            {
                var self = gameState.Self;

                Card result;
                if (self.Hand.CountOf(CardTypes.Develop.card) > 1)
                {
                    result = TrashOrder().GetPreferredCard(gameState, card => self.Hand.HasCard(card));
                }
                else
                {
                    result = TrashOrder().GetPreferredCard(gameState, card => self.Hand.HasCard(card) && card != CardTypes.Develop.card);
                }

                return result != null;
            }

            private static bool ShouldTrashFeodum(GameState gameState)
            {
                int countFeodumRemaining = CountOfPile(CardTypes.Feodum.card, gameState);

                int countSilvers = CountAllOwned(CardTypes.Silver.card, gameState);
                int countFeodum = CountAllOwned(CardTypes.Feodum.card, gameState);

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
                int countFeodumRemaining = CountOfPile(CardTypes.Feodum.card, gameState);

                int countSilvers = CountAllOwned(CardTypes.Silver.card, gameState);
                int countFeodum = CountAllOwned(CardTypes.Feodum.card, gameState);

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

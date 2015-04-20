using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;

namespace Strategies
{
    /*
     * 
     * http://forum.dominionstrategy.com/index.php?topic=11000.0
     Strategy:

    Get 2 Hermits to trash your estates and Feoda and gain Silvers (3rd one when your deck is more than 20 cards)
    and gain Hermit with Hermit whenever you only have a < $3 hand (and turn the played hermit into a madman)

    Buy Feodum when you have more than 9 silver or when you have a Hermit coming up before the next reshuffle (with at least 80% chance)
    ( hermits left in draw pile >0 && cards left in drawpile [mod 5] / cards left in draw pile <= 0,2)

    -Trash Feodum over Estate, but only trash Feodum if you trashed 2 or less Feoda so far.

    -Buy Province over Feodum when you have less than 18 Silver. 

    -Buy silver on all other turns .

     * */
    public class HermitFeodum
        : Strategy
    {
            
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "HermitFeodum",                            
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder(),                            
                        gainOrder: GainOrder(),
                        chooseDefaultActionOnNone:false);
        }

        private static ICardPicker PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Silver, gameState) < 18 || CountOfPile(Cards.Feodum, gameState) == 0 ),
                        CardAcceptance.For(Cards.Feodum, ShouldGainFeodum),
                        // open up double hermit
                        CardAcceptance.For(Cards.Hermit, gameState => CountAllOwned(Cards.Silver, gameState) == 0 && CountAllOwned(Cards.Hermit, gameState) < 2),
                        CardAcceptance.For(Cards.Silver));
        }

        private static ICardPicker GainOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Hermit, gameState => gameState.Self.ExpectedCoinValueAtEndOfTurn < 3),                       
                        CardAcceptance.For(Cards.Silver),
                        CardAcceptance.For(Cards.Estate));
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Madman),
                        CardAcceptance.For(Cards.Hermit));
        }

        private static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Feodum, ShouldTrashFeodum),
                        CardAcceptance.For(Cards.Curse),
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.Copper));
        }

        private static bool ShouldTrashFeodum(GameState gameState)
        {          
            int countSilvers = CountAllOwned(Cards.Silver, gameState);
            int countFeodum = CountAllOwned(Cards.Feodum, gameState);

            // if you have trashed 2 or less feodum, you should have less than 9 silvers.
            if (countSilvers < 8)
            {
                return true;
            }

            // otherwise maximize feodum points
            int scoreTrashNothing = CardTypes.Feodum.VictoryCountForSilver(countSilvers) * countFeodum;
            int scoreTrashFeodum = CardTypes.Feodum.VictoryCountForSilver((countSilvers + 4)) * (countFeodum - 1);

            return scoreTrashFeodum > scoreTrashNothing;
        }

        private static bool ShouldGainFeodum(GameState gameState)
        {
            
            int countSilvers = CountAllOwned(Cards.Silver, gameState);

            if (countSilvers > 9)
            {
                return true;
            }

            //hermits left in draw pile >0 && cards left in drawpile [mod 5] / cards left in draw pile <= 0,2

            bool hasHermitInDrawPile = CountInDeck(Cards.Hermit, gameState) > 0;
            bool atLeast80PercentChanceOfDrawingBeforeShuffle = (((double)(gameState.Self.CardsInDeck.Count % 5)) / gameState.Self.CardsInDeck.Count) < 0.2;

            if (hasHermitInDrawPile && atLeast80PercentChanceOfDrawingBeforeShuffle)
                return true;

            return false;
        }
    }
}

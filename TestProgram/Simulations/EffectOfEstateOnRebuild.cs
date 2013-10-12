using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Simulations
{
    static public class EffectOfEstateOnRebuild
    {
        public static void Run()
        {
            Program.ComparePlayers(Strategies.RebuildAdvanced.Player(1), Strategies.RebuildAdvanced.Player(2), startingDeckPerPlayer: StartingDecksForRebuildWithEstateAdvantage.StartingDecks);            
        }

        class StartingDecksForRebuildWithEstateAdvantage
            : StartingDeckBuilder
        {
            static new public IEnumerable<CardCountPair>[] StartingDecks
            {
                get
                {
                    return StartingDecks(
                       StartingDeck(
                            CardWithCount(CardTypes.Copper.card, 7),
                            CardWithCount(CardTypes.Estate.card, 1),
                            CardWithCount(CardTypes.Silver.card, 5),
                            CardWithCount(CardTypes.Gold.card, 2),
                            CardWithCount(CardTypes.Rebuild.card, 2),
                            CardWithCount(CardTypes.Duchy.card, 4)),
                       StartingDeck(
                            CardWithCount(CardTypes.Copper.card, 7),
                            CardWithCount(CardTypes.Silver.card, 5),
                            CardWithCount(CardTypes.Gold.card, 2),
                            CardWithCount(CardTypes.Rebuild.card, 2),
                            CardWithCount(CardTypes.Duchy.card, 4))
                            );
                }
            }
        }
    }
}
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
                            CardWithCount<CardTypes.Copper>(7),
                            CardWithCount<CardTypes.Estate>(1),
                            CardWithCount<CardTypes.Silver>(5),
                            CardWithCount<CardTypes.Gold>(2),
                            CardWithCount<CardTypes.Rebuild>(2),
                            CardWithCount<CardTypes.Duchy>(4)),
                       StartingDeck(
                            CardWithCount<CardTypes.Copper>(7),
                            CardWithCount<CardTypes.Silver>(5),
                            CardWithCount<CardTypes.Gold>(2),
                            CardWithCount<CardTypes.Rebuild>(2),
                            CardWithCount<CardTypes.Duchy>(4))
                            );
                }
            }
        }
    }
}
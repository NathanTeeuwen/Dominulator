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
                            CardWithCount(Cards.Copper, 7),
                            CardWithCount(Cards.Estate, 1),
                            CardWithCount(Cards.Silver, 5),
                            CardWithCount(Cards.Gold, 2),
                            CardWithCount(Cards.Rebuild, 2),
                            CardWithCount(Cards.Duchy, 4)),
                       StartingDeck(
                            CardWithCount(Cards.Copper, 7),
                            CardWithCount(Cards.Silver, 5),
                            CardWithCount(Cards.Gold, 2),
                            CardWithCount(Cards.Rebuild, 2),
                            CardWithCount(Cards.Duchy, 4))
                            );
                }
            }
        }
    }
}
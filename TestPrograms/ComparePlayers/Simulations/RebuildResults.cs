using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Simulations
{
    static public class RebuildResults
    {
        public static void Run(TestOutput testOutput)
        {
            var rebuildStrategy = Strategies.Rebuild.Player();
            // for forum topic: http://forum.dominionstrategy.com/index.php?topic=8391.0
            testOutput.ComparePlayers(rebuildStrategy, Strategies.BigMoney.Player());
            testOutput.ComparePlayers(rebuildStrategy, Strategies.BigMoneyWithCard.Player(Cards.Wharf, cardCount: 2));
            testOutput.ComparePlayers(rebuildStrategy, Strategies.BigMoneyWithCard.Player(Cards.Mountebank, cardCount: 2));
            testOutput.ComparePlayers(rebuildStrategy, Strategies.BigMoneyWithCard.Player(Cards.Witch, cardCount: 2));
            testOutput.ComparePlayers(rebuildStrategy, Strategies.BigMoneyWithCard.Player(Cards.YoungWitch, cardCount: 2));        
        }
    }
}
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
        public static void Run()
        {
            // for forum topic: http://forum.dominionstrategy.com/index.php?topic=8391.0
            Program.ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoney.Player(2));
            Program.ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoneyWithCard.Player(Cards.Wharf, 2, cardCount: 2));
            Program.ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoneyWithCard.Player(Cards.Mountebank, 2, cardCount: 2));
            Program.ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoneyWithCard.Player(Cards.Witch, 2, cardCount: 2));
            Program.ComparePlayers(Strategies.Rebuild.Player(1), Strategies.BigMoneyWithCard.Player(Cards.YoungWitch, 2, cardCount: 2));        
        }
    }
}
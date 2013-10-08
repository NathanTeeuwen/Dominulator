using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Simulations
{
    static public class GuildsResults
    {
        public static void Run()
        {
            // for forum topic: http://forum.dominionstrategy.com/index.php?topic=8461.0
            Program.ComparePlayers(Strategies.BigMoneyWithCard<CardTypes.Soothsayer>.Player(1), Strategies.BigMoneyWithCard<CardTypes.Witch>.Player(2));
        }
    }
}
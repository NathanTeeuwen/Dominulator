using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Simulations
{
    static public class FollowersTest
    {
        public static void Run()
        {
            // for forum topic: http://forum.dominionstrategy.com/index.php?topic=6623.0
            System.Console.WriteLine("Followers Cost, Player 1 Win %, Player 2 Win%, Tie%");
            for (int i = 0; i < 16; ++i)
            {
                System.Console.Write("{0}, ", i);
                Program.ComparePlayers(Strategies.FollowersTest.TestPlayer(i), "BigMoney", showCompactScore: true);
            }
        }
    }
}
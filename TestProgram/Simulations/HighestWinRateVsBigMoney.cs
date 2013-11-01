using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Simulations
{
    static public class HighestWinRateVsBigMoney
    {
        static void Run()
        {
            // goal is to find a strategy that always beats big money.  Haven't found it yet.
            // for forum topic: http://forum.dominionstrategy.com/index.php?topic=8580.0
            //ComparePlayers(Strategies.FishingVillageChapelPoorHouseTalisman.Player(1), Strategies.BigMoney.Player(2));
            //ComparePlayers(Strategies.FishingVillageChapelPoorHouse.Player(1), Strategies.BigMoney.Player(2));            
            Program.ComparePlayers("GardensBeggarIronworks", "BigMoney", numberOfGames: 10000);
        }
    }
}
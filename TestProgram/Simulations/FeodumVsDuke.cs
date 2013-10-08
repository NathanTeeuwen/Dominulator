using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Simulations
{
    static public class FeodumVsDuke
    {
        public static void Run()
        {
            // for forum topic: http://forum.dominionstrategy.com/index.php?topic=7476.msg212934#msg212934
            Program.ComparePlayers(Strategies.DuchyDukeWarehouseEmbassy.Player(1), Strategies.BigMoney.Player(2));
            Program.ComparePlayers(Strategies.FeodumDevelop.Player(1), Strategies.BigMoney.Player(2));
            Program.ComparePlayers(Strategies.DuchyDukeWarehouseEmbassy.Player(1), Strategies.FeodumDevelop.Player(2));
        }
    }
}
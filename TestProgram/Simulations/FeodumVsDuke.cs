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
            Program.ComparePlayers(Strategies.DuchyDukeWarehouseEmbassy.Player(), Strategies.BigMoney.Player());
            Program.ComparePlayers(Strategies.FeodumDevelop.Player(), Strategies.BigMoney.Player());
            Program.ComparePlayers(Strategies.DuchyDukeWarehouseEmbassy.Player(), Strategies.FeodumDevelop.Player());
        }
    }
}
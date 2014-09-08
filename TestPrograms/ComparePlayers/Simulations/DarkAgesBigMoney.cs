using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Simulations
{
    static public class DarkAgesBigMoney
    {
        static void Run(TestOutput testOutput)
        {
            // for forum topic: http://forum.dominionstrategy.com/index.php?topic=6281.0
            testOutput.ComparePlayers("Rebuild", "BigMoney");
            //testOutput.ComparePlayers(Strategies.BigMoneyWithCard<CardTypes.Catacombs>.Player(1, 2), Strategies.BigMoney.Player(2));
            //testOutput.ComparePlayers(Strategies.BigMoneyWithCard<CardTypes.Count>.Player(1), Strategies.BigMoney.Player(2));
            testOutput.ComparePlayers(Strategies.BigMoneyWithCard.Player(Cards.HuntingGrounds), Strategies.BigMoney.Player());
        }
    }
}
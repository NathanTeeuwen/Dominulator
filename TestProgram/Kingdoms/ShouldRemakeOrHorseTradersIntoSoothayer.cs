using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program;

namespace Program.Kingdoms
{
    static class ShouldRemakeOrHorseTradersIntoSoothayer
    {
        // forum post:  http://forum.dominionstrategy.com/index.php?topic=9602.0
        /*  any attempt to get more than one remake, or more than 2 sooth sayers results in a loss ....  (but in real game, the plan was 3 remakes and 3 soothsayers ...)
         * 
         * 
         * */        

        public static void Run()
        {
            GameConfig gameConfig = GameConfigBuilder.Create(
                StartingCardSplit.Split43,
                Cards.Butcher,
                Cards.GreatHall,
                Cards.HornOfPlenty,
                Cards.HorseTraders,
                Cards.Minion,
                Cards.Pawn,
                Cards.Remake,
                Cards.Soothsayer,
                Cards.StoneMason,
                Cards.Swindler
                );

            //Program.ComparePlayers(Strategies.HorseTraderSoothsayerMinionGreatHall.Player(1), Strategies.HorseTraderSoothsayerMinionGreatHall.Player(2, false), gameConfig);
            Program.ComparePlayers("HorseTraderSoothsayerMinionGreatHall", "BigMoney", gameConfig);
            Program.ComparePlayers("RemakeSoothsayer", "BigMoney", gameConfig);
            Program.ComparePlayers("RemakeSoothsayer", "HorseTraderSoothsayerMinionGreatHall", gameConfig);
        }
    }
}


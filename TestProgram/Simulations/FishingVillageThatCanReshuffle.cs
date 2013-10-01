using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Simulations.FishingVillageThatCanReshuffle
{
    static public class Simulation
    {
        public static void Run()
        {
            Program.ComparePlayers(Strategies.BigMoney.Player(1), Strategies.FishingVillage.Player(2));
            Program.ComparePlayers(Strategies.BigMoney.Player(1), Strategies.FishingVillageTest.Player(2));
            Program.ComparePlayers(Strategies.FishingVillageTest.Player(1), Strategies.FishingVillage.Player(2));                        
        }
    }
}
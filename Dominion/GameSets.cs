using Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public static class GameSets
    {        
        public static readonly GameConfig FirstGame = GameConfigBuilder.Create( Cellar.card, Market.card, Militia.card, Mine.card, Moat.card, Remodel.card, Smithy.card, Village.card, WoodCutter.card, Workshop.card);
        public static readonly GameConfig BigMoney = GameConfigBuilder.Create(Adventurer.card, Bureaucrat.card, Chancellor.card, Chapel.card, Feast.card, Laboratory.card, Market.card, Mine.card, Moneylender.card, ThroneRoom.card);
        public static readonly GameConfig Interaction = GameConfigBuilder.Create(Bureaucrat.card, Chancellor.card, CouncilRoom.card, Festival.card, Library.card, Militia.card, Moat.card, Spy.card, Thief.card, Village.card);
        public static readonly GameConfig SizeDistortion = GameConfigBuilder.Create(Cellar.card, Chapel.card, Feast.card, Gardens.card, Laboratory.card, Thief.card, Village.card, Witch.card, WoodCutter.card, Workshop.card);
        public static readonly GameConfig VillageSquare = GameConfigBuilder.Create(Bureaucrat.card, Cellar.card, Festival.card, Library.card, Market.card, Remodel.card, Smithy.card, ThroneRoom.card, Village.card, WoodCutter.card);
    }
}

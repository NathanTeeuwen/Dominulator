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
        public static readonly GameConfig FirstGame = GameConfigBuilder.Create( Card.Type<Cellar>(), Card.Type<Market>(), Card.Type<Militia>(), Card.Type<Mine>(), Card.Type<Moat>(), Card.Type<Remodel>(), Card.Type<Smithy>(), Card.Type<Village>(), Card.Type<WoodCutter>(), Card.Type<Workshop>());
        public static readonly GameConfig BigMoney = GameConfigBuilder.Create(Card.Type<Adventurer>(), Card.Type<Bureaucrat>(), Card.Type<Chancellor>(), Card.Type<Chapel>(), Card.Type<Feast>(), Card.Type<Laboratory>(), Card.Type<Market>(), Card.Type<Mine>(), Card.Type<Moneylender>(), Card.Type<ThroneRoom>());
        public static readonly GameConfig Interaction = GameConfigBuilder.Create(Card.Type<Bureaucrat>(), Card.Type<Chancellor>(), Card.Type<CouncilRoom>(), Card.Type<Festival>(), Card.Type<Library>(), Card.Type<Militia>(), Card.Type<Moat>(), Card.Type<Spy>(), Card.Type<Thief>(), Card.Type<Village>());
        public static readonly GameConfig SizeDistortion = GameConfigBuilder.Create(Card.Type<Cellar>(), Card.Type<Chapel>(), Card.Type<Feast>(), Card.Type<Gardens>(), Card.Type<Laboratory>(), Card.Type<Thief>(), Card.Type<Village>(), Card.Type<Witch>(), Card.Type<WoodCutter>(), Card.Type<Workshop>());
        public static readonly GameConfig VillageSquare = GameConfigBuilder.Create(Card.Type<Bureaucrat>(), Card.Type<Cellar>(), Card.Type<Festival>(), Card.Type<Library>(), Card.Type<Market>(), Card.Type<Remodel>(), Card.Type<Smithy>(), Card.Type<ThroneRoom>(), Card.Type<Village>(), Card.Type<WoodCutter>());
    }
}

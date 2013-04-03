using Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    static class GameSets
    {        
        static readonly GameConfig FirstGame = new GameConfig( new Cellar(), new Market(), new Militia(), new Mine(), new Moat(), new Remodel(), new Smithy(), new Village(), new WoodCutter(), new Workshop());
        static readonly GameConfig BigMoney = new GameConfig( new Adventurer(), new Bureaucrat(), new Chancellor(), new Chapel(), new Feast(), new Laboratory(), new Market(), new Mine(), new Moneylender(), new ThroneRoom());
        static readonly GameConfig Interaction = new GameConfig( new Bureaucrat(), new Chancellor(), new CouncilRoom(), new Festival(), new Library(), new Militia(), new Moat(), new Spy(), new Thief(), new Village());
        static readonly GameConfig SizeDistortion = new GameConfig( new Cellar(), new Chapel(), new Feast(), new Gardens(), new Laboratory(), new Thief(), new Village(), new Witch(), new WoodCutter(), new Workshop());
        static readonly GameConfig VillageSquare = new GameConfig( new Bureaucrat(), new Cellar(), new Festival(), new Library(), new Market(), new Remodel(), new Smithy(), new ThroneRoom(), new Village(), new WoodCutter());
    }
}

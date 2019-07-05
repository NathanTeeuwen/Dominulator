using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class ActingTroupe
       : Card
    {
        public static ActingTroupe card = new ActingTroupe();

        private ActingTroupe()
            : base("Acting Troupe", Expansion.Renaissance, coinCost: 3, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class BorderGuard
       : Card
    {
        public static BorderGuard card = new BorderGuard();

        private BorderGuard()
            : base("Border Guard", Expansion.Renaissance, coinCost: 2, isAction: true, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Horn
       : Artifact
    {
        public static Horn card = new Horn();

        private Horn()
            : base("Horn ", Expansion.Renaissance)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Lantern
       : Artifact
    {
        public static Lantern card = new Lantern();

        private Lantern()
            : base("Lantern", Expansion.Renaissance)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class CargoShip
      : Card
    {
        public static CargoShip card = new CargoShip();

        private CargoShip()
            : base("Cargo Ship", Expansion.Renaissance, coinCost: 3, isAction: true, isDuration: true, plusCoins: 2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Ducat
       : Card
    {
        public static Ducat card = new Ducat();

        private Ducat()
            : base("Ducat", Expansion.Renaissance, coinCost: 2, isTreasure: true, plusBuy:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.AddCoinTokens(1);
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerTrashCardFromHand(gameState, card => card == Cards.Copper, isOptional: true);
            return base.DoSpecializedWhenGain(currentPlayer, gameState);
        }
    }

    public class Experiment
       : Card
    {
        public static Experiment card = new Experiment();

        private Experiment()
            : base("Experiment", Expansion.Renaissance, coinCost: 3, isAction: true, plusActions:1, plusCards:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }
    
    public class FlagBearer
       : Card
    {
        public static FlagBearer card = new FlagBearer();

        private FlagBearer()
            : base("Flag Bearer", Expansion.Renaissance, coinCost: 4, isAction: true, plusCoins:2)
        {
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Flag
       : Artifact
    {
        public static Flag card = new Flag();

        private Flag()
            : base("Flag", Expansion.Renaissance)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Hideout
       : Card
    {
        public static Hideout card = new Hideout();

        private Hideout()
            : base("Hideout", Expansion.Renaissance, coinCost: 4, isAction: true, plusCards:1, plusActions:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Improve
       : Card
    {
        public static Improve card = new Improve();

        private Improve()
            : base("Improve", Expansion.Renaissance, coinCost: 4, isAction: true, plusCoins:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Inventor
       : Card
    {
        public static Inventor card = new Inventor();

        private Inventor()
            : base("Inventor", Expansion.Renaissance, coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Lackeys
       : Card
    {
        public static Lackeys card = new Lackeys();

        private Lackeys()
            : base("Lackeys", Expansion.Renaissance, coinCost: 2, isAction: true, plusCards:2)
        {
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class MountainVillage
       : Card
    {
        public static MountainVillage card = new MountainVillage();

        private MountainVillage()
            : base("Mountain Village", Expansion.Renaissance, coinCost: 4, isAction: true, plusActions:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class OldWitch
       : Card
    {
        public static OldWitch card = new OldWitch();

        private OldWitch()
            : base("Old Witch", Expansion.Renaissance, coinCost: 5, isAction: true, isAttack: true, plusCards:3)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Patron
       : Card
    {
        public static Patron card = new Patron();

        private Patron()
            : base("Patron", Expansion.Renaissance, coinCost: 4, isAction: true, plusCoins: 2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Priest
       : Card
    {
        public static Priest card = new Priest();

        private Priest()
            : base("Priest", Expansion.Renaissance, coinCost: 4, isAction: true, plusCoins: 2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Recruiter
       : Card
    {
        public static Recruiter card = new Recruiter();

        private Recruiter()
            : base("Recruiter", Expansion.Renaissance, coinCost: 5, isAction: true, plusCards:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Research
       : Card
    {
        public static Research card = new Research();

        private Research()
            : base("Research", Expansion.Renaissance, coinCost: 5, isAction: true, plusActions:1, isDuration:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Scepter
       : Card
    {
        public static Scepter card = new Scepter();

        private Scepter()
            : base("Scepter", Expansion.Renaissance, coinCost: 5, isTreasure: false)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Scholar
       : Card
    {
        public static Scholar card = new Scholar();

        private Scholar()
            : base("Scholar", Expansion.Renaissance, coinCost: 5, isAction:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Sculptor
       : Card
    {
        public static Sculptor card = new Sculptor();

        private Sculptor()
            : base("Sculptor", Expansion.Renaissance, coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Seer
       : Card
    {
        public static Seer card = new Seer();

        private Seer()
            : base("Seer", Expansion.Renaissance, coinCost: 5, isAction:true, plusCards:1, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class SilkMerchant
       : Card
    {
        public static SilkMerchant card = new SilkMerchant();

        private SilkMerchant()
            : base("Silk Merchant", Expansion.Renaissance, coinCost: 4, isAction:true, plusCards:2, plusBuy:1)
        {
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }

        public override bool DoSpecializedTrash(PlayerState selfPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Spices
       : Card
    {
        public static Spices card = new Spices();

        private Spices()
            : base("Spices", Expansion.Renaissance, coinCost: 5, isTreasure:true, plusCoins: 2, plusBuy: 1)
        {
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Swashbuckler
       : Card
    {
        public static Swashbuckler card = new Swashbuckler();

        private Swashbuckler()
            : base("Swashbuckler", Expansion.Renaissance, coinCost: 5, isAction:true, plusCoins: 2, plusBuy: 1)
        {
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class TreasureChest
        : Artifact
    {
        public static TreasureChest card = new TreasureChest();

        private TreasureChest()
            : base("Treasure Chest", Expansion.Renaissance)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Treasurer
       : Card
    {
        public static Treasurer card = new Treasurer();

        private Treasurer()
            : base("Treasurer", Expansion.Renaissance, coinCost: 5, isAction: true, plusCoins: 3)
        {
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Key
        : Artifact
    {
        public static Key card = new Key();

        private Key()
            : base("Key", Expansion.Renaissance)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Villain
       : Card
    {
        public static Villain card = new Villain();

        private Villain()
            : base("Villain", Expansion.Renaissance, coinCost: 5, isAction: true)
        {
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Academy
        : Project
    {
        public static Academy card = new Academy();

        private Academy()
            : base("Academy", Expansion.Renaissance, cost:5)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Barracks
        : Project
    {
        public static Barracks card = new Barracks();

        private Barracks()
            : base("Barracks", Expansion.Renaissance, cost: 6)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Canal
        : Project
    {
        public static Canal card = new Canal();

        private Canal()
            : base("Canal", Expansion.Renaissance, cost: 7)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Capitalism
        : Project
    {
        public static Capitalism card = new Capitalism();

        private Capitalism()
            : base("Capitalism", Expansion.Renaissance, cost: 5)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Cathedral
        : Project
    {
        public static Cathedral card = new Cathedral();

        private Cathedral()
            : base("Cathedral", Expansion.Renaissance, cost: 3)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }
    public class Citadel
        : Project
    {
        public static Citadel card = new Citadel();

        private Citadel()
            : base("Citadel", Expansion.Renaissance, cost: 8)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class CityGate
        : Project
    {
        public static CityGate card = new CityGate();

        private CityGate()
            : base("City Gate", Expansion.Renaissance, cost: 3)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class CropRotation
        : Project
    {
        public static CropRotation card = new CropRotation();

        private CropRotation()
            : base("Crop Rotation", Expansion.Renaissance, cost: 6)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Exploration
        : Project
    {
        public static Exploration card = new Exploration();

        private Exploration()
            : base("Exploration", Expansion.Renaissance, cost: 4)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Fair
        : Project
    {
        public static Fair card = new Fair();

        private Fair()
            : base("Fair", Expansion.Renaissance, cost: 4)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Fleet
        : Project
    {
        public static Fleet card = new Fleet();

        private Fleet()
            : base("Fleet", Expansion.Renaissance, cost: 5)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class GuildHall
        : Project
    {
        public static GuildHall card = new GuildHall();

        private GuildHall()
            : base("Guild Hall", Expansion.Renaissance, cost: 5)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Innovation
        : Project
    {
        public static Innovation card = new Innovation();

        private Innovation()
            : base("Innovation", Expansion.Renaissance, cost: 6)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Pageant
        : Project
    {
        public static Pageant card = new Pageant();

        private Pageant()
            : base("Pageant", Expansion.Renaissance, cost: 3)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Piazza
        : Project
    {
        public static Piazza card = new Piazza();

        private Piazza()
            : base("Piazza", Expansion.Renaissance, cost: 5)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class RoadNetwork
        : Project
    {
        public static RoadNetwork card = new RoadNetwork();

        private RoadNetwork()
            : base("Road Network", Expansion.Renaissance, cost: 5)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Sewers
        : Project
    {
        public static Sewers card = new Sewers();

        private Sewers()
            : base("Sewers", Expansion.Renaissance, cost: 3)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Silos
        : Project
    {
        public static Silos card = new Silos();

        private Silos()
            : base("Silos", Expansion.Renaissance, cost: 4)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class SinisterPlot
        : Project
    {
        public static SinisterPlot card = new SinisterPlot();

        private SinisterPlot()
            : base("Sinister Plot", Expansion.Renaissance, cost: 4)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class StarChart
        : Project
    {
        public static StarChart card = new StarChart();

        private StarChart()
            : base("Star Chart", Expansion.Renaissance, cost: 3)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }
}
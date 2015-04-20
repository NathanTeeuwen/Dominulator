using System;
using System.Linq;
using System.Collections.Generic;

namespace Dominion
{
    // All of the Card Instances.
    public static class Cards
    {
        public static readonly CardTypes.AbandonedMine AbandonedMine = CardTypes.AbandonedMine.card;
        public static readonly CardTypes.Adventurer Adventurer = CardTypes.Adventurer.card;
        public static readonly CardTypes.Advisor Advisor = CardTypes.Advisor.card;
        public static readonly CardTypes.Alchemist Alchemist = CardTypes.Alchemist.card;
        public static readonly CardTypes.Altar Altar = CardTypes.Altar.card;
        public static readonly CardTypes.Ambassador Ambassador = CardTypes.Ambassador.card;
        public static readonly CardTypes.Amulet Amulet = CardTypes.Amulet.card;
        public static readonly CardTypes.Apothecary Apothecary = CardTypes.Apothecary.card;
        public static readonly CardTypes.Apprentice Apprentice = CardTypes.Apprentice.card;
        public static readonly CardTypes.Armory Armory = CardTypes.Armory.card;
        public static readonly CardTypes.Artificer Artificer = CardTypes.Artificer.card;
        public static readonly CardTypes.BagOfGold BagOfGold = CardTypes.BagOfGold.card;
        public static readonly CardTypes.Baker Baker = CardTypes.Baker.card;
        public static readonly CardTypes.BanditCamp BanditCamp = CardTypes.BanditCamp.card;
        public static readonly CardTypes.BandOfMisfits BandOfMisfits = CardTypes.BandOfMisfits.card;
        public static readonly CardTypes.Bank Bank = CardTypes.Bank.card;
        public static readonly CardTypes.Baron Baron = CardTypes.Baron.card;
        public static readonly CardTypes.Bazaar Bazaar = CardTypes.Bazaar.card;
        public static readonly CardTypes.Beggar Beggar = CardTypes.Beggar.card;
        public static readonly CardTypes.Bishop Bishop = CardTypes.Bishop.card;
        public static readonly CardTypes.BlackMarket BlackMarket = CardTypes.BlackMarket.card;
        public static readonly CardTypes.BorderVillage BorderVillage = CardTypes.BorderVillage.card;
        public static readonly CardTypes.Bridge Bridge = CardTypes.Bridge.card;
        public static readonly CardTypes.BridgeTroll BridgeTroll = CardTypes.BridgeTroll.card;
        public static readonly CardTypes.Bureaucrat Bureaucrat = CardTypes.Bureaucrat.card;
        public static readonly CardTypes.Butcher Butcher = CardTypes.Butcher.card;
        public static readonly CardTypes.Cache Cache = CardTypes.Cache.card;
        public static readonly CardTypes.CandlestickMaker CandlestickMaker = CardTypes.CandlestickMaker.card;
        public static readonly CardTypes.Caravan Caravan = CardTypes.Caravan.card;
        public static readonly CardTypes.CaravanGuard CaravanGuard = CardTypes.CaravanGuard.card;
        public static readonly CardTypes.Cartographer Cartographer = CardTypes.Cartographer.card;
        public static readonly CardTypes.Catacombs Catacombs = CardTypes.Catacombs.card;
        public static readonly CardTypes.Cellar Cellar = CardTypes.Cellar.card;
        public static readonly CardTypes.Champion Champion = CardTypes.Champion.card;
        public static readonly CardTypes.Chancellor Chancellor = CardTypes.Chancellor.card;
        public static readonly CardTypes.Chapel Chapel = CardTypes.Chapel.card;
        public static readonly CardTypes.City City = CardTypes.City.card;
        public static readonly CardTypes.Colony Colony = CardTypes.Colony.card;
        public static readonly CardTypes.CoinOfTheRealm CoinOfTheRealm = CardTypes.CoinOfTheRealm.card;
        public static readonly CardTypes.Conspirator Conspirator = CardTypes.Conspirator.card;
        public static readonly CardTypes.Contraband Contraband = CardTypes.Contraband.card;
        public static readonly CardTypes.Copper Copper = CardTypes.Copper.card;
        public static readonly CardTypes.Coppersmith Coppersmith = CardTypes.Coppersmith.card;
        public static readonly CardTypes.CouncilRoom CouncilRoom = CardTypes.CouncilRoom.card;
        public static readonly CardTypes.Count Count = CardTypes.Count.card;
        public static readonly CardTypes.CounterFeit CounterFeit = CardTypes.CounterFeit.card;
        public static readonly CardTypes.CountingHouse CountingHouse = CardTypes.CountingHouse.card;
        public static readonly CardTypes.Courtyard Courtyard = CardTypes.Courtyard.card;
        public static readonly CardTypes.CrossRoads CrossRoads = CardTypes.CrossRoads.card;
        public static readonly CardTypes.Cultist Cultist = CardTypes.Cultist.card;
        public static readonly CardTypes.Curse Curse = CardTypes.Curse.card;
        public static readonly CardTypes.Cutpurse Cutpurse = CardTypes.Cutpurse.card;
        public static readonly CardTypes.DeathCart DeathCart = CardTypes.DeathCart.card;
        public static readonly CardTypes.Develop Develop = CardTypes.Develop.card;
        public static readonly CardTypes.Diadem Diadem = CardTypes.Diadem.card;
        public static readonly CardTypes.Disciple Disciple = CardTypes.Disciple.card;
        public static readonly CardTypes.DistantLands DistantLands = CardTypes.DistantLands.card;        
        public static readonly CardTypes.Doctor Doctor = CardTypes.Doctor.card;
        public static readonly CardTypes.Duchess Duchess = CardTypes.Duchess.card;
        public static readonly CardTypes.Duchy Duchy = CardTypes.Duchy.card;
        public static readonly CardTypes.Duke Duke = CardTypes.Duke.card;
        public static readonly CardTypes.Dungeon Dungeon = CardTypes.Dungeon.card;
        public static readonly CardTypes.Duplicate Duplicate = CardTypes.Duplicate.card;
        public static readonly CardTypes.Embargo Embargo = CardTypes.Embargo.card;
        public static readonly CardTypes.Embassy Embassy = CardTypes.Embassy.card;
        public static readonly CardTypes.Envoy Envoy = CardTypes.Envoy.card;
        public static readonly CardTypes.Estate Estate = CardTypes.Estate.card;
        public static readonly CardTypes.Expand Expand = CardTypes.Expand.card;
        public static readonly CardTypes.Explorer Explorer = CardTypes.Explorer.card;
        public static readonly CardTypes.Fairgrounds Fairgrounds = CardTypes.Fairgrounds.card;
        public static readonly CardTypes.Familiar Familiar = CardTypes.Familiar.card;
        public static readonly CardTypes.FarmingVillage FarmingVillage = CardTypes.FarmingVillage.card;
        public static readonly CardTypes.Farmland Farmland = CardTypes.Farmland.card;
        public static readonly CardTypes.Feast Feast = CardTypes.Feast.card;
        public static readonly CardTypes.Feodum Feodum = CardTypes.Feodum.card;
        public static readonly CardTypes.Festival Festival = CardTypes.Festival.card;
        public static readonly CardTypes.FishingVillage FishingVillage = CardTypes.FishingVillage.card;
        public static readonly CardTypes.Followers Followers = CardTypes.Followers.card;
        public static readonly CardTypes.FoolsGold FoolsGold = CardTypes.FoolsGold.card;
        public static readonly CardTypes.Forager Forager = CardTypes.Forager.card;
        public static readonly CardTypes.Forge Forge = CardTypes.Forge.card;
        public static readonly CardTypes.Fortress Fortress = CardTypes.Fortress.card;
        public static readonly CardTypes.FortuneTeller FortuneTeller = CardTypes.FortuneTeller.card;
        public static readonly CardTypes.Fugitive Fugitive = CardTypes.Fugitive.card;
        public static readonly CardTypes.Gardens Gardens = CardTypes.Gardens.card;
        public static readonly CardTypes.Gear Gear = CardTypes.Gear.card;
        public static readonly CardTypes.GhostShip GhostShip = CardTypes.GhostShip.card;
        public static readonly CardTypes.Giant Giant = CardTypes.Giant.card;
        public static readonly CardTypes.Gold Gold = CardTypes.Gold.card;
        public static readonly CardTypes.Golem Golem = CardTypes.Golem.card;
        public static readonly CardTypes.Goons Goons = CardTypes.Goons.card;
        public static readonly CardTypes.Governor Governor = CardTypes.Governor.card;
        public static readonly CardTypes.Guide Guide = CardTypes.Guide.card;
        public static readonly CardTypes.GrandMarket GrandMarket = CardTypes.GrandMarket.card;
        public static readonly CardTypes.Graverobber Graverobber = CardTypes.Graverobber.card;
        public static readonly CardTypes.GreatHall GreatHall = CardTypes.GreatHall.card;
        public static readonly CardTypes.Haggler Haggler = CardTypes.Haggler.card;
        public static readonly CardTypes.Hamlet Hamlet = CardTypes.Hamlet.card;
        public static readonly CardTypes.Harem Harem = CardTypes.Harem.card;
        public static readonly CardTypes.Harvest Harvest = CardTypes.Harvest.card;
        public static readonly CardTypes.HauntedWoods HauntedWoods = CardTypes.HauntedWoods.card;
        public static readonly CardTypes.Haven Haven = CardTypes.Haven.card;
        public static readonly CardTypes.Herald Herald = CardTypes.Herald.card;
        public static readonly CardTypes.Hero Hero = CardTypes.Hero.card;
        public static readonly CardTypes.Herbalist Herbalist = CardTypes.Herbalist.card;
        public static readonly CardTypes.Hermit Hermit = CardTypes.Hermit.card;
        public static readonly CardTypes.Highway Highway = CardTypes.Highway.card;
        public static readonly CardTypes.Hireling Hireling = CardTypes.Hireling.card;
        public static readonly CardTypes.Hoard Hoard = CardTypes.Hoard.card;
        public static readonly CardTypes.HornOfPlenty HornOfPlenty = CardTypes.HornOfPlenty.card;
        public static readonly CardTypes.HorseTraders HorseTraders = CardTypes.HorseTraders.card;
        public static readonly CardTypes.Hovel Hovel = CardTypes.Hovel.card;
        public static readonly CardTypes.HuntingGrounds HuntingGrounds = CardTypes.HuntingGrounds.card;
        public static readonly CardTypes.HuntingParty HuntingParty = CardTypes.HuntingParty.card;
        public static readonly CardTypes.IllGottenGains IllGottenGains = CardTypes.IllGottenGains.card;
        public static readonly CardTypes.Inn Inn = CardTypes.Inn.card;
        public static readonly CardTypes.IronMonger IronMonger = CardTypes.IronMonger.card;
        public static readonly CardTypes.IronWorks IronWorks = CardTypes.IronWorks.card;
        public static readonly CardTypes.Island Island = CardTypes.Island.card;
        public static readonly CardTypes.JackOfAllTrades JackOfAllTrades = CardTypes.JackOfAllTrades.card;
        public static readonly CardTypes.Jester Jester = CardTypes.Jester.card;
        public static readonly CardTypes.Journeyman Journeyman = CardTypes.Journeyman.card;
        public static readonly CardTypes.JunkDealer JunkDealer = CardTypes.JunkDealer.card;
        public static readonly CardTypes.KingsCourt KingsCourt = CardTypes.KingsCourt.card;
        public static readonly CardTypes.Knights Knights = CardTypes.Knights.card;
        public static readonly CardTypes.Laboratory Laboratory = CardTypes.Laboratory.card;
        public static readonly CardTypes.Library Library = CardTypes.Library.card;
        public static readonly CardTypes.Lighthouse Lighthouse = CardTypes.Lighthouse.card;
        public static readonly CardTypes.Loan Loan = CardTypes.Loan.card;
        public static readonly CardTypes.Lookout Lookout = CardTypes.Lookout.card;
        public static readonly CardTypes.LostCity LostCity = CardTypes.LostCity.card;
        public static readonly CardTypes.Madman Madman = CardTypes.Madman.card;
        public static readonly CardTypes.Mandarin Mandarin = CardTypes.Mandarin.card;
        public static readonly CardTypes.Magpie Magpie = CardTypes.Magpie.card;
        public static readonly CardTypes.Marauder Marauder = CardTypes.Marauder.card;
        public static readonly CardTypes.Margrave Margrave = CardTypes.Margrave.card;
        public static readonly CardTypes.Market Market = CardTypes.Market.card;
        public static readonly CardTypes.MarketSquare MarketSquare = CardTypes.MarketSquare.card;
        public static readonly CardTypes.Masquerade Masquerade = CardTypes.Masquerade.card;
        public static readonly CardTypes.Masterpiece Masterpiece = CardTypes.Masterpiece.card;
        public static readonly CardTypes.Menagerie Menagerie = CardTypes.Menagerie.card;
        public static readonly CardTypes.Mercenary Mercenary = CardTypes.Mercenary.card;
        public static readonly CardTypes.MerchantGuild MerchantGuild = CardTypes.MerchantGuild.card;
        public static readonly CardTypes.MerchantShip MerchantShip = CardTypes.MerchantShip.card;
        public static readonly CardTypes.Messenger Messenger = CardTypes.Messenger.card;
        public static readonly CardTypes.Militia Militia = CardTypes.Militia.card;
        public static readonly CardTypes.Mine Mine = CardTypes.Mine.card;
        public static readonly CardTypes.MiningVillage MiningVillage = CardTypes.MiningVillage.card;
        public static readonly CardTypes.Minion Minion = CardTypes.Minion.card;
        public static readonly CardTypes.Mint Mint = CardTypes.Mint.card;
        public static readonly CardTypes.Miser Miser = CardTypes.Miser.card;
        public static readonly CardTypes.Moat Moat = CardTypes.Moat.card;
        public static readonly CardTypes.Moneylender Moneylender = CardTypes.Moneylender.card;
        public static readonly CardTypes.Monument Monument = CardTypes.Monument.card;
        public static readonly CardTypes.Mountebank Mountebank = CardTypes.Mountebank.card;
        public static readonly CardTypes.Mystic Mystic = CardTypes.Mystic.card;
        public static readonly CardTypes.NativeVillage NativeVillage = CardTypes.NativeVillage.card;
        public static readonly CardTypes.Navigator Navigator = CardTypes.Navigator.card;
        public static readonly CardTypes.Necropolis Necropolis = CardTypes.Necropolis.card;
        public static readonly CardTypes.NobleBrigand NobleBrigand = CardTypes.NobleBrigand.card;
        public static readonly CardTypes.Nobles Nobles = CardTypes.Nobles.card;
        public static readonly CardTypes.NomadCamp NomadCamp = CardTypes.NomadCamp.card;
        public static readonly CardTypes.Oasis Oasis = CardTypes.Oasis.card;
        public static readonly CardTypes.Oracle Oracle = CardTypes.Oracle.card;
        public static readonly CardTypes.Outpost Outpost = CardTypes.Outpost.card;
        public static readonly CardTypes.OvergrownEstate OvergrownEstate = CardTypes.OvergrownEstate.card;
        public static readonly CardTypes.Pawn Pawn = CardTypes.Pawn.card;
        public static readonly CardTypes.Page Page = CardTypes.Page.card;
        public static readonly CardTypes.Peasant Peasant = CardTypes.Peasant.card;
        public static readonly CardTypes.PearlDiver PearlDiver = CardTypes.PearlDiver.card;
        public static readonly CardTypes.Peddler Peddler = CardTypes.Peddler.card;
        public static readonly CardTypes.PhilosophersStone PhilosophersStone = CardTypes.PhilosophersStone.card;
        public static readonly CardTypes.Pillage Pillage = CardTypes.Pillage.card;
        public static readonly CardTypes.PirateShip PirateShip = CardTypes.PirateShip.card;
        public static readonly CardTypes.Platinum Platinum = CardTypes.Platinum.card;
        public static readonly CardTypes.Plaza Plaza = CardTypes.Plaza.card;
        public static readonly CardTypes.PoorHouse PoorHouse = CardTypes.PoorHouse.card;
        public static readonly CardTypes.Port Port = CardTypes.Port.card;
        public static readonly CardTypes.Possession Possession = CardTypes.Possession.card;
        public static readonly CardTypes.Potion Potion = CardTypes.Potion.card;
        public static readonly CardTypes.Princess Princess = CardTypes.Princess.card;
        public static readonly CardTypes.Prince Prince = CardTypes.Prince.card;
        public static readonly CardTypes.Prize Prize = CardTypes.Prize.card;
        public static readonly CardTypes.Procession Procession = CardTypes.Procession.card;
        public static readonly CardTypes.Province Province = CardTypes.Province.card;
        public static readonly CardTypes.Quarry Quarry = CardTypes.Quarry.card;
        public static readonly CardTypes.Rabble Rabble = CardTypes.Rabble.card;
        public static readonly CardTypes.Ranger Ranger = CardTypes.Ranger.card;
        public static readonly CardTypes.Rats Rats = CardTypes.Rats.card;
        public static readonly CardTypes.RatCatcher RatCatcher = CardTypes.RatCatcher.card;
        public static readonly CardTypes.Raze Raze = CardTypes.Raze.card;
        public static readonly CardTypes.Rebuild Rebuild = CardTypes.Rebuild.card;
        public static readonly CardTypes.Relic Relic = CardTypes.Relic.card;
        public static readonly CardTypes.Remake Remake = CardTypes.Remake.card;
        public static readonly CardTypes.Remodel Remodel = CardTypes.Remodel.card;
        public static readonly CardTypes.Rogue Rogue = CardTypes.Rogue.card;
        public static readonly CardTypes.RoyalSeal RoyalSeal = CardTypes.RoyalSeal.card;
        public static readonly CardTypes.RoyalCarriage RoyalCarriage = CardTypes.RoyalCarriage.card;
        public static readonly CardTypes.RuinedLibrary RuinedLibrary = CardTypes.RuinedLibrary.card;
        public static readonly CardTypes.RuinedMarket RuinedMarket = CardTypes.RuinedMarket.card;
        public static readonly CardTypes.RuinedVillage RuinedVillage = CardTypes.RuinedVillage.card;
        public static readonly CardTypes.Ruins Ruins = CardTypes.Ruins.card;
        public static readonly CardTypes.Saboteur Saboteur = CardTypes.Saboteur.card;
        public static readonly CardTypes.Sage Sage = CardTypes.Sage.card;
        public static readonly CardTypes.Salvager Salvager = CardTypes.Salvager.card;
        public static readonly CardTypes.Scavenger Scavenger = CardTypes.Scavenger.card;
        public static readonly CardTypes.Scheme Scheme = CardTypes.Scheme.card;
        public static readonly CardTypes.Scout Scout = CardTypes.Scout.card;
        public static readonly CardTypes.ScryingPool ScryingPool = CardTypes.ScryingPool.card;
        public static readonly CardTypes.SeaHag SeaHag = CardTypes.SeaHag.card;
        public static readonly CardTypes.SecretChamber SecretChamber = CardTypes.SecretChamber.card;
        public static readonly CardTypes.ShantyTown ShantyTown = CardTypes.ShantyTown.card;
        public static readonly CardTypes.SilkRoad SilkRoad = CardTypes.SilkRoad.card;
        public static readonly CardTypes.Silver Silver = CardTypes.Silver.card;
        public static readonly CardTypes.Smithy Smithy = CardTypes.Smithy.card;
        public static readonly CardTypes.Smugglers Smugglers = CardTypes.Smugglers.card;
        public static readonly CardTypes.Solider Solider = CardTypes.Solider.card;
        public static readonly CardTypes.Soothsayer Soothsayer = CardTypes.Soothsayer.card;
        public static readonly CardTypes.SpiceMerchant SpiceMerchant = CardTypes.SpiceMerchant.card;
        public static readonly CardTypes.Spoils Spoils = CardTypes.Spoils.card;
        public static readonly CardTypes.Spy Spy = CardTypes.Spy.card;
        public static readonly CardTypes.Squire Squire = CardTypes.Squire.card;
        public static readonly CardTypes.Stables Stables = CardTypes.Stables.card;
        public static readonly CardTypes.Stash Stash = CardTypes.Stash.card;
        public static readonly CardTypes.Steward Steward = CardTypes.Steward.card;
        public static readonly CardTypes.StoneMason StoneMason = CardTypes.StoneMason.card;
        public static readonly CardTypes.Storeroom Storeroom = CardTypes.Storeroom.card;
        public static readonly CardTypes.Storyteller Storyteller = CardTypes.Storyteller.card;
        public static readonly CardTypes.SwampHag SwampHag = CardTypes.SwampHag.card;
        public static readonly CardTypes.Survivors Survivors = CardTypes.Survivors.card;
        public static readonly CardTypes.Swindler Swindler = CardTypes.Swindler.card;
        public static readonly CardTypes.Tactician Tactician = CardTypes.Tactician.card;
        public static readonly CardTypes.Talisman Talisman = CardTypes.Talisman.card;
        public static readonly CardTypes.Taxman Taxman = CardTypes.Taxman.card;
        public static readonly CardTypes.Teacher Teacher = CardTypes.Teacher.card;
        public static readonly CardTypes.Thief Thief = CardTypes.Thief.card;
        public static readonly CardTypes.ThroneRoom ThroneRoom = CardTypes.ThroneRoom.card;
        public static readonly CardTypes.Torturer Torturer = CardTypes.Torturer.card;
        public static readonly CardTypes.Tournament Tournament = CardTypes.Tournament.card;
        public static readonly CardTypes.Trader Trader = CardTypes.Trader.card;
        public static readonly CardTypes.TradeRoute TradeRoute = CardTypes.TradeRoute.card;
        public static readonly CardTypes.TradingPost TradingPost = CardTypes.TradingPost.card;
        public static readonly CardTypes.Transmute Transmute = CardTypes.Transmute.card;
        public static readonly CardTypes.Transmogrify Transmogrify = CardTypes.Transmogrify.card;
        public static readonly CardTypes.TreasureMap TreasureMap = CardTypes.TreasureMap.card;
        public static readonly CardTypes.TreasureHunter TreasureHunter = CardTypes.TreasureHunter.card;
        public static readonly CardTypes.TreasureTrove TreasureTrove = CardTypes.TreasureTrove.card;        
        public static readonly CardTypes.Treasury Treasury = CardTypes.Treasury.card;
        public static readonly CardTypes.Tribute Tribute = CardTypes.Tribute.card;
        public static readonly CardTypes.TrustySteed TrustySteed = CardTypes.TrustySteed.card;
        public static readonly CardTypes.Tunnel Tunnel = CardTypes.Tunnel.card;
        public static readonly CardTypes.University University = CardTypes.University.card;
        public static readonly CardTypes.Upgrade Upgrade = CardTypes.Upgrade.card;
        public static readonly CardTypes.Urchin Urchin = CardTypes.Urchin.card;
        public static readonly CardTypes.Vagrant Vagrant = CardTypes.Vagrant.card;
        public static readonly CardTypes.Vault Vault = CardTypes.Vault.card;
        public static readonly CardTypes.Venture Venture = CardTypes.Venture.card;
        public static readonly CardTypes.Village Village = CardTypes.Village.card;
        public static readonly CardTypes.Vineyard Vineyard = CardTypes.Vineyard.card;
        public static readonly CardTypes.WalledVillage WalledVillage = CardTypes.WalledVillage.card;
        public static readonly CardTypes.WanderingMinstrel WanderingMinstrel = CardTypes.WanderingMinstrel.card;
        public static readonly CardTypes.Warehouse Warehouse = CardTypes.Warehouse.card;
        public static readonly CardTypes.Warrior Warrior = CardTypes.Warrior.card;        
        public static readonly CardTypes.Watchtower Watchtower = CardTypes.Watchtower.card;
        public static readonly CardTypes.Wharf Wharf = CardTypes.Wharf.card;
        public static readonly CardTypes.WineMerchant WineMerchant = CardTypes.WineMerchant.card;
        public static readonly CardTypes.WishingWell WishingWell = CardTypes.WishingWell.card;
        public static readonly CardTypes.Witch Witch = CardTypes.Witch.card;
        public static readonly CardTypes.WoodCutter WoodCutter = CardTypes.WoodCutter.card;
        public static readonly CardTypes.WorkersVillage WorkersVillage = CardTypes.WorkersVillage.card;
        public static readonly CardTypes.Workshop Workshop = CardTypes.Workshop.card;
        public static readonly CardTypes.YoungWitch YoungWitch = CardTypes.YoungWitch.card;

        // Events

        public static readonly CardTypes.Alms Alms = CardTypes.Alms.card;
        public static readonly CardTypes.Ball Ball = CardTypes.Ball.card;
        public static readonly CardTypes.Borrow Borrow = CardTypes.Borrow.card;
        public static readonly CardTypes.Bonfire Bonfire = CardTypes.Bonfire.card;
        public static readonly CardTypes.Expedition Expedition = CardTypes.Expedition.card;
        public static readonly CardTypes.Ferry Ferry = CardTypes.Ferry.card;
        public static readonly CardTypes.Inheritance Inheritance = CardTypes.Inheritance.card;
        public static readonly CardTypes.LostArts LostArts = CardTypes.LostArts.card;
        public static readonly CardTypes.Mission Mission = CardTypes.Mission.card;
        public static readonly CardTypes.PathFinding PathFinding = CardTypes.PathFinding.card;
        public static readonly CardTypes.Pilgrimage Pilgrimage = CardTypes.Pilgrimage.card;
        public static readonly CardTypes.Plan Plan = CardTypes.Plan.card;
        public static readonly CardTypes.Quest Quest = CardTypes.Quest.card;
        public static readonly CardTypes.Raid Raid = CardTypes.Raid.card;
        public static readonly CardTypes.Save Save = CardTypes.Save.card;
        public static readonly CardTypes.ScoutingParty ScoutingParty = CardTypes.ScoutingParty.card;
        public static readonly CardTypes.Seaway Seaway = CardTypes.Seaway.card;
        public static readonly CardTypes.Trade Trade = CardTypes.Trade.card;
        public static readonly CardTypes.Training Training = CardTypes.Training.card;
        public static readonly CardTypes.TravellingFair TravellingFair = CardTypes.TravellingFair.card;

        /* 
        public static void PrintAllCards()
        {            
            foreach (System.Reflection.FieldInfo fieldInfo in typeof(Cards).GetFields())
            {
                System.Console.WriteLine("Cards.{0},", fieldInfo.Name);
            }            
        }
        
        public static Card[] AllCards()
        {
            var result = new List<Card>();
            foreach (System.Reflection.FieldInfo fieldInfo in typeof(Cards).GetFields())
            {                
                result.Add((Card)fieldInfo.GetValue(null));                
            }
            
            return result.ToArray();
        }

        */

        public static Card[] AllCards()
        {
            return AllCardsList;
        }

        public static Card[] Shelters = new Card[] {
            Cards.OvergrownEstate,
            Cards.Hovel,
            Cards.Necropolis};

        public static Card[] AllKingdomCards()
        {
            var result = new List<Card>();
            foreach (Card card in AllCards())
            {
                if (card.isKingdomCard)
                    result.Add(card);
            }

            return result.ToArray();
        }

        private static Card[] AllCardsList = new Card[]
        {
            Cards.AbandonedMine,
            Cards.Adventurer,
            Cards.Advisor,
            Cards.Alchemist,
            Cards.Altar,
            Cards.Ambassador,
            Cards.Apothecary,
            Cards.Apprentice,
            Cards.Armory,
            Cards.BagOfGold,
            Cards.Baker,
            Cards.BanditCamp,
            Cards.BandOfMisfits,
            Cards.Bank,
            Cards.Baron,
            Cards.Bazaar,
            Cards.Beggar,
            Cards.Bishop,
            Cards.BlackMarket,
            Cards.BorderVillage,
            Cards.Bridge,
            Cards.Bureaucrat,
            Cards.Butcher,
            Cards.Cache,
            Cards.CandlestickMaker,
            Cards.Caravan,
            Cards.Cartographer,
            Cards.Catacombs,
            Cards.Cellar,
            Cards.Chancellor,
            Cards.Chapel,
            Cards.City,
            Cards.Colony,
            Cards.Conspirator,
            Cards.Contraband,
            Cards.Copper,
            Cards.Coppersmith,
            Cards.CouncilRoom,
            Cards.Count,
            Cards.CounterFeit,
            Cards.CountingHouse,
            Cards.Courtyard,
            Cards.CrossRoads,
            Cards.Cultist,
            Cards.Curse,
            Cards.Cutpurse,
            Cards.DeathCart,
            Cards.Develop,
            Cards.Diadem,
            Cards.Doctor,
            Cards.Duchess,
            Cards.Duchy,
            Cards.Duke,
            Cards.Embargo,
            Cards.Embassy,
            Cards.Envoy,
            Cards.Estate,
            Cards.Expand,
            Cards.Explorer,
            Cards.Fairgrounds,
            Cards.Familiar,
            Cards.FarmingVillage,
            Cards.Farmland,
            Cards.Feast,
            Cards.Feodum,
            Cards.Festival,
            Cards.FishingVillage,
            Cards.Followers,
            Cards.FoolsGold,
            Cards.Forager,
            Cards.Forge,
            Cards.Fortress,
            Cards.FortuneTeller,
            Cards.Gardens,
            Cards.GhostShip,
            Cards.Gold,
            Cards.Golem,
            Cards.Goons,
            Cards.Governor,
            Cards.GrandMarket,
            Cards.Graverobber,
            Cards.GreatHall,
            Cards.Haggler,
            Cards.Hamlet,
            Cards.Harem,
            Cards.Harvest,
            Cards.Haven,
            Cards.Herald,
            Cards.Herbalist,
            Cards.Hermit,
            Cards.Highway,
            Cards.Hoard,
            Cards.HornOfPlenty,
            Cards.HorseTraders,
            Cards.Hovel,
            Cards.HuntingGrounds,
            Cards.HuntingParty,
            Cards.IllGottenGains,
            Cards.Inn,
            Cards.IronMonger,
            Cards.IronWorks,
            Cards.Island,
            Cards.JackOfAllTrades,
            Cards.Jester,
            Cards.JunkDealer,
            Cards.KingsCourt,
            Cards.Knights,
            Cards.Laboratory,
            Cards.Library,
            Cards.Lighthouse,
            Cards.Loan,
            Cards.Lookout,
            Cards.LostCity,
            Cards.Madman,
            Cards.Magpie,
            Cards.Mandarin,
            Cards.Marauder,
            Cards.Margrave,
            Cards.Market,
            Cards.MarketSquare,
            Cards.Masquerade,
            Cards.Masterpiece,
            Cards.Menagerie,            
            Cards.Mercenary,
            Cards.MerchantGuild,
            Cards.MerchantShip,
            Cards.Messenger,
            Cards.Militia,
            Cards.Mine,
            Cards.MiningVillage,
            Cards.Minion,
            Cards.Mint,
            Cards.Moat,
            Cards.Moneylender,
            Cards.Monument,
            Cards.Mountebank,
            Cards.Mystic,
            Cards.NativeVillage,
            Cards.Navigator,
            Cards.Necropolis,
            Cards.NobleBrigand,
            Cards.Nobles,
            Cards.NomadCamp,
            Cards.Oasis,
            Cards.Oracle,
            Cards.Outpost,
            Cards.OvergrownEstate,
            Cards.Pawn,
            Cards.PearlDiver,
            Cards.Peddler,
            Cards.PhilosophersStone,
            Cards.Pillage,
            Cards.PirateShip,
            Cards.Platinum,
            Cards.Plaza,
            Cards.PoorHouse,
            Cards.Possession,
            Cards.Potion,
            Cards.Princess,
            Cards.Prince,
            Cards.Prize,
            Cards.Procession,
            Cards.Province,
            Cards.Quarry,
            Cards.Rabble,
            Cards.Rats,
            Cards.Rebuild,
            Cards.Remake,
            Cards.Remodel,
            Cards.Rogue,
            Cards.RoyalSeal,
            Cards.RuinedLibrary,
            Cards.RuinedMarket,
            Cards.RuinedVillage,
            Cards.Ruins,
            Cards.Saboteur,
            Cards.Sage,
            Cards.Salvager,
            Cards.Scavenger,
            Cards.Scheme,
            Cards.Scout,
            Cards.ScryingPool,
            Cards.SeaHag,
            Cards.SecretChamber,
            Cards.ShantyTown,
            Cards.SilkRoad,
            Cards.Silver,
            Cards.Smithy,
            Cards.Smugglers,
            Cards.Soothsayer,
            Cards.SpiceMerchant,
            Cards.Spoils,
            Cards.Spy,
            Cards.Squire,
            Cards.Stables,
            Cards.Stash,
            Cards.Steward,
            Cards.StoneMason,
            Cards.Storeroom,
            Cards.Storyteller,
            Cards.Survivors,
            Cards.Swindler,
            Cards.Tactician,
            Cards.Talisman,
            Cards.Taxman,
            Cards.Thief,
            Cards.ThroneRoom,
            Cards.Torturer,
            Cards.Tournament,
            Cards.Trader,
            Cards.TradeRoute,
            Cards.TradingPost,
            Cards.Transmute,
            Cards.TreasureMap,
            Cards.Treasury,
            Cards.Tribute,
            Cards.TrustySteed,
            Cards.Tunnel,
            Cards.University,
            Cards.Upgrade,
            Cards.Urchin,
            Cards.Vagrant,
            Cards.Vault,
            Cards.Venture,
            Cards.Village,
            Cards.Vineyard,
            Cards.WalledVillage,
            Cards.WanderingMinstrel,
            Cards.Warehouse,
            Cards.Watchtower,
            Cards.Wharf,
            Cards.WishingWell,
            Cards.Witch,
            Cards.WoodCutter,
            Cards.WorkersVillage,
            Cards.Workshop,
            Cards.YoungWitch,

            // Adventures
            Cards.Amulet,
            Cards.Artificer,
            Cards.BridgeTroll,
            Cards.CaravanGuard,
            Cards.Champion,
            Cards.CoinOfTheRealm,
            Cards.Disciple,
            Cards.DistantLands,
            Cards.Duplicate,
            Cards.Dungeon,
            Cards.Fugitive,
            Cards.Gear,
            Cards.Giant,
            Cards.Guide,
            Cards.HauntedWoods,
            Cards.Hero,
            Cards.Hireling,            
            Cards.Miser,
            Cards.Page,
            Cards.Peasant,
            Cards.Port,
            Cards.Ranger,
            Cards.RatCatcher,
            Cards.Raze,
            Cards.Relic,
            Cards.RoyalCarriage,
            Cards.Solider,            
            Cards.SwampHag,
            Cards.Teacher,
            Cards.Transmogrify,
            Cards.TreasureHunter,
            Cards.TreasureTrove,
            Cards.Warrior,
            Cards.WineMerchant,

            // events

            Cards.Alms,
            Cards.Ball,
            Cards.Borrow,
            Cards.Bonfire,
            Cards.Expedition,
            Cards.Ferry,
            Cards.Inheritance,
            Cards.LostArts,
            Cards.Mission,
            Cards.PathFinding,
            Cards.Pilgrimage,
            Cards.Plan,
            Cards.Quest,
            Cards.Raid,
            Cards.Save,
            Cards.ScoutingParty,
            Cards.Seaway,
            Cards.Trade,
            Cards.Training,
            Cards.TravellingFair
        };
  
        public static Card[] UnimplementedCards = new Card[]
        {          
            Cards.Knights,
            Cards.BandOfMisfits,
            Cards.BlackMarket,
            
            Cards.Possession,
            Cards.Prince,

            // Adventures
            Cards.Amulet,
            Cards.Artificer,
            Cards.BridgeTroll,
            Cards.CaravanGuard,
            Cards.Champion,
            Cards.CoinOfTheRealm,
            Cards.Disciple,
            Cards.DistantLands,
            Cards.Duplicate,
            Cards.Dungeon,
            Cards.Fugitive,
            Cards.Gear,
            Cards.Giant,
            Cards.Guide,
            Cards.HauntedWoods,
            Cards.Hero,
            Cards.Hireling,            
            Cards.Miser,
            Cards.Page,
            Cards.Peasant,
            Cards.Port,
            Cards.Ranger,
            Cards.RatCatcher,
            Cards.Raze,
            Cards.Relic,
            Cards.RoyalCarriage,
            Cards.Solider,            
            Cards.SwampHag,
            Cards.Teacher,
            Cards.Transmogrify,
            Cards.TreasureHunter,
            Cards.TreasureTrove,
            Cards.Warrior,
            Cards.WineMerchant,

            // events
            Cards.Alms,
            Cards.Ball,
            Cards.Borrow,
            Cards.Bonfire,
            Cards.Expedition,
            Cards.Ferry,
            Cards.Inheritance,
            Cards.LostArts,
            Cards.Mission,
            Cards.PathFinding,
            Cards.Pilgrimage,
            Cards.Plan,
            Cards.Quest,
            Cards.Raid,
            Cards.Save,
            Cards.ScoutingParty,
            Cards.Seaway,
            Cards.Trade,
            Cards.Training,
            Cards.TravellingFair
        };        
    }   
}

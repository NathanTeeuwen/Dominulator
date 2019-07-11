using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Dominion
{
    // All of the Card Instances.
    public static class Cards
    {
        public static readonly CardTypes.AbandonedMine AbandonedMine = CardTypes.AbandonedMine.card;
        public static readonly CardTypes.Adventurer Adventurer = CardTypes.Adventurer.card;
        public static readonly CardTypes.ActingTroupe ActingTroupe = CardTypes.ActingTroupe.card;
        public static readonly CardTypes.Advisor Advisor = CardTypes.Advisor.card;
        public static readonly CardTypes.Alchemist Alchemist = CardTypes.Alchemist.card;
        public static readonly CardTypes.Altar Altar = CardTypes.Altar.card;
        public static readonly CardTypes.Ambassador Ambassador = CardTypes.Ambassador.card;
        public static readonly CardTypes.Amulet Amulet = CardTypes.Amulet.card;
        public static readonly CardTypes.Apothecary Apothecary = CardTypes.Apothecary.card;
        public static readonly CardTypes.Apprentice Apprentice = CardTypes.Apprentice.card;
        public static readonly CardTypes.Archive Archive = CardTypes.Archive.card;
        public static readonly CardTypes.Armory Armory = CardTypes.Armory.card;
        public static readonly CardTypes.Artificer Artificer = CardTypes.Artificer.card;
        public static readonly CardTypes.Artisan Artisan = CardTypes.Artisan.card;
        public static readonly CardTypes.Avanto Avanto = CardTypes.Avanto.card;
        public static readonly CardTypes.BagOfGold BagOfGold = CardTypes.BagOfGold.card;
        public static readonly CardTypes.Baker Baker = CardTypes.Baker.card;
        public static readonly CardTypes.Bandit Bandit = CardTypes.Bandit.card;
        public static readonly CardTypes.BanditCamp BanditCamp = CardTypes.BanditCamp.card;
        public static readonly CardTypes.BandOfMisfits BandOfMisfits = CardTypes.BandOfMisfits.card;
        public static readonly CardTypes.Bank Bank = CardTypes.Bank.card;
        public static readonly CardTypes.Bard Bard = CardTypes.Bard.card;
        public static readonly CardTypes.Baron Baron = CardTypes.Baron.card;
        public static readonly CardTypes.Bat Bat = CardTypes.Bat.card;
        public static readonly CardTypes.Bazaar Bazaar = CardTypes.Bazaar.card;
        public static readonly CardTypes.Beggar Beggar = CardTypes.Beggar.card;
        public static readonly CardTypes.Bishop Bishop = CardTypes.Bishop.card;
        public static readonly CardTypes.BlackMarket BlackMarket = CardTypes.BlackMarket.card;
        public static readonly CardTypes.BlessedVillage BlessedVillage = CardTypes.BlessedVillage.card;
        public static readonly CardTypes.BorderVillage BorderVillage = CardTypes.BorderVillage.card;
        public static readonly CardTypes.BorderGuard BorderGuard = CardTypes.BorderGuard.card;
        public static readonly CardTypes.Bridge Bridge = CardTypes.Bridge.card;
        public static readonly CardTypes.BridgeTroll BridgeTroll = CardTypes.BridgeTroll.card;
        public static readonly CardTypes.Bureaucrat Bureaucrat = CardTypes.Bureaucrat.card;
        public static readonly CardTypes.BustlingVillage BustlingVillage = CardTypes.BustlingVillage.card;
        public static readonly CardTypes.Butcher Butcher = CardTypes.Butcher.card;
        public static readonly CardTypes.Cache Cache = CardTypes.Cache.card;
        public static readonly CardTypes.CandlestickMaker CandlestickMaker = CardTypes.CandlestickMaker.card;
        public static readonly CardTypes.Capital Capital = CardTypes.Capital.card;
        public static readonly CardTypes.Castles Castles = CardTypes.Castles.card;
        public static readonly CardTypes.Caravan Caravan = CardTypes.Caravan.card;
        public static readonly CardTypes.CaravanGuard CaravanGuard = CardTypes.CaravanGuard.card;
        public static readonly CardTypes.CargoShip CargoShip = CardTypes.CargoShip.card;
        public static readonly CardTypes.Cartographer Cartographer = CardTypes.Cartographer.card;
        public static readonly CardTypes.Catacombs Catacombs = CardTypes.Catacombs.card;
        public static readonly CardTypes.Catapult Catapult = CardTypes.Catapult.card;
        public static readonly CardTypes.CatapultRocks CatapultRocks = CardTypes.CatapultRocks.card;
        public static readonly CardTypes.Cellar Cellar = CardTypes.Cellar.card;
        public static readonly CardTypes.Cemetery Cemetary = CardTypes.Cemetery.card;
        public static readonly CardTypes.Champion Champion = CardTypes.Champion.card;
        public static readonly CardTypes.Chancellor Chancellor = CardTypes.Chancellor.card;
        public static readonly CardTypes.Changeling Changeling = CardTypes.Changeling.card;
        public static readonly CardTypes.Chapel Chapel = CardTypes.Chapel.card;
        public static readonly CardTypes.ChariotRace ChariotRace = CardTypes.ChariotRace.card;
        public static readonly CardTypes.Charm Charm = CardTypes.Charm.card;
        public static readonly CardTypes.City City = CardTypes.City.card;
        public static readonly CardTypes.CityQuarter CityQuarter = CardTypes.CityQuarter.card;
        public static readonly CardTypes.Cobbler Cobbler = CardTypes.Cobbler.card;
        public static readonly CardTypes.Colony Colony = CardTypes.Colony.card;
        public static readonly CardTypes.CoinOfTheRealm CoinOfTheRealm = CardTypes.CoinOfTheRealm.card;
        public static readonly CardTypes.Conclave Conclave = CardTypes.Conclave.card;
        public static readonly CardTypes.Conspirator Conspirator = CardTypes.Conspirator.card;
        public static readonly CardTypes.Contraband Contraband = CardTypes.Contraband.card;
        public static readonly CardTypes.Copper Copper = CardTypes.Copper.card;
        public static readonly CardTypes.Coppersmith Coppersmith = CardTypes.Coppersmith.card;
        public static readonly CardTypes.CouncilRoom CouncilRoom = CardTypes.CouncilRoom.card;
        public static readonly CardTypes.Count Count = CardTypes.Count.card;
        public static readonly CardTypes.CounterFeit CounterFeit = CardTypes.CounterFeit.card;
        public static readonly CardTypes.CountingHouse CountingHouse = CardTypes.CountingHouse.card;
        public static readonly CardTypes.Courtier Courtier = CardTypes.Courtier.card;
        public static readonly CardTypes.Courtyard Courtyard = CardTypes.Courtyard.card;
        public static readonly CardTypes.CrumblingCastle CrumblingCastle = CardTypes.CrumblingCastle.card;
        public static readonly CardTypes.CrossRoads CrossRoads = CardTypes.CrossRoads.card;
        public static readonly CardTypes.Crown Crown = CardTypes.Crown.card;
        public static readonly CardTypes.Crypt Crypt = CardTypes.Crypt.card;
        public static readonly CardTypes.Cultist Cultist = CardTypes.Cultist.card;
        public static readonly CardTypes.Curse Curse = CardTypes.Curse.card;
        public static readonly CardTypes.CursedGold CursedGold = CardTypes.CursedGold.card;
        public static readonly CardTypes.CursedVillage CursedVillage = CardTypes.CursedVillage.card;
        public static readonly CardTypes.Cutpurse Cutpurse = CardTypes.Cutpurse.card;
        public static readonly CardTypes.DameAnna DameAnna = CardTypes.DameAnna.card;
        public static readonly CardTypes.DameJosephine DameJosephine = CardTypes.DameJosephine.card;
        public static readonly CardTypes.DameMolly DameMolly = CardTypes.DameMolly.card;
        public static readonly CardTypes.DameNatalie DameNatalie = CardTypes.DameNatalie.card;
        public static readonly CardTypes.DameSylvia DameSylvia = CardTypes.DameSylvia.card;
        public static readonly CardTypes.DeathCart DeathCart = CardTypes.DeathCart.card;
        public static readonly CardTypes.DenOfSin DenOfSin = CardTypes.DenOfSin.card;
        public static readonly CardTypes.Develop Develop = CardTypes.Develop.card;
        public static readonly CardTypes.DevilsWorkshop DevilsWorkshop = CardTypes.DevilsWorkshop.card;
        public static readonly CardTypes.Diadem Diadem = CardTypes.Diadem.card;
        public static readonly CardTypes.Dismantle Dismantle = CardTypes.Dismantle.card;
        public static readonly CardTypes.Disciple Disciple = CardTypes.Disciple.card;
        public static readonly CardTypes.DistantLands DistantLands = CardTypes.DistantLands.card;
        public static readonly CardTypes.Diplomat Diplomat = CardTypes.Diplomat.card;
        public static readonly CardTypes.Doctor Doctor = CardTypes.Doctor.card;
        public static readonly CardTypes.Druid Druid = CardTypes.Druid.card;
        public static readonly CardTypes.Duchess Duchess = CardTypes.Duchess.card;
        public static readonly CardTypes.Ducat Ducat = CardTypes.Ducat.card;
        public static readonly CardTypes.Duchy Duchy = CardTypes.Duchy.card;
        public static readonly CardTypes.Duke Duke = CardTypes.Duke.card;
        public static readonly CardTypes.Dungeon Dungeon = CardTypes.Dungeon.card;
        public static readonly CardTypes.Duplicate Duplicate = CardTypes.Duplicate.card;
        public static readonly CardTypes.Embargo Embargo = CardTypes.Embargo.card;
        public static readonly CardTypes.Embassy Embassy = CardTypes.Embassy.card;
        public static readonly CardTypes.Emporium Emporium = CardTypes.Emporium.card;
        public static readonly CardTypes.Encampment Encampment = CardTypes.Encampment.card;
        public static readonly CardTypes.EncampmentPlunder EncampmentPlunder = CardTypes.EncampmentPlunder.card;
        public static readonly CardTypes.Enchantress Enchantress = CardTypes.Enchantress.card;
        public static readonly CardTypes.Engineer Engineer = CardTypes.Engineer.card;
        public static readonly CardTypes.Envoy Envoy = CardTypes.Envoy.card;
        public static readonly CardTypes.Estate Estate = CardTypes.Estate.card;
        public static readonly CardTypes.Exorcist Exorcist = CardTypes.Exorcist.card;
        public static readonly CardTypes.Expand Expand = CardTypes.Expand.card;
        public static readonly CardTypes.Experiment Experiment = CardTypes.Experiment.card;
        public static readonly CardTypes.Explorer Explorer = CardTypes.Explorer.card;
        public static readonly CardTypes.Fairgrounds Fairgrounds = CardTypes.Fairgrounds.card;
        public static readonly CardTypes.FaithfulHound FaithfulHound = CardTypes.FaithfulHound.card;
        public static readonly CardTypes.Familiar Familiar = CardTypes.Familiar.card;
        public static readonly CardTypes.FarmingVillage FarmingVillage = CardTypes.FarmingVillage.card;
        public static readonly CardTypes.FarmersMarket FarmersMarket = CardTypes.FarmersMarket.card;
        public static readonly CardTypes.Farmland Farmland = CardTypes.Farmland.card;
        public static readonly CardTypes.Feast Feast = CardTypes.Feast.card;
        public static readonly CardTypes.Feodum Feodum = CardTypes.Feodum.card;
        public static readonly CardTypes.Festival Festival = CardTypes.Festival.card;
        public static readonly CardTypes.FishingVillage FishingVillage = CardTypes.FishingVillage.card;
        public static readonly CardTypes.FlagBearer FlagBearer = CardTypes.FlagBearer.card;
        public static readonly CardTypes.Followers Followers = CardTypes.Followers.card;
        public static readonly CardTypes.Fool Fool = CardTypes.Fool.card;
        public static readonly CardTypes.FoolsGold FoolsGold = CardTypes.FoolsGold.card;
        public static readonly CardTypes.Forager Forager = CardTypes.Forager.card;
        public static readonly CardTypes.Forge Forge = CardTypes.Forge.card;
        public static readonly CardTypes.Fortress Fortress = CardTypes.Fortress.card;
        public static readonly CardTypes.Fortune Fortune = CardTypes.Fortune.card;
        public static readonly CardTypes.FortuneTeller FortuneTeller = CardTypes.FortuneTeller.card;
        public static readonly CardTypes.Forum Forum = CardTypes.Forum.card;
        public static readonly CardTypes.Fugitive Fugitive = CardTypes.Fugitive.card;
        public static readonly CardTypes.Gardens Gardens = CardTypes.Gardens.card;
        public static readonly CardTypes.Gear Gear = CardTypes.Gear.card;
        public static readonly CardTypes.Ghost Ghost = CardTypes.Ghost.card;
        public static readonly CardTypes.GhostShip GhostShip = CardTypes.GhostShip.card;
        public static readonly CardTypes.GhostTown GhostTown = CardTypes.GhostTown.card;
        public static readonly CardTypes.Gladiator Gladiator = CardTypes.Gladiator.card;
        public static readonly CardTypes.GladiatorFortune GladiatorFortune = CardTypes.GladiatorFortune.card;
        public static readonly CardTypes.Giant Giant = CardTypes.Giant.card;
        public static readonly CardTypes.Goat Goat = CardTypes.Goat.card;
        public static readonly CardTypes.Gold Gold = CardTypes.Gold.card;
        public static readonly CardTypes.Golem Golem = CardTypes.Golem.card;
        public static readonly CardTypes.Goons Goons = CardTypes.Goons.card;
        public static readonly CardTypes.Governor Governor = CardTypes.Governor.card;
        public static readonly CardTypes.Guardian Guardian = CardTypes.Guardian.card;
        public static readonly CardTypes.Guide Guide = CardTypes.Guide.card;
        public static readonly CardTypes.GrandCastle GrandCastle = CardTypes.GrandCastle.card;
        public static readonly CardTypes.GrandMarket GrandMarket = CardTypes.GrandMarket.card;
        public static readonly CardTypes.Graverobber Graverobber = CardTypes.Graverobber.card;
        public static readonly CardTypes.GreatHall GreatHall = CardTypes.GreatHall.card;
        public static readonly CardTypes.Groundskeeper Groundskeeper = CardTypes.Groundskeeper.card;
        public static readonly CardTypes.Haggler Haggler = CardTypes.Haggler.card;
        public static readonly CardTypes.Hamlet Hamlet = CardTypes.Hamlet.card;
        public static readonly CardTypes.Harbinger Harbinger = CardTypes.Harbinger.card;
        public static readonly CardTypes.Harem Harem = CardTypes.Harem.card;
        public static readonly CardTypes.Harvest Harvest = CardTypes.Harvest.card;
        public static readonly CardTypes.HauntedCastle HauntedCastle = CardTypes.HauntedCastle.card;
        public static readonly CardTypes.HauntedMirror HauntedMirror = CardTypes.HauntedMirror.card;
        public static readonly CardTypes.HauntedWoods HauntedWoods = CardTypes.HauntedWoods.card;
        public static readonly CardTypes.Haven Haven = CardTypes.Haven.card;
        public static readonly CardTypes.Herald Herald = CardTypes.Herald.card;
        public static readonly CardTypes.Hero Hero = CardTypes.Hero.card;
        public static readonly CardTypes.Herbalist Herbalist = CardTypes.Herbalist.card;
        public static readonly CardTypes.Hermit Hermit = CardTypes.Hermit.card;
        public static readonly CardTypes.Hideout Hideout = CardTypes.Hideout.card;
        public static readonly CardTypes.Highway Highway = CardTypes.Highway.card;
        public static readonly CardTypes.Hireling Hireling = CardTypes.Hireling.card;
        public static readonly CardTypes.Hoard Hoard = CardTypes.Hoard.card;
        public static readonly CardTypes.HornOfPlenty HornOfPlenty = CardTypes.HornOfPlenty.card;
        public static readonly CardTypes.HorseTraders HorseTraders = CardTypes.HorseTraders.card;
        public static readonly CardTypes.Hovel Hovel = CardTypes.Hovel.card;
        public static readonly CardTypes.HumbleCastle HumbleCastle = CardTypes.HumbleCastle.card;
        public static readonly CardTypes.HuntingGrounds HuntingGrounds = CardTypes.HuntingGrounds.card;
        public static readonly CardTypes.HuntingParty HuntingParty = CardTypes.HuntingParty.card;
        public static readonly CardTypes.Idol Idol = CardTypes.Idol.card;
        public static readonly CardTypes.IllGottenGains IllGottenGains = CardTypes.IllGottenGains.card;
        public static readonly CardTypes.Imp Imp = CardTypes.Imp.card;
        public static readonly CardTypes.Improve Improve = CardTypes.Improve.card;
        public static readonly CardTypes.Inn Inn = CardTypes.Inn.card;
        public static readonly CardTypes.Inventor Inventor = CardTypes.Inventor.card;
        public static readonly CardTypes.IronMonger IronMonger = CardTypes.IronMonger.card;
        public static readonly CardTypes.IronWorks IronWorks = CardTypes.IronWorks.card;
        public static readonly CardTypes.Island Island = CardTypes.Island.card;
        public static readonly CardTypes.JackOfAllTrades JackOfAllTrades = CardTypes.JackOfAllTrades.card;
        public static readonly CardTypes.Jester Jester = CardTypes.Jester.card;
        public static readonly CardTypes.Journeyman Journeyman = CardTypes.Journeyman.card;
        public static readonly CardTypes.JunkDealer JunkDealer = CardTypes.JunkDealer.card;
        public static readonly CardTypes.KingsCastle KingsCastle = CardTypes.KingsCastle.card;
        public static readonly CardTypes.KingsCourt KingsCourt = CardTypes.KingsCourt.card;
        public static readonly CardTypes.Knights Knights = CardTypes.Knights.card;
        public static readonly CardTypes.Laboratory Laboratory = CardTypes.Laboratory.card;
        public static readonly CardTypes.Lackeys Lackeys = CardTypes.Lackeys.card;
        public static readonly CardTypes.Legionary Legionary = CardTypes.Legionary.card;
        public static readonly CardTypes.Leprechaun Leprechaun = CardTypes.Leprechaun.card;
        public static readonly CardTypes.Library Library = CardTypes.Library.card;
        public static readonly CardTypes.Lighthouse Lighthouse = CardTypes.Lighthouse.card;
        public static readonly CardTypes.Loan Loan = CardTypes.Loan.card;
        public static readonly CardTypes.Lookout Lookout = CardTypes.Lookout.card;
        public static readonly CardTypes.LostCity LostCity = CardTypes.LostCity.card;
        public static readonly CardTypes.LuckyCoin LuckyCoin = CardTypes.LuckyCoin.card;
        public static readonly CardTypes.Lurker Lurker = CardTypes.Lurker.card;
        public static readonly CardTypes.Madman Madman = CardTypes.Madman.card;
        public static readonly CardTypes.Mandarin Mandarin = CardTypes.Mandarin.card;
        public static readonly CardTypes.MagicLamp MagicLamp = CardTypes.MagicLamp.card;
        public static readonly CardTypes.Magpie Magpie = CardTypes.Magpie.card;
        public static readonly CardTypes.Marauder Marauder = CardTypes.Marauder.card;
        public static readonly CardTypes.Margrave Margrave = CardTypes.Margrave.card;
        public static readonly CardTypes.Market Market = CardTypes.Market.card;
        public static readonly CardTypes.MarketSquare MarketSquare = CardTypes.MarketSquare.card;
        public static readonly CardTypes.Masquerade Masquerade = CardTypes.Masquerade.card;
        public static readonly CardTypes.Masterpiece Masterpiece = CardTypes.Masterpiece.card;
        public static readonly CardTypes.Menagerie Menagerie = CardTypes.Menagerie.card;
        public static readonly CardTypes.Mercenary Mercenary = CardTypes.Mercenary.card;
        public static readonly CardTypes.Merchant Merchant = CardTypes.Merchant.card;
        public static readonly CardTypes.MerchantGuild MerchantGuild = CardTypes.MerchantGuild.card;
        public static readonly CardTypes.MerchantShip MerchantShip = CardTypes.MerchantShip.card;
        public static readonly CardTypes.Messenger Messenger = CardTypes.Messenger.card;
        public static readonly CardTypes.Militia Militia = CardTypes.Militia.card;
        public static readonly CardTypes.Mill Mill = CardTypes.Mill.card;
        public static readonly CardTypes.Mine Mine = CardTypes.Mine.card;
        public static readonly CardTypes.MiningVillage MiningVillage = CardTypes.MiningVillage.card;
        public static readonly CardTypes.Minion Minion = CardTypes.Minion.card;
        public static readonly CardTypes.Mint Mint = CardTypes.Mint.card;
        public static readonly CardTypes.Miser Miser = CardTypes.Miser.card;
        public static readonly CardTypes.Moat Moat = CardTypes.Moat.card;
        public static readonly CardTypes.Monastery Monastery = CardTypes.Monastery.card;
        public static readonly CardTypes.Moneylender Moneylender = CardTypes.Moneylender.card;
        public static readonly CardTypes.Monument Monument = CardTypes.Monument.card;
        public static readonly CardTypes.MountainVillage MountainVillage = CardTypes.MountainVillage.card;
        public static readonly CardTypes.Mountebank Mountebank = CardTypes.Mountebank.card;
        public static readonly CardTypes.Mystic Mystic = CardTypes.Mystic.card;
        public static readonly CardTypes.NativeVillage NativeVillage = CardTypes.NativeVillage.card;
        public static readonly CardTypes.Navigator Navigator = CardTypes.Navigator.card;
        public static readonly CardTypes.Necropolis Necropolis = CardTypes.Necropolis.card;
        public static readonly CardTypes.NobleBrigand NobleBrigand = CardTypes.NobleBrigand.card;
        public static readonly CardTypes.Nobles Nobles = CardTypes.Nobles.card;
        public static readonly CardTypes.NomadCamp NomadCamp = CardTypes.NomadCamp.card;
        public static readonly CardTypes.Necromancer Necromancer = CardTypes.Necromancer.card;
        public static readonly CardTypes.NightWatchman NightWatchman = CardTypes.NightWatchman.card;
        public static readonly CardTypes.Oasis Oasis = CardTypes.Oasis.card;
        public static readonly CardTypes.OldWitch OldWitch = CardTypes.OldWitch.card;
        public static readonly CardTypes.OpulentCastle OpulentCastle = CardTypes.OpulentCastle.card;
        public static readonly CardTypes.Oracle Oracle = CardTypes.Oracle.card;
        public static readonly CardTypes.Outpost Outpost = CardTypes.Outpost.card;
        public static readonly CardTypes.OvergrownEstate OvergrownEstate = CardTypes.OvergrownEstate.card;
        public static readonly CardTypes.Overlord Overlord = CardTypes.Overlord.card;
        public static readonly CardTypes.Page Page = CardTypes.Page.card;
        public static readonly CardTypes.Pasture Pasture = CardTypes.Pasture.card;
        public static readonly CardTypes.PatricianEmporium PatricianEmporium = CardTypes.PatricianEmporium.card;
        public static readonly CardTypes.Patrician Patrician = CardTypes.Patrician.card;
        public static readonly CardTypes.Patrol Patrol = CardTypes.Patrol.card;
        public static readonly CardTypes.Patron Patron = CardTypes.Patron.card;
        public static readonly CardTypes.Pawn Pawn = CardTypes.Pawn.card;
        public static readonly CardTypes.Peasant Peasant = CardTypes.Peasant.card;
        public static readonly CardTypes.PearlDiver PearlDiver = CardTypes.PearlDiver.card;
        public static readonly CardTypes.Peddler Peddler = CardTypes.Peddler.card;
        public static readonly CardTypes.PhilosophersStone PhilosophersStone = CardTypes.PhilosophersStone.card;
        public static readonly CardTypes.Pillage Pillage = CardTypes.Pillage.card;
        public static readonly CardTypes.PirateShip PirateShip = CardTypes.PirateShip.card;
        public static readonly CardTypes.Pixie Pixie = CardTypes.Pixie.card;
        public static readonly CardTypes.Platinum Platinum = CardTypes.Platinum.card;
        public static readonly CardTypes.Plaza Plaza = CardTypes.Plaza.card;
        public static readonly CardTypes.Plunder Plunder = CardTypes.Plunder.card;
        public static readonly CardTypes.Poacher Poacher = CardTypes.Poacher.card;
        public static readonly CardTypes.Pooka Pooka = CardTypes.Pooka.card;
        public static readonly CardTypes.PoorHouse PoorHouse = CardTypes.PoorHouse.card;
        public static readonly CardTypes.Port Port = CardTypes.Port.card;
        public static readonly CardTypes.Possession Possession = CardTypes.Possession.card;
        public static readonly CardTypes.Pouch Pouch = CardTypes.Pouch.card;
        public static readonly CardTypes.Potion Potion = CardTypes.Potion.card;
        public static readonly CardTypes.Priest Priest = CardTypes.Priest.card;
        public static readonly CardTypes.Princess Princess = CardTypes.Princess.card;
        public static readonly CardTypes.Prince Prince = CardTypes.Prince.card;
        public static readonly CardTypes.Prize Prize = CardTypes.Prize.card;
        public static readonly CardTypes.Procession Procession = CardTypes.Procession.card;
        public static readonly CardTypes.Province Province = CardTypes.Province.card;
        public static readonly CardTypes.Quarry Quarry = CardTypes.Quarry.card;
        public static readonly CardTypes.Rabble Rabble = CardTypes.Rabble.card;
        public static readonly CardTypes.Raider Raider = CardTypes.Raider.card;
        public static readonly CardTypes.Ranger Ranger = CardTypes.Ranger.card;
        public static readonly CardTypes.Rats Rats = CardTypes.Rats.card;
        public static readonly CardTypes.RatCatcher RatCatcher = CardTypes.RatCatcher.card;
        public static readonly CardTypes.Raze Raze = CardTypes.Raze.card;
        public static readonly CardTypes.Rebuild Rebuild = CardTypes.Rebuild.card;
        public static readonly CardTypes.Recruiter Recruiter = CardTypes.Recruiter.card;
        public static readonly CardTypes.Relic Relic = CardTypes.Relic.card;
        public static readonly CardTypes.Remake Remake = CardTypes.Remake.card;
        public static readonly CardTypes.Remodel Remodel = CardTypes.Remodel.card;
        public static readonly CardTypes.Replace Replace = CardTypes.Replace.card;
        public static readonly CardTypes.Research Research = CardTypes.Research.card;
        public static readonly CardTypes.Rocks Rocks = CardTypes.Rocks.card;
        public static readonly CardTypes.Rogue Rogue = CardTypes.Rogue.card;
        public static readonly CardTypes.RoyalBlacksmith RoyalBlacksmith = CardTypes.RoyalBlacksmith.card;
        public static readonly CardTypes.RoyalSeal RoyalSeal = CardTypes.RoyalSeal.card;
        public static readonly CardTypes.RoyalCarriage RoyalCarriage = CardTypes.RoyalCarriage.card;
        public static readonly CardTypes.RuinedLibrary RuinedLibrary = CardTypes.RuinedLibrary.card;
        public static readonly CardTypes.RuinedMarket RuinedMarket = CardTypes.RuinedMarket.card;
        public static readonly CardTypes.RuinedVillage RuinedVillage = CardTypes.RuinedVillage.card;
        public static readonly CardTypes.Ruins Ruins = CardTypes.Ruins.card;
        public static readonly CardTypes.Saboteur Saboteur = CardTypes.Saboteur.card;
        public static readonly CardTypes.SacredGrove SacredGrove = CardTypes.SacredGrove.card;
        public static readonly CardTypes.Sacrifice Sacrifice = CardTypes.Sacrifice.card;
        public static readonly CardTypes.Sage Sage = CardTypes.Sage.card;
        public static readonly CardTypes.Salvager Salvager = CardTypes.Salvager.card;
        public static readonly CardTypes.Sauna Sauna = CardTypes.Sauna.card;
        public static readonly CardTypes.SaunaAvanto SaunaAvanto = CardTypes.SaunaAvanto.card;
        public static readonly CardTypes.Scavenger Scavenger = CardTypes.Scavenger.card;
        public static readonly CardTypes.Scepter Scepter = CardTypes.Scepter.card;
        public static readonly CardTypes.Scheme Scheme = CardTypes.Scheme.card;
        public static readonly CardTypes.Scholar Scholar = CardTypes.Scholar.card;
        public static readonly CardTypes.Scout Scout = CardTypes.Scout.card;
        public static readonly CardTypes.ScryingPool ScryingPool = CardTypes.ScryingPool.card;
        public static readonly CardTypes.Sculptor Sculptor = CardTypes.Sculptor.card;
        public static readonly CardTypes.SeaHag SeaHag = CardTypes.SeaHag.card;
        public static readonly CardTypes.SecretCave SecretCave = CardTypes.SecretCave.card;
        public static readonly CardTypes.SecretChamber SecretChamber = CardTypes.SecretChamber.card;
        public static readonly CardTypes.SecretPassage SecretPassage = CardTypes.SecretPassage.card;
        public static readonly CardTypes.Seer Seer = CardTypes.Seer.card;
        public static readonly CardTypes.Sentry Sentry = CardTypes.Sentry.card;
        public static readonly CardTypes.Settlers Settlers = CardTypes.Settlers.card;
        public static readonly CardTypes.SettlersBustlingVillage SettlersBustlingVillage = CardTypes.SettlersBustlingVillage.card;
        public static readonly CardTypes.ShantyTown ShantyTown = CardTypes.ShantyTown.card;
        public static readonly CardTypes.Shepherd Shepherd = CardTypes.Shepherd.card;
        public static readonly CardTypes.SirBailey SirBailey = CardTypes.SirBailey.card;
        public static readonly CardTypes.SirDestry SirDestry = CardTypes.SirDestry.card;
        public static readonly CardTypes.SirMartin SirMartin = CardTypes.SirMartin.card;
        public static readonly CardTypes.SirMichael SirMichael = CardTypes.SirMichael.card;
        public static readonly CardTypes.SirVander SirVander = CardTypes.SirVander.card;
        public static readonly CardTypes.SilkMerchant SilkMerchant = CardTypes.SilkMerchant.card;
        public static readonly CardTypes.SilkRoad SilkRoad = CardTypes.SilkRoad.card;
        public static readonly CardTypes.Silver Silver = CardTypes.Silver.card;
        public static readonly CardTypes.Skulk Skulk = CardTypes.Skulk.card;
        public static readonly CardTypes.SmallCastle SmallCastle = CardTypes.SmallCastle.card;
        public static readonly CardTypes.Smithy Smithy = CardTypes.Smithy.card;
        public static readonly CardTypes.Smugglers Smugglers = CardTypes.Smugglers.card;
        public static readonly CardTypes.Soldier Soldier = CardTypes.Soldier.card;
        public static readonly CardTypes.Soothsayer Soothsayer = CardTypes.Soothsayer.card;
        public static readonly CardTypes.SpiceMerchant SpiceMerchant = CardTypes.SpiceMerchant.card;
        public static readonly CardTypes.Spoils Spoils = CardTypes.Spoils.card;
        public static readonly CardTypes.Spices Spices = CardTypes.Spices.card;
        public static readonly CardTypes.SprawlingCastle SprawlingCastle = CardTypes.SprawlingCastle.card;
        public static readonly CardTypes.Spy Spy = CardTypes.Spy.card;
        public static readonly CardTypes.Squire Squire = CardTypes.Squire.card;
        public static readonly CardTypes.Stables Stables = CardTypes.Stables.card;
        public static readonly CardTypes.Stash Stash = CardTypes.Stash.card;
        public static readonly CardTypes.Steward Steward = CardTypes.Steward.card;
        public static readonly CardTypes.StoneMason StoneMason = CardTypes.StoneMason.card;
        public static readonly CardTypes.Storeroom Storeroom = CardTypes.Storeroom.card;
        public static readonly CardTypes.Storyteller Storyteller = CardTypes.Storyteller.card;
        public static readonly CardTypes.SwampHag SwampHag = CardTypes.SwampHag.card;
        public static readonly CardTypes.Swashbuckler Swashbuckler = CardTypes.Swashbuckler.card;
        public static readonly CardTypes.Survivors Survivors = CardTypes.Survivors.card;
        public static readonly CardTypes.Swindler Swindler = CardTypes.Swindler.card;
        public static readonly CardTypes.Tactician Tactician = CardTypes.Tactician.card;
        public static readonly CardTypes.Talisman Talisman = CardTypes.Talisman.card;
        public static readonly CardTypes.Taxman Taxman = CardTypes.Taxman.card;
        public static readonly CardTypes.Teacher Teacher = CardTypes.Teacher.card;
        public static readonly CardTypes.Temple Temple = CardTypes.Temple.card;
        public static readonly CardTypes.Thief Thief = CardTypes.Thief.card;
        public static readonly CardTypes.ThroneRoom ThroneRoom = CardTypes.ThroneRoom.card;
        public static readonly CardTypes.Tormentor Tormentor = CardTypes.Tormentor.card;
        public static readonly CardTypes.Torturer Torturer = CardTypes.Torturer.card;
        public static readonly CardTypes.Tournament Tournament = CardTypes.Tournament.card;
        public static readonly CardTypes.Tracker Tracker = CardTypes.Tracker.card;
        public static readonly CardTypes.Trader Trader = CardTypes.Trader.card;
        public static readonly CardTypes.TradeRoute TradeRoute = CardTypes.TradeRoute.card;
        public static readonly CardTypes.TradingPost TradingPost = CardTypes.TradingPost.card;
        public static readonly CardTypes.TragicHero TragicHero = CardTypes.TragicHero.card;
        public static readonly CardTypes.Transmute Transmute = CardTypes.Transmute.card;
        public static readonly CardTypes.Transmogrify Transmogrify = CardTypes.Transmogrify.card;
        public static readonly CardTypes.TreasureMap TreasureMap = CardTypes.TreasureMap.card;
        public static readonly CardTypes.Treasurer Treasurer = CardTypes.Treasurer.card;
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
        public static readonly CardTypes.Vampire Vampire = CardTypes.Vampire.card;
        public static readonly CardTypes.Vassal Vassal = CardTypes.Vassal.card;
        public static readonly CardTypes.Vault Vault = CardTypes.Vault.card;
        public static readonly CardTypes.Venture Venture = CardTypes.Venture.card;
        public static readonly CardTypes.Villa Villa = CardTypes.Villa.card;
        public static readonly CardTypes.Village Village = CardTypes.Village.card;
        public static readonly CardTypes.Villain Villain = CardTypes.Villain.card;
        public static readonly CardTypes.Vineyard Vineyard = CardTypes.Vineyard.card;
        public static readonly CardTypes.WalledVillage WalledVillage = CardTypes.WalledVillage.card;
        public static readonly CardTypes.WanderingMinstrel WanderingMinstrel = CardTypes.WanderingMinstrel.card;
        public static readonly CardTypes.Warehouse Warehouse = CardTypes.Warehouse.card;
        public static readonly CardTypes.Warrior Warrior = CardTypes.Warrior.card;
        public static readonly CardTypes.Watchtower Watchtower = CardTypes.Watchtower.card;
        public static readonly CardTypes.Werewolf Werewolf = CardTypes.Werewolf.card;
        public static readonly CardTypes.WildHunt WildHunt = CardTypes.WildHunt.card;
        public static readonly CardTypes.WilloWisp WilloWisp = CardTypes.WilloWisp.card;
        public static readonly CardTypes.Wish Wish = CardTypes.Wish.card;
        public static readonly CardTypes.Wharf Wharf = CardTypes.Wharf.card;
        public static readonly CardTypes.WineMerchant WineMerchant = CardTypes.WineMerchant.card;
        public static readonly CardTypes.WishingWell WishingWell = CardTypes.WishingWell.card;
        public static readonly CardTypes.Witch Witch = CardTypes.Witch.card;
        public static readonly CardTypes.WoodCutter WoodCutter = CardTypes.WoodCutter.card;
        public static readonly CardTypes.WorkersVillage WorkersVillage = CardTypes.WorkersVillage.card;
        public static readonly CardTypes.Workshop Workshop = CardTypes.Workshop.card;
        public static readonly CardTypes.YoungWitch YoungWitch = CardTypes.YoungWitch.card;
        public static readonly CardTypes.ZombieApprentice ZombieApprentice = CardTypes.ZombieApprentice.card;
        public static readonly CardTypes.ZombieMason ZombieMason = CardTypes.ZombieMason.card;
        public static readonly CardTypes.ZombieSpy ZombieSpy = CardTypes.ZombieSpy.card;

        // Events

        public static readonly CardTypes.Alms Alms = CardTypes.Alms.card;
        public static readonly CardTypes.Advance Advance = CardTypes.Advance.card;
        public static readonly CardTypes.Annex Annex = CardTypes.Annex.card;
        public static readonly CardTypes.Ball Ball = CardTypes.Ball.card;
        public static readonly CardTypes.Banquet Banquet = CardTypes.Banquet.card;
        public static readonly CardTypes.Borrow Borrow = CardTypes.Borrow.card;
        public static readonly CardTypes.Bonfire Bonfire = CardTypes.Bonfire.card;
        public static readonly CardTypes.Conquest Conquest = CardTypes.Conquest.card;
        public static readonly CardTypes.Delve Delve = CardTypes.Delve.card;
        public static readonly CardTypes.Dominate Dominate = CardTypes.Dominate.card;
        public static readonly CardTypes.Donate Donate = CardTypes.Donate.card;
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
        public static readonly CardTypes.Ritual Ritual = CardTypes.Ritual.card;
        public static readonly CardTypes.SaltTheEarth SalthTheEarth = CardTypes.SaltTheEarth.card;
        public static readonly CardTypes.Save Save = CardTypes.Save.card;
        public static readonly CardTypes.ScoutingParty ScoutingParty = CardTypes.ScoutingParty.card;
        public static readonly CardTypes.Seaway Seaway = CardTypes.Seaway.card;
        public static readonly CardTypes.Summon Summon = CardTypes.Summon.card;
        public static readonly CardTypes.Tax Tax = CardTypes.Tax.card;
        public static readonly CardTypes.Trade Trade = CardTypes.Trade.card;
        public static readonly CardTypes.Training Training = CardTypes.Training.card;
        public static readonly CardTypes.TravellingFair TravellingFair = CardTypes.TravellingFair.card;
        public static readonly CardTypes.Triumph Triumph = CardTypes.Triumph.card;
        public static readonly CardTypes.Wedding Wedding = CardTypes.Wedding.card;
        public static readonly CardTypes.Windfall Windfall = CardTypes.Windfall.card;


        //Landmarks
        public static readonly CardTypes.Aqueduct aqueduct = CardTypes.Aqueduct.card;
        public static readonly CardTypes.Arena arena = CardTypes.Arena.card;
        public static readonly CardTypes.BanditFort banditFort = CardTypes.BanditFort.card;
        public static readonly CardTypes.Basilica basilica = CardTypes.Basilica.card;
        public static readonly CardTypes.Baths baths = CardTypes.Baths.card;
        public static readonly CardTypes.BattleField battleField = CardTypes.BattleField.card;
        public static readonly CardTypes.Colonnade colonnade = CardTypes.Colonnade.card;
        public static readonly CardTypes.DefiledShrine defiledShrine = CardTypes.DefiledShrine.card;
        public static readonly CardTypes.Fountain fountain = CardTypes.Fountain.card;
        public static readonly CardTypes.Keep keep = CardTypes.Keep.card;
        public static readonly CardTypes.Labyrinth labyrinth = CardTypes.Labyrinth.card;
        public static readonly CardTypes.MountainPass mountainPass = CardTypes.MountainPass.card;
        public static readonly CardTypes.Museum museum = CardTypes.Museum.card;
        public static readonly CardTypes.Obelisk obelisk = CardTypes.Obelisk.card;
        public static readonly CardTypes.Orchard orchard = CardTypes.Orchard.card;
        public static readonly CardTypes.Palace palace = CardTypes.Palace.card;
        public static readonly CardTypes.Tomb tomb = CardTypes.Tomb.card;
        public static readonly CardTypes.Tower tower = CardTypes.Tower.card;
        public static readonly CardTypes.TriumphalArch triumphalArch = CardTypes.TriumphalArch.card;
        public static readonly CardTypes.Wall wall = CardTypes.Wall.card;
        public static readonly CardTypes.WolfDen wolfDen = CardTypes.WolfDen.card;

        //Hex
        public static readonly CardTypes.BadOmens BadOmens = CardTypes.BadOmens.card;
        public static readonly CardTypes.Delusion Delusion = CardTypes.Delusion.card;
        public static readonly CardTypes.Envy Envy = CardTypes.Envy.card;
        public static readonly CardTypes.Famine Famine = CardTypes.Famine.card;
        public static readonly CardTypes.Fear Fear = CardTypes.Fear.card;
        public static readonly CardTypes.Locusts Locusts = CardTypes.Locusts.card;
        public static readonly CardTypes.Misery Misery = CardTypes.Misery.card;
        public static readonly CardTypes.War War = CardTypes.War.card;
        public static readonly CardTypes.Greed Greed = CardTypes.Greed.card;
        public static readonly CardTypes.Haunting Haunting = CardTypes.Haunting.card;
        public static readonly CardTypes.Plague Plague = CardTypes.Plague.card;
        public static readonly CardTypes.Poverty Poverty = CardTypes.Poverty.card;

        //Boon
        public static readonly CardTypes.TheMoonsGift TheMoonsGift = CardTypes.TheMoonsGift.card;
        public static readonly CardTypes.TheRiversGift TheRiversGift = CardTypes.TheRiversGift.card;
        public static readonly CardTypes.TheSkysGift TheSkysGift = CardTypes.TheSkysGift.card;
        public static readonly CardTypes.TheEarthsGift TheEarthsGift = CardTypes.TheEarthsGift.card;
        public static readonly CardTypes.TheFieldsGift TheFieldsGift = CardTypes.TheFieldsGift.card;
        public static readonly CardTypes.TheFlagmesGift TheFlagmesGift = CardTypes.TheFlagmesGift.card;
        public static readonly CardTypes.TheForestsGift TheForestsGift = CardTypes.TheForestsGift.card;
        public static readonly CardTypes.TheMountainsGift TheMountainsGift = CardTypes.TheMountainsGift.card;
        public static readonly CardTypes.TheSeasGift TheSeasGift = CardTypes.TheSeasGift.card;
        public static readonly CardTypes.TheSunsGift TheSunsGift = CardTypes.TheSunsGift.card;
        public static readonly CardTypes.TheSwampsGift TheSwampsGift = CardTypes.TheSwampsGift.card;
        public static readonly CardTypes.TheWindsGift TheWindsGift = CardTypes.TheWindsGift.card;

        //States
        public static readonly CardTypes.Envious Envious = CardTypes.Envious.card;
        public static readonly CardTypes.LostInTheWoods LostInTheWoods = CardTypes.LostInTheWoods.card;
        public static readonly CardTypes.Miserable Miserable = CardTypes.Miserable.card;
        public static readonly CardTypes.TwiceMiserable TwiceMiserable = CardTypes.TwiceMiserable.card;

        //Artifacts
        public static readonly CardTypes.Horn Horn = CardTypes.Horn.card;
        public static readonly CardTypes.Lantern Lantern = CardTypes.Lantern.card;
        public static readonly CardTypes.Flag Flag = CardTypes.Flag.card;
        public static readonly CardTypes.Key Key = CardTypes.Key.card;
        public static readonly CardTypes.TreasureChest TreasureChest = CardTypes.TreasureChest.card;


        //Projects
        public static readonly CardTypes.Academy Academy = CardTypes.Academy.card;
        public static readonly CardTypes.Barracks Barracks = CardTypes.Barracks.card;
        public static readonly CardTypes.Canal Canal = CardTypes.Canal.card;
        public static readonly CardTypes.Capitalism Capitalism = CardTypes.Capitalism.card;
        public static readonly CardTypes.Cathedral Cathedral = CardTypes.Cathedral.card;
        public static readonly CardTypes.Citadel Citadel = CardTypes.Citadel.card;
        public static readonly CardTypes.CityGate CityGate = CardTypes.CityGate.card;
        public static readonly CardTypes.CropRotation CropRotation = CardTypes.CropRotation.card;
        public static readonly CardTypes.Exploration Exploration = CardTypes.Exploration.card;
        public static readonly CardTypes.Fair Fair = CardTypes.Fair.card;
        public static readonly CardTypes.Fleet Fleet = CardTypes.Fleet.card;
        public static readonly CardTypes.GuildHall GuildHall = CardTypes.GuildHall.card;
        public static readonly CardTypes.Innovation Innovation = CardTypes.Innovation.card;
        public static readonly CardTypes.Pageant Pageant = CardTypes.Pageant.card;
        public static readonly CardTypes.Piazza Piazza = CardTypes.Piazza.card;
        public static readonly CardTypes.RoadNetwork RoadNetwork = CardTypes.RoadNetwork.card;
        public static readonly CardTypes.Sewers Sewers = CardTypes.Sewers.card;
        public static readonly CardTypes.Silos Silos = CardTypes.Silos.card;
        public static readonly CardTypes.SinisterPlot SinisterPlot = CardTypes.SinisterPlot.card;
        public static readonly CardTypes.StarChart StarChart = CardTypes.StarChart.card;


        private static CardShapedObject[] AllCards()
        {
            var result = new List<CardShapedObject>();
            foreach (System.Reflection.FieldInfo fieldInfo in typeof(Cards).GetTypeInfo().DeclaredFields)
            {
                var fieldValue = fieldInfo.GetValue(null);
                if (fieldValue is CardShapedObject card)
                    result.Add(card);
            }
            
            return result.ToArray();
        }

        public static Card[] Shelters = new Card[] {
            Cards.OvergrownEstate,
            Cards.Hovel,
            Cards.Necropolis};

        private static Card[] AllKingdomCards()
        {
            var result = new List<Card>();
            foreach (CardShapedObject cardShapedObject in AllCardsList)
            {
                if (cardShapedObject is Dominion.Card card)
                {
                    if (card.isKingdomCard)
                        result.Add(card);
                }
            }
            return result.ToArray();
        }

        public static Card[] AllKingdomCardsListCache; 
        static public Card[] AllKingdomCardsList
        {
            get
            {
                if (AllKingdomCardsListCache == null)
                    AllKingdomCardsListCache = AllKingdomCards();
                return AllKingdomCardsListCache;
            }
        }

        static CardShapedObject[] AllCardsListcache;
        static public CardShapedObject[] AllCardsList
        {
            get
            {
                if (AllCardsListcache == null)
                    AllCardsListcache = AllCards();
                return AllCardsListcache;
            }
        }

        public static CardShapedObject[] UnimplementedCards = new CardShapedObject[]
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
            Cards.Fugitive,
            Cards.Gear,
            Cards.Giant,
            Cards.Guide,
            Cards.HauntedWoods,
            Cards.Hero,
            Cards.Hireling,
            Cards.Page,
            Cards.Peasant,
            Cards.Ranger,
            Cards.RatCatcher,
            Cards.Relic,
            Cards.RoyalCarriage,
            Cards.Soldier,
            Cards.SwampHag,
            Cards.Teacher,
            Cards.Transmogrify,
            Cards.TreasureHunter,
            Cards.Warrior,
            Cards.WineMerchant,

            // Empires
            Cards.Archive,
            Cards.BustlingVillage,
            Cards.Capital,
            Cards.Castles,
            Cards.HumbleCastle,
            Cards.CrumblingCastle,
            Cards.SmallCastle,
            Cards.HauntedCastle,
            Cards.OpulentCastle,
            Cards.SprawlingCastle,
            Cards.GrandCastle,
            Cards.KingsCastle,
            Cards.Catapult,
            Cards.CatapultRocks,
            Cards.Charm,
            Cards.Crown,
            Cards.Encampment,
            Cards.Enchantress,
            Cards.FarmersMarket,
            Cards.Fortune,
            Cards.Gladiator,
            Cards.GladiatorFortune,
            Cards.Legionary,
            Cards.Overlord,
            Cards.PatricianEmporium,
            Cards.Settlers,
            Cards.SettlersBustlingVillage,
            Cards.Temple,
            Cards.Villa,
            Cards.WildHunt,
            
            // Second Edition
            Cards.Artisan,
            Cards.Harbinger,
            Cards.Merchant,
            Cards.Vassal,
            Cards.Poacher,
            Cards.Bandit,
            Cards.Sentry,
            Cards.Lurker,
            Cards.Diplomat,
            Cards.Mill,
            Cards.SecretPassage,
            Cards.Courtier,
            Cards.Patrol,
            Cards.Replace,

            // Nocturne
            Cards.Bard,
            Cards.Bat,
            Cards.BlessedVillage,
            Cards.Changeling,
            Cards.Crypt,
            Cards.CursedGold,
            Cards.CursedVillage,
            Cards.DevilsWorkshop,
            Cards.Druid,
            Cards.Exorcist,
            Cards.FaithfulHound,
            Cards.Fool,
            Cards.Ghost,
            Cards.GhostTown,
            Cards.Goat,
            Cards.Guardian,
            Cards.HauntedMirror,
            Cards.Idol,
            Cards.Imp,
            Cards.Leprechaun,
            Cards.LuckyCoin,
            Cards.MagicLamp,
            Cards.Monastery,
            Cards.Necromancer,
            Cards.NightWatchman,
            Cards.Pasture,
            Cards.Pixie,
            Cards.Pooka,
            Cards.Pouch,
            Cards.Raider,
            Cards.SacredGrove,
            Cards.SecretCave,
            Cards.Shepherd,
            Cards.Skulk,
            Cards.Tormentor,
            Cards.Tracker,
            Cards.TragicHero,
            Cards.Vampire,
            Cards.Werewolf,
            Cards.WilloWisp,
            Cards.Wish,
            Cards.ZombieApprentice,
            Cards.ZombieMason,
            Cards.ZombieSpy,

            //Promos
            Cards.Dismantle,
            Cards.SaunaAvanto,

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
            Cards.TravellingFair,
            // Empires events
            Cards.Advance,
            Cards.Annex,
            Cards.Banquet,
            Cards.Conquest,
            Cards.Delve,
            Cards.Dominate,
            Cards.Donate,
            Cards.Ritual,
            Cards.SalthTheEarth,
            Cards.Tax,
            Cards.Triumph,
            Cards.Wedding,
            Cards.Windfall,
            
            //Renaissance
            Cards.ActingTroupe,
            Cards.BorderGuard,
            Cards.CargoShip,
            Cards.Ducat,
            Cards.Experiment,
            Cards.FlagBearer,
            Cards.Hideout,
            Cards.Improve,
            Cards.Inventor,
            Cards.Lackeys,
            Cards.MountainVillage,
            Cards.OldWitch,
            Cards.Patron,
            Cards.Priest,
            Cards.Recruiter,
            Cards.Research,
            Cards.Scepter,
            Cards.Scholar,
            Cards.Sculptor,
            Cards.Seer,
            Cards.SilkMerchant,
            Cards.Spices,
            Cards.Swashbuckler,
            Cards.Treasurer,
            Cards.Villain,

            //Artifacts
            Cards.Horn,
            Cards.Lantern,
            Cards.Flag,
            Cards.TreasureChest,
            Cards.Key,

            //Projects
            Cards.Academy,
            Cards.Barracks,
            Cards.Canal,
            Cards.Capitalism,
            Cards.Cathedral,
            Cards.Citadel,
            Cards.CityGate,
            Cards.CropRotation,
            Cards.Exploration,
            Cards.Fair,
            Cards.Fleet,
            Cards.GuildHall,
            Cards.Innovation,
            Cards.Pageant,
            Cards.Piazza,
            Cards.RoadNetwork,
            Cards.Sewers,
            Cards.Silos,
            Cards.SinisterPlot,
            Cards.StarChart,
        };
    }   
}

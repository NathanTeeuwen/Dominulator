using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion.CardTypes
{
    using Dominion;

    public class Bard
        : Card
    {
        public static Bard card = new Bard();

        private Bard()
            : base("Bard", Expansion.Nocturne, coinCost: 4, isAction: true, isFate:true, plusCoins:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Bat
        : Card
    {
        public static Bat card = new Bat();

        private Bat()
            : base("Bat", Expansion.Nocturne, coinCost: 2, isNight:true, isKingdomCard:false)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class BlessedVillage
        : Card
    {
        public static BlessedVillage card = new BlessedVillage();

        private BlessedVillage()
            : base("Blessed Village", Expansion.Nocturne, coinCost: 4, isAction: true, isFate: true, plusCards:1, plusActions:2)
        {
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Cemetery
        : Card
    {
        public static Cemetery card = new Cemetery();

        private Cemetery()
            : base("Cemetery", Expansion.Nocturne, coinCost: 4, victoryPoints: c => 2)
        {
        }

        public override bool DoSpecializedTrash(PlayerState selfPlayer, GameState gameState)
        {
            selfPlayer.RequestPlayerTrashCardsFromHand(gameState, 4, isOptional:true);
            return true;
        }
    }

    public class Changeling
        : Card
    {
        public static Changeling card = new Changeling();

        private Changeling()
            : base("Changeling", Expansion.Nocturne, coinCost: 3, isNight:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.MoveCardFromPlayToTrash(gameState);
            currentPlayer.RequestPlayerGainCardFromSupply(gameState, c => currentPlayer.CardsInPlay.Contains(c), "A copy of a card you have in play");
            throw new NotImplementedException(); // imlpement the onbuy effect
        }
    }

    public class Cobbler
        : Card
    {
        public static Cobbler card = new Cobbler();

        private Cobbler()
            : base("Cobbler", Expansion.Nocturne, coinCost: 5, isNight: true, isDuration: true)
        {
        }

        public override void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerGainCardFromSupply(gameState, c => c.CurrentCoinCost(currentPlayer) <= 4, "card costing up to 4 to your hand", isOptional: false, defaultLocation: DeckPlacement.Hand);
        }
    }

    public class Conclave
        : Card
    {
        public static Conclave card = new Conclave();

        private Conclave()
            : base("Conclave", Expansion.Nocturne, coinCost: 4, isAction:true,  plusCoins:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.RequestPlayerPlayActionFromHand(gameState, c => !currentPlayer.CardsInPlay.Contains(c), isOptional: true))
            {
                currentPlayer.AddActions(1);
            }
        }
    }

    public class Crypt
        : Card
    {
        public static Crypt card = new Crypt();

        private Crypt()
            : base("Crypt", Expansion.Nocturne, coinCost: 5, isNight: true, isDuration:true)
        {
        }

        public override void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class CursedGold
        : Card
    {
        public static CursedGold card = new CursedGold();

        private CursedGold()
            : base("Cursed Gold", Expansion.Nocturne, coinCost: 4, isTreasure: true, isHeirloom: true, isKingdomCard:false, plusCoins: 3)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class CursedVillage
        : Card
    {
        public static CursedVillage card = new CursedVillage();

        private CursedVillage()
            : base("Cursed Village", Expansion.Nocturne, coinCost: 5, isAction:true, isDoom:true, plusActions:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class DenOfSin
        : Card
    {
        public static DenOfSin card = new DenOfSin();

        private DenOfSin()
            : base("Den Of Sin", Expansion.Nocturne, coinCost: 5, isNight:true, isDuration:true)
        {
        }

        public override void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.DrawAdditionalCardsIntoHand(2, gameState);
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            return DeckPlacement.Hand;
        }
    }

    public class DevilsWorkshop
        : Card
    {
        public static DevilsWorkshop card = new DevilsWorkshop();

        private DevilsWorkshop()
            : base("Devil's Workshop", Expansion.Nocturne, coinCost: 4, isNight: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Druid
        : Card
    {
        public static Druid card = new Druid();

        private Druid()
            : base("Druid", Expansion.Nocturne, coinCost: 2, isAction:true, isFate:true, plusBuy:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Exorcist
        : Card
    {
        public static Exorcist card = new Exorcist();

        private Exorcist()
            : base("Exorcist", Expansion.Nocturne, coinCost: 2, isNight:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class FaithfulHound
        : Card
    {
        public static FaithfulHound card = new FaithfulHound();

        private FaithfulHound()
            : base("Faithful Hound", Expansion.Nocturne, coinCost:2, plusCards:2, isAction:true, isReaction:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Fool
        : Card
    {
        public static Fool card = new Fool();

        private Fool()
            : base("Fool", Expansion.Nocturne, coinCost:3, isAction: true, isFate: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Ghost
        : Card
    {
        public static Ghost card = new Ghost();

        private Ghost()
            : base("Ghost", Expansion.Nocturne, coinCost: 4, isNight: true, isDuration: true, isSpirit:true, isKingdomCard:false)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class GhostTown
        : Card
    {
        public static GhostTown card = new GhostTown();

        private GhostTown()
            : base("Ghost Town", Expansion.Nocturne, coinCost: 3, isNight:true, isDuration:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Goat
        : Card
    {
        public static Goat card = new Goat();

        private Goat()
            : base("Goat", Expansion.Nocturne, coinCost: 2, isTreasure:true, isHeirloom:true, isKingdomCard: false, plusCoins:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Guardian
        : Card
    {
        public static Guardian card = new Guardian();

        private Guardian()
            : base("Guardian", Expansion.Nocturne, coinCost:2, isNight: true, isDuration: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class HauntedMirror
        : Card
    {
        public static HauntedMirror card = new HauntedMirror();

        private HauntedMirror()
            : base("Haunted Mirror", Expansion.Nocturne, coinCost: 0, isTreasure:true, isHeirloom:true, isKingdomCard: false, plusCoins:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Idol
        : Card
    {
        public static Idol card = new Idol();

        private Idol()
            : base("Idol", Expansion.Nocturne, coinCost:5, plusCoins:2, isTreasure:true, isAttack:true, isFate:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Imp
        : Card
    {
        public static Imp card = new Imp();

        private Imp()
            : base("Imp", Expansion.Nocturne, coinCost: 2, plusCards:2, isAction:true, isSpirit:true, isKingdomCard:false)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Leprechaun
        : Card
    {
        public static Leprechaun card = new Leprechaun();

        private Leprechaun()
            : base("Leprechaun", Expansion.Nocturne, coinCost:3, isAction:true, isDoom:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class LuckyCoin
        : Card
    {
        public static LuckyCoin card = new LuckyCoin();

        private LuckyCoin()
            : base("Lucky Coin", Expansion.Nocturne, coinCost: 4, isTreasure:true, isHeirloom:true, isKingdomCard: false, plusCoins:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class MagicLamp
        : Card
    {
        public static MagicLamp card = new MagicLamp();

        private MagicLamp()
            : base("Magic Lamp", Expansion.Nocturne, coinCost:0, isTreasure:true, isHeirloom:true, isKingdomCard: false, plusCoins: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Monastery
        : Card
    {
        public static Monastery card = new Monastery();

        private Monastery()
            : base("Monastery", Expansion.Nocturne, coinCost: 2, isNight:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Necromancer
        : Card
    {
        public static Necromancer card = new Necromancer();

        private Necromancer()
            : base("Necromancer", Expansion.Nocturne, coinCost:4, isAction:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class NightWatchman
        : Card
    {
        public static NightWatchman card = new NightWatchman();

        private NightWatchman()
            : base("Night Watchman", Expansion.Nocturne, coinCost:3)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Pasture
        : Card
    {
        public static Pasture card = new Pasture();

        private Pasture()
            : base("Pasture", Expansion.Nocturne, coinCost: 2, isTreasure:true, isHeirloom:true, isKingdomCard: false, plusCoins: 1, victoryPoints: player => player.AllOwnedCards.Count)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Pixie
        : Card
    {
        public static Pixie card = new Pixie();

        private Pixie()
            : base("Pixie", Expansion.Nocturne, coinCost: 2, plusCards:1, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Pooka
        : Card
    {
        public static Pooka card = new Pooka();

        private Pooka()
            : base("Pooka", Expansion.Nocturne, coinCost: 5, isAction:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Pouch
        : Card
    {
        public static Pouch card = new Pouch();

        private Pouch()
            : base("Pouch", Expansion.Nocturne, coinCost: 2, isTreasure:true, isHeirloom:true, isKingdomCard: false, plusCoins: 1, plusBuy:1)
        {
        }
    }

    public class Raider
        : Card
    {
        public static Raider card = new Raider();

        private Raider()
            : base("Raider", Expansion.Nocturne, coinCost: 6, isNight:true, isDuration:true, isAttack:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class SacredGrove
        : Card
    {
        public static SacredGrove card = new SacredGrove();

        private SacredGrove()
            : base("Sacred Grove", Expansion.Nocturne, coinCost: 5, isAction:true, isFate:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class SecretCave
        : Card
    {
        public static SecretCave card = new SecretCave();

        private SecretCave()
            : base("Secret Cave", Expansion.Nocturne, coinCost: 3, isAction: true, isDuration: true, plusCards: 1, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Shepherd
        : Card
    {
        public static Shepherd card = new Shepherd();

        private Shepherd()
            : base("Shepherd", Expansion.Nocturne, coinCost: 4, isAction: true, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Skulk
        : Card
    {
        public static Skulk card = new Skulk();

        private Skulk()
            : base("Skulk", Expansion.Nocturne, coinCost: 4, isAction: true, isAttack:true, isDoom:true, plusBuy:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Tormentor
        : Card
    {
        public static Tormentor card = new Tormentor();

        private Tormentor()
            : base("Tormentor", Expansion.Nocturne, coinCost: 5, isAction: true, isAttack: true, isDoom: true, plusCoins:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Tracker
        : Card
    {
        public static Tracker card = new Tracker();

        private Tracker()
            : base("Tracker", Expansion.Nocturne, coinCost: 2, isAction: true, isFate:true, plusCoins: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class TragicHero
        : Card
    {
        public static TragicHero card = new TragicHero();

        private TragicHero()
            : base("Tragic Hero", Expansion.Nocturne, coinCost: 5, isAction: true, plusCards:3, plusBuy:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Vampire
        : Card
    {
        public static Vampire card = new Vampire();

        private Vampire()
            : base("Vampire", Expansion.Nocturne, coinCost: 5, isNight:true, isAttack:true, isDoom:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Werewolf
        : Card
    {
        public static Werewolf card = new Werewolf();

        private Werewolf()
            : base("Werewolf", Expansion.Nocturne, coinCost: 5, isAction:true, isNight: true, isAttack: true, isDoom: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class WilloWisp
        : Card
    {
        public static WilloWisp card = new WilloWisp();

        private WilloWisp()
            : base("Will-o'-Wisp", Expansion.Nocturne, coinCost: 0, plusCards:1, plusActions:1, isAction:true, isSpirit:true, isKingdomCard:false)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Wish
        : Card
    {
        public static Wish card = new Wish();

        private Wish()
            : base("Wish", Expansion.Nocturne, coinCost: 0, plusActions:1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class ZombieApprentice
        : Card
    {
        public static ZombieApprentice card = new ZombieApprentice();

        private ZombieApprentice()
            : base("Zombie Apprentice", Expansion.Nocturne, coinCost: 3, isAction:true, isZombie:true, isKingdomCard:false)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class ZombieMason
        : Card
    {
        public static ZombieMason card = new ZombieMason();

        private ZombieMason()
            : base("Zombie Mason", Expansion.Nocturne, coinCost: 3, isAction: true, isZombie: true, isKingdomCard: false)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class ZombieSpy
        : Card
    {
        public static ZombieSpy card = new ZombieSpy();

        private ZombieSpy()
            : base("Zombie Spy", Expansion.Nocturne, coinCost: 3, plusCards:1, plusActions:1, isAction: true, isZombie: true, isKingdomCard: false)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class BadOmens
        : Hex
    {
        public static BadOmens card = new BadOmens();

        private BadOmens()
            : base("Bad Omens", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedHex(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Delusion
        : Hex
    {
        public static Delusion card = new Delusion();

        private Delusion()
            : base("Delusion", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedHex(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Envy
        : Hex
    {
        public static Envy card = new Envy();

        private Envy()
            : base("Envy", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedHex(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Famine
        : Hex
    {
        public static Famine card = new Famine();

        private Famine()
            : base("Famine", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedHex(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Fear
        : Hex
    {
        public static Fear card = new Fear();

        private Fear()
            : base("Fear", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedHex(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Locusts
        : Hex
    {
        public static Locusts card = new Locusts();

        private Locusts()
            : base("Locusts", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedHex(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Misery
        : Hex
    {
        public static Misery card = new Misery();

        private Misery()
            : base("Misery", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedHex(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class War
        : Hex
    {
        public static War card = new War();

        private War()
            : base("War", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedHex(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Greed
        : Hex
    {
        public static Greed card = new Greed();

        private Greed()
            : base("Greed", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedHex(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Haunting
        : Hex
    {
        public static Haunting card = new Haunting();

        private Haunting()
            : base("Haunting", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedHex(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Plague
        : Hex
    {
        public static Plague card = new Plague();

        private Plague()
            : base("Plague", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedHex(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Poverty
        : Hex
    {
        public static Poverty card = new Poverty();

        private Poverty()
            : base("Poverty", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedHex(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class TheMoonsGift
        : Boon
    {
        public static TheMoonsGift card = new TheMoonsGift();

        private TheMoonsGift()
            : base("The Moon's Gift", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedBoon(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class TheRiversGift
        : Boon
    {
        public static TheRiversGift card = new TheRiversGift();

        private TheRiversGift()
            : base("The River's Gift", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedBoon(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class TheSkysGift
        : Boon
    {
        public static TheSkysGift card = new TheSkysGift();

        private TheSkysGift()
            : base("The Sky's Gift", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedBoon(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class TheEarthsGift
        : Boon
    {
        public static TheEarthsGift card = new TheEarthsGift();

        private TheEarthsGift()
            : base("The Earth's Gift", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedBoon(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class TheFieldsGift
        : Boon
    {
        public static TheFieldsGift card = new TheFieldsGift();

        private TheFieldsGift()
            : base("The Field's Gift", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedBoon(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class TheFlagmesGift
        : Boon
    {
        public static TheFlagmesGift card = new TheFlagmesGift();

        private TheFlagmesGift()
            : base("The Flagme's Gift", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedBoon(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class TheForestsGift
        : Boon
    {
        public static TheForestsGift card = new TheForestsGift();

        private TheForestsGift()
            : base("The Forest's Gift", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedBoon(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class TheMountainsGift
        : Boon
    {
        public static TheMountainsGift card = new TheMountainsGift();

        private TheMountainsGift()
            : base("The Mountain's Gift", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedBoon(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class TheSeasGift
        : Boon
    {
        public static TheSeasGift card = new TheSeasGift();

        private TheSeasGift()
            : base("The Sea's Gift", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedBoon(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class TheSunsGift
        : Boon
    {
        public static TheSunsGift card = new TheSunsGift();

        private TheSunsGift()
            : base("The Sun's Gift", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedBoon(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class TheSwampsGift
        : Boon
    {
        public static TheSwampsGift card = new TheSwampsGift();

        private TheSwampsGift()
            : base("The Swamp's Gift", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedBoon(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class TheWindsGift
        : Boon
    {
        public static TheWindsGift card = new TheWindsGift();

        private TheWindsGift()
            : base("The Wind's Gift", Expansion.Nocturne)
        {
        }

        public override void DoSpecializedBoon(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Deluded
        : State
    {
        public static Deluded card = new Deluded();

        private Deluded()
            : base("Deluded", Expansion.Nocturne)
        {
        }
    }

    public class Envious
        : State
    {
        public static Envious card = new Envious();

        private Envious()
            : base("Envious", Expansion.Nocturne)
        {
        }
    }

    public class LostInTheWoods
        : State
    {
        public static LostInTheWoods card = new LostInTheWoods();

        private LostInTheWoods()
            : base("Lost In The Woods", Expansion.Nocturne)
        {
        }
    }

    public class Miserable
        : State
    {
        public static Miserable card = new Miserable();

        private Miserable()
            : base("Miserable", Expansion.Nocturne)
        {
        }
    }

    public class TwiceMiserable
        : State
    {
        public static TwiceMiserable card = new TwiceMiserable();

        private TwiceMiserable()
            : base("TwiceMiserable", Expansion.Nocturne)
        {
        }
    }
}

using System;

namespace Dominion.CardTypes
{
    using Dominion;
  
    public class Advance
        : Event
    {
        public static Advance card = new Advance();

        private Advance()
            : base("Advance", Expansion.Empires, coinCost:0)
        {
        }
    }

    public class Annex
        : Event
    {
        public static Annex card = new Annex();

        private Annex()
            : base("Annex", Expansion.Empires, coinCost: 0, debtCost:8)
        {
        }
    }

    public class Banquet
        : Event
    {
        public static Banquet card = new Banquet();

        private Banquet()
            : base("Banquet", Expansion.Empires, coinCost: 3)
        {
        }
    }

    public class Conquest
        : Event
    {
        public static Conquest card = new Conquest();

        private Conquest()
            : base("Conquest", Expansion.Empires, coinCost: 6)
        {
        }
    }

    public class Delve
        : Event
    {
        public static Delve card = new Delve();

        private Delve()
            : base("Delve", Expansion.Empires, coinCost: 2)
        {
        }
    }

    public class Dominate
        : Event
    {
        public static Dominate card = new Dominate();

        private Dominate()
            : base("Dominate", Expansion.Empires, 14)
        {
        }       
    }

    public class Donate
        : Event
    {
        public static Donate card = new Donate();

        private Donate()
            : base("Donate", Expansion.Empires, coinCost: 0, debtCost:8)
        {
        }
    }

    public class Ritual
        : Event
    {
        public static Ritual card = new Ritual();

        private Ritual()
            : base("Ritual", Expansion.Empires, coinCost: 4)
        {
        }
    }    

    public class SaltTheEarth
        : Event
    {
        public static SaltTheEarth card = new SaltTheEarth();

        private SaltTheEarth()
            : base("Salt The Earth", Expansion.Empires, coinCost: 4)
        {
        }
    }

    public class Tax
        : Event
    {
        public static Tax card = new Tax();

        private Tax()
            : base("Tax", Expansion.Empires, coinCost:2)
        {
        }
    }

    public class Triumph
        : Event
    {
        public static Triumph card = new Triumph();

        private Triumph()
            : base("Triumph", Expansion.Empires, coinCost: 5)
        {
        }
    }

    public class Wedding
        : Event
    {
        public static Wedding card = new Wedding();

        private Wedding()
            : base("Wedding", Expansion.Empires, coinCost: 4, debtCost:3)
        {
        }
    }

    public class Windfall
        : Event
    {
        public static Windfall card = new Windfall();

        private Windfall()
            : base("Windfall", Expansion.Empires, coinCost: 5)
        {
        }
    }

    public class Aqueduct
        : Landmark
    {
        public static Aqueduct card = new Aqueduct();

        private Aqueduct()
            : base("Aqueduct", Expansion.Empires)
        {
        }
    }

    public class Arena
        : Landmark
    {
        public static Arena card = new Arena();

        private Arena()
            : base("Arena", Expansion.Empires)
        {
        }
    }

    public class BanditFort
        : Landmark
    {
        public static BanditFort card = new BanditFort();

        private BanditFort()
            : base("Bandit Fort", Expansion.Empires)
        {
        }
    }

    public class Basilica
        : Landmark
    {
        public static Basilica card = new Basilica();

        private Basilica()
            : base("Basilica", Expansion.Empires)
        {
        }
    }

    public class Baths
        : Landmark
    {
        public static Baths card = new Baths();

        private Baths()
            : base("Baths", Expansion.Empires)
        {
        }
    }

    public class BattleField
        : Landmark
    {
        public static BattleField card = new BattleField();

        private BattleField()
            : base("BattleField", Expansion.Empires)
        {
        }
    }

    public class Colonnade
        : Landmark
    {
        public static Colonnade card = new Colonnade();

        private Colonnade()
            : base("Colonnade", Expansion.Empires)
        {
        }
    }

    public class DefiledShrine
        : Landmark
    {
        public static DefiledShrine card = new DefiledShrine();

        private DefiledShrine()
            : base("Defiled Shrine", Expansion.Empires)
        {
        }
    }

    public class Fountain
        : Landmark
    {
        public static Fountain card = new Fountain();

        private Fountain()
            : base("Fountain", Expansion.Empires)
        {
        }
    }

    public class Keep
        : Landmark
    {
        public static Keep card = new Keep();

        private Keep()
            : base("Keep", Expansion.Empires)
        {
        }
    }

    public class Labyrinth
        : Landmark
    {
        public static Labyrinth card = new Labyrinth();

        private Labyrinth()
            : base("Labyrinth", Expansion.Empires)
        {
        }
    }

    public class MountainPass
        : Landmark
    {
        public static MountainPass card = new MountainPass();

        private MountainPass()
            : base("MountainPass", Expansion.Empires)
        {
        }
    }

    public class Museum
        : Landmark
    {
        public static Museum card = new Museum();

        private Museum()
            : base("Museum", Expansion.Empires)
        {
        }
    }

    public class Obelisk
        : Landmark
    {
        public static Obelisk card = new Obelisk();

        private Obelisk()
            : base("Obelisk", Expansion.Empires)
        {
        }
    }

    public class Orchard
        : Landmark
    {
        public static Orchard card = new Orchard();

        private Orchard()
            : base("Orchard", Expansion.Empires)
        {
        }
    }

    public class Palace
        : Landmark
    {
        public static Palace card = new Palace();

        private Palace()
            : base("Palace", Expansion.Empires)
        {
        }
    }

    public class Tomb
        : Landmark
    {
        public static Tomb card = new Tomb();

        private Tomb()
            : base("Tomb", Expansion.Empires)
        {
        }
    }

    public class Tower
        : Landmark
    {
        public static Tower card = new Tower();

        private Tower()
            : base("Tower", Expansion.Empires)
        {
        }
    }

    public class TriumphalArch
        : Landmark
    {
        public static TriumphalArch card = new TriumphalArch();

        private TriumphalArch()
            : base("Triumphal Arch", Expansion.Empires)
        {
        }
    }

    public class Wall
        : Landmark
    {
        public static Wall card = new Wall();

        private Wall()
            : base("Wall", Expansion.Empires)
        {
        }
    }

    public class WolfDen
        : Landmark
    {
        public static WolfDen card = new WolfDen();

        private WolfDen()
            : base("Wolf Den", Expansion.Empires)
        {
        }
    }


    //****************************************
    public class Archive
        : Card
    {
        public static Archive card = new Archive();

        private Archive()
            : base("Archive", Expansion.Empires, coinCost: 5, isAction: true, isDuration:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class BustlingVillage
       : Card
    {
        public static BustlingVillage card = new BustlingVillage();

        private BustlingVillage()
            : base("BustlingVillage", Expansion.Empires, coinCost: 5, isAction: true, plusActions: 3, plusCards: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Capital
       : Card
    {
        public static Capital card = new Capital();

        private Capital()
            : base("Capital", Expansion.Empires, coinCost: 5, isTreasure:true)
        {
        }

        public override void DoSpecializedDiscardFromPlay(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Castles
       : Card
    {
        public static Castles card = new Castles();

        private Castles()
            : base("Castles", Expansion.Empires, coinCost: 3)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Catapult
       : Card
    {
        public static Catapult card = new Catapult();

        private Catapult()
            : base("Catapult", Expansion.Empires, coinCost: 3, isAction: true, plusCoins: 1, isAttack: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            var trashedCards = currentPlayer.RequestPlayerTrashCardsFromHand(gameState, 1, isOptional: false);
            if (!trashedCards.Any)
                return;
            var trashedCard = trashedCards.SomeCard();
            bool shouldGainCurse = trashedCard.CurrentCoinCost(currentPlayer) >= 3;
            bool shouldDiscard = trashedCard.isTreasure;

            foreach (var otherPlayer in gameState.players.OtherPlayers)
            {
                if (otherPlayer.IsAffectedByAttacks(gameState))
                {
                    if (shouldGainCurse)
                        otherPlayer.GainCardFromSupply(Cards.Curse, gameState);
                    if (shouldDiscard)
                        otherPlayer.RequestPlayerDiscardDownToCountInHand(gameState, 3);
                }
            }
        }
    }

    public class ChariotRace
       : Card
    {
        public static ChariotRace card = new ChariotRace();

        private ChariotRace()
            : base("Chariot Race", Expansion.Empires, coinCost: 3, isAction: true, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Charm
       : Card
    {
        public static Charm card = new Charm();

        private Charm()
            : base("Charm", Expansion.Empires, coinCost: 5, isTreasure: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class CityQuarter
        : Card
    {
        public static CityQuarter card = new CityQuarter();

        private CityQuarter()
            : base("City Quarter", Expansion.Empires, coinCost: 0, debtCost: 8, plusActions: 2, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealHand();
            int countActionsInHand = currentPlayer.hand.CountWhere(c => c.isAction);
            currentPlayer.DrawAdditionalCardsIntoHand(countActionsInHand, gameState);
        }
    }

    public class Crown
       : Card
    {
        public static Crown card = new Crown();

        private Crown()
            : base("Crown", Expansion.Empires, coinCost: 5, isTreasure: true, isAction:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Emporium
      : Card
    {
        public static Emporium card = new Emporium();

        private Emporium()
            : base("Emporium", Expansion.Empires, coinCost: 5, isAction: true, plusCards: 1, plusActions: 1, plusCoins: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.cardsBeingPlayed.CountWhere(c => c.isAction) >= 5)
                currentPlayer.AddVictoryTokens(2);

            return DeckPlacement.Default;
        }
    }

    public class Encampment
       : Card
    {
        public static Encampment card = new Encampment();

        private Encampment()
            : base("Encampment", Expansion.Empires, coinCost: 2, isAction: true, plusCards: 2, plusActions: 2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.RequestPlayerRevealCardFromHand(c => c.IsType(Cards.Gold) || c.IsType(Cards.Plunder), gameState) == null)
            {
                // TODO
                throw new NotImplementedException();
            }
        }
    }

    public class Enchantress
       : Card
    {
        public static Enchantress card = new Enchantress();

        private Enchantress()
            : base("Enchantress", Expansion.Empires, coinCost: 3, isAction: true, isAttack:true, isDuration:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Engineer
        : Card
    {
        public static Engineer card = new Engineer();

        private Engineer()
            : base("Engineer", Expansion.Empires, coinCost: 0, debtCost: 4, isAction: true)
        {
        }
       
        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerGainCardFromSupply(
                gameState,
                acceptableCard => acceptableCard.CurrentCoinCost(currentPlayer) <= 4 && acceptableCard.potionCost == 0,
                "Card must cost up to 4");

            if (currentPlayer.Actions.ShouldTrashCard(gameState, this))
            {
                currentPlayer.MoveCardFromPlayedAreaToTrash(this, gameState);
                currentPlayer.RequestPlayerGainCardFromSupply(
                    gameState,
                    acceptableCard => acceptableCard.CurrentCoinCost(currentPlayer) <= 4 && acceptableCard.potionCost == 0,
                    "Card must cost up to 4");
            }
        }
    }

    public class FarmersMarket
       : Card
    {
        public static FarmersMarket card = new FarmersMarket();

        private FarmersMarket()
            : base("Farmers' Market", Expansion.Empires, coinCost: 3, isAction: true, isGathering: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Fortune
       : Card
    {
        public static Fortune card = new Fortune();

        private Fortune()
            : base("Fortune", Expansion.Empires, coinCost: 8, debtCost:8, isTreasure:true, plusBuy:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }
    
    public class Forum
       : Card
    {
        public static Forum card = new Forum();

        private Forum()
            : base("Forum", Expansion.Empires, coinCost: 5, isAction: true, plusCards:3, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, 2, isOptional: false);
        }

        public override void DoSpecializedWhenBuy(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.AddBuys(1);
        }
    }

    public class Gladiator
       : Card
    {
        public static Gladiator card = new Gladiator();

        private Gladiator()
            : base("Gladiator", Expansion.Empires, coinCost: 3, isAction: true, plusCoins:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Groundskeeper
       : Card
    {
        public static Groundskeeper card = new Groundskeeper();

        private Groundskeeper()
            : base("Groundskeeper", Expansion.Empires, coinCost: 5, isAction: true, plusCards:1, plusActions:1)
        {
            this.doSpecializedActionOnGainWhileInPlay = Groundskeeper.DoSpecializedActionOnGainWhileInPlay;
        }

        new private static DeckPlacement DoSpecializedActionOnGainWhileInPlay(PlayerState currentPlayer, GameState gameState, Card card)
        {
            if (card.isVictory)
                currentPlayer.AddVictoryTokens(1);

            return DeckPlacement.Default;
        }
    }

    public class Legionary
       : Card
    {
        public static Legionary card = new Legionary();

        private Legionary()
            : base("Legionary", Expansion.Empires, coinCost: 5, isAction: true, isAttack:true, plusCoins:3)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }

        public override void DoSpecializedAttack(PlayerState currentPlayer, PlayerState otherPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Overlord
       : Card
    {
        public static Overlord card = new Overlord();

        private Overlord()
            : base("Overlord", Expansion.Empires, coinCost: 0, debtCost:8, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Patrician
       : Card
    {
        public static Patrician card = new Patrician();

        private Patrician()
            : base("Patrician", Expansion.Empires, coinCost: 2, isAction: true, plusCards: 1, plusActions: 1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card revealedCard = currentPlayer.DrawAndRevealOneCardFromDeck(gameState);
            if (revealedCard.CurrentCoinCost(currentPlayer) >= 5)
            {
                currentPlayer.MoveAllRevealedCardsToHand();
            }
            else
            {
                currentPlayer.MoveRevealedCardToTopOfDeck();
            }
        }
    }

    public class Plunder
       : Card
    {
        public static Plunder card = new Plunder();

        private Plunder()
            : base("Plunder", Expansion.Empires, coinCost: 5, isTreasure: true, plusCoins: 2, plusVictoryToken: 1)
        {
        }
    }

    public class Rocks
        : Card
    {
        public static Rocks card = new Rocks();

        private Rocks()
            : base("Rocks", Expansion.Empires, coinCost: 4, isTreasure: true, plusCoins:1)
        {
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            GainSilver(currentPlayer, gameState);
            return DeckPlacement.Default;
        }

        private static void GainSilver(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply(
                Cards.Silver,
                gameState,
                currentPlayer.PlayPhase == PlayPhase.Buy ? DeckPlacement.Deck : DeckPlacement.Hand);
        }

        public override bool DoSpecializedTrash(PlayerState selfPlayer, GameState gameState)
        {
            GainSilver(selfPlayer, gameState);
            return base.DoSpecializedTrash(selfPlayer, gameState);
        }
    }

    public class RoyalBlacksmith
       : Card
    {
        public static RoyalBlacksmith card = new RoyalBlacksmith();

        private RoyalBlacksmith()
            : base("Royal Blacksmith", Expansion.Empires, coinCost: 0, debtCost:8, isAction: true, plusCards:5)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RevealHand();
            while (currentPlayer.hand.Contains(Cards.Copper))
            {
                currentPlayer.DiscardCard(Cards.Copper, gameState, source:DeckPlacement.Hand);
            }
        }
    }

    public class Sacrifice
        : Card
    {
        public static Sacrifice card = new Sacrifice();

        private Sacrifice()
            : base("Sacrifice", Expansion.Empires, coinCost: 4, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card trashedCard = currentPlayer.RequestPlayerTrashCardFromHand(gameState, card => true, isOptional: false);
            if (trashedCard.isAction)
            {
                currentPlayer.DrawAdditionalCardsIntoHand(2, gameState);
                currentPlayer.AddActions(2);
            }
            if (trashedCard.isTreasure)
                currentPlayer.AddCoins(2);
            if (trashedCard.isVictory)
                currentPlayer.AddVictoryTokens(2);
        }
    }

    public class Settlers
       : Card
    {
        public static Settlers card = new Settlers();

        private Settlers()
            : base("Settlers", Expansion.Empires, coinCost: 2, isAction: true, plusCards:1, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Temple
       : Card
    {
        public static Temple card = new Temple();

        private Temple()
            : base("Temple", Expansion.Empires, coinCost: 4, isAction: true, isGathering:true, plusVictoryToken:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Villa
       : Card
    {
        public static Villa card = new Villa();

        private Villa()
            : base("Villa", Expansion.Empires, coinCost: 4, isAction: true, plusActions:2, plusBuy:1, plusCoins:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class WildHunt
       : Card
    {
        public static WildHunt card = new WildHunt();

        private WildHunt()
            : base("Wild Hunt", Expansion.Empires, coinCost: 5, isAction: true, isGathering: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }
}
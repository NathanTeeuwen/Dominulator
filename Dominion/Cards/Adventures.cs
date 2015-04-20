using System;

namespace Dominion.CardTypes
{
    using Dominion;

    public class Event 
        : Card
    {
        protected Event(
            string name,
            Expansion expansion,
            string pluralName = null)
            : base(name:name, expansion:expansion, coinCost:0, isEvent:true, pluralName:null)
        {

        }
    }

    public class Ball
        : Event
    {
        public static Ball card = new Ball();

        private Ball()
            : base("Ball", Expansion.Adventures)
        {
        }
    }

    public class Bonfire
        : Event
    {
        public static Bonfire card = new Bonfire();

        private Bonfire()
            : base("Bonfire", Expansion.Adventures)
        {
        }
    }

    public class Ferry
        : Event
    {
        public static Ferry card = new Ferry();

        private Ferry()
            : base("Ferry", Expansion.Adventures)
        {
        }
    }

    public class PathFinding
        : Event
    {
        public static PathFinding card = new PathFinding();

        private PathFinding()
            : base("Path Finding", Expansion.Adventures)
        {
        }
    }

    public class Pilgrimage
        : Event
    {
        public static Pilgrimage card = new Pilgrimage();

        private Pilgrimage()
            : base("Pilgrimage", Expansion.Adventures)
        {
        }
    }

    public class Plan
        : Event
    {
        public static Plan card = new Plan();

        private Plan()
            : base("Plan", Expansion.Adventures)
        {
        }
    }

    public class Raid
        : Event
    {
        public static Raid card = new Raid();

        private Raid()
            : base("Raid", Expansion.Adventures)
        {
        }
    }

    public class Save
        : Event
    {
        public static Save card = new Save();

        private Save()
            : base("Save", Expansion.Adventures)
        {
        }
    }

    public class Seaway
        : Event
    {
        public static Seaway card = new Seaway();

        private Seaway()
            : base("Seaway", Expansion.Adventures)
        {
        }
    }


    public class Training
        : Event
    {
        public static Training card = new Training();

        private Training()
            : base("Training", Expansion.Adventures)
        {
        }
    }

    public class TravellingFair
        : Event
    {
        public static TravellingFair card = new TravellingFair();

        private TravellingFair()
            : base("Travelling Fair", Expansion.Adventures)
        {
        }
    }

    //****************************************


    public class Artificier
        : Card
    {
        public static Artificier card = new Artificier();

        private Artificier()
            : base("Artificier", Expansion.Adventures, coinCost: 5, plusCoins: 1, plusCards:1, plusActions:1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }        
    }

    public class BridgeToll
       : Card
    {
        public static BridgeToll card = new BridgeToll();

        private BridgeToll()
            : base("BridgeToll", Expansion.Adventures, coinCost: 5, plusBuy:1, isAction: true, isDuration:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class CaravanGuard
       : Card
    {
        public static CaravanGuard card = new CaravanGuard();

        private CaravanGuard()
            : base("Caravan Guard", Expansion.Adventures, coinCost: 3, isAction: true, isDuration: true, isReaction:true, plusCards:1, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Champion
       : Card
    {
        public static Champion card = new Champion();

        private Champion()
            : base("Champion", Expansion.Adventures, coinCost: 6, isAction: true, isDuration: true, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Disciple
       : Card
    {
        public static Disciple card = new Disciple();

        private Disciple()
            : base("Disciple", Expansion.Adventures, coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class DistantLands
       : Card
    {
        public static DistantLands card = new DistantLands();

        private DistantLands()
            : base("Distant Lands", Expansion.Adventures, coinCost: 5, isAction: true, isReserve:true, victoryPoints:delegate(PlayerState player){ throw new NotImplementedException(); } )
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Dungeon
       : Card
    {
        public static Dungeon card = new Dungeon();

        private Dungeon()
            : base("Dungeon", Expansion.Adventures, coinCost: 3, isAction: true, isDuration:true, plusActions:1)
        {
        }
        
        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Fugitive
       : Card
    {
        public static Fugitive card = new Fugitive();

        private Fugitive()
            : base("Fugitive", Expansion.Adventures, coinCost: 4, isAction: true, plusCards:2, plusActions:1)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Gear
       : Card
    {
        public static Gear card = new Gear();

        private Gear()
            : base("Gear", Expansion.Adventures, coinCost: 3, isAction: true, isDuration:true, plusCards:2)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }


    public class Hero
        : Card
    {
        public static Hero card = new Hero();

        private Hero()
            : base("Hero", Expansion.Adventures, coinCost: 5, plusCoins: 2, isAction: true, isTraveller:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.RequestPlayerGainCardFromSupply(gameState, card => card.isTreasure, "Gain a Treasure");
        }

        public override void DoSpecializedDiscardFromPlay(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class LostCity
        : Card
    {
        public static LostCity card = new LostCity();

        private LostCity()
            : base("Lost City", Expansion.Adventures, coinCost: 5, plusCards: 2, plusActions: 2, isAction: true)
        {
        }

        public override DeckPlacement DoSpecializedWhenGain(PlayerState currentPlayer, GameState gameState)
        {
            foreach (var player in gameState.players.OtherPlayers)
                player.DrawOneCardIntoHand(gameState);

            return base.DoSpecializedWhenGain(currentPlayer, gameState);
        }
    }

    public class Magpie
        : Card
    {
        public static Magpie card = new Magpie();

        private Magpie()
            : base("Magpie", Expansion.Adventures, coinCost: 4, plusCards: 1, plusActions: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            Card revealedCard = currentPlayer.DrawAndRevealOneCardFromDeck(gameState);
            if (revealedCard.isTreasure)
            {
                currentPlayer.MoveAllRevealedCardsToHand();
            }
            else
            {
                currentPlayer.MoveRevealedCardToTopOfDeck();
            }

            if (revealedCard.isAction || revealedCard.isVictory)
            {
                currentPlayer.GainCardFromSupply(Cards.Magpie, gameState);
            }            
        }
    }

    public class Miser
       : Card
    {
        public static Miser card = new Miser();

        private Miser()
            : base("Miser", Expansion.Adventures, coinCost: 4, isAction: true)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Page
       : Card
    {
        public static Page card = new Page();

        private Page()
            : base("Page", Expansion.Adventures, coinCost: 2, isAction: true, plusCards:1, plusActions:1)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Peasant
       : Card
    {
        public static Peasant card = new Peasant();

        private Peasant()
            : base("Peasant", Expansion.Adventures, coinCost: 2, isAction: true, plusBuy:1, plusCoins:1)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Port
       : Card
    {
        public static Port card = new Port();

        private Port()
            : base("Port", Expansion.Adventures, coinCost: 4, isAction: true, plusCards:1, plusActions:2)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Ranger
       : Card
    {
        public static Ranger card = new Ranger();

        private Ranger()
            : base("Ranger", Expansion.Adventures, coinCost: 4, isAction: true, plusBuy:1)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class RatCatcher
       : Card
    {
        public static RatCatcher card = new RatCatcher();

        private RatCatcher()
            : base("Rat Catcher", Expansion.Adventures, coinCost: 2, isReserve:true, isAction: true, plusCards:1, plusActions:1)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Raze
       : Card
    {
        public static Raze card = new Raze();

        private Raze()
            : base("Raze", Expansion.Adventures, coinCost: 2, isAction: true, plusActions:1)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Relic
       : Card
    {
        public static Relic card = new Relic();

        private Relic()
            : base("Relic", Expansion.Adventures, coinCost: 5, isTreasure:true, isAttack:true, plusCoins:2)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }
    
    public class RoyalCarriage
       : Card
    {
        public static RoyalCarriage card = new RoyalCarriage();

        private RoyalCarriage()
            : base("Royal Carriage", Expansion.Adventures, coinCost: 5, isReserve:true, plusActions:1, isAction:true)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Solider
       : Card
    {
        public static Solider card = new Solider();

        private Solider()
            : base("Solider", Expansion.Adventures, coinCost: 3, isAction:true, plusCoins:2)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }


    public class Storyteller
       : Card
    {
        public static Storyteller card = new Storyteller();

        private Storyteller()
            : base("Storyteller", Expansion.Adventures, coinCost: 5, plusActions: 1, plusCoins: 1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
           for (int i = 0; i < 3; ++i)
           {
               Card cardPlayed = gameState.DoPlayOneTreasure(currentPlayer);
               if (cardPlayed == null)
                   break;               
           }
           currentPlayer.DrawAdditionalCardsIntoHand(currentPlayer.AvailableCoins, gameState);
           currentPlayer.turnCounters.RemoveCoins(currentPlayer.turnCounters.AvailableCoins);
        }
    }

    public class Teacher
       : Card
    {
        public static Teacher card = new Teacher();

        private Teacher()
            : base("Teacher", Expansion.Adventures, coinCost: 6, isAction:true)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class TreasureHunter
       : Card
    {
        public static TreasureHunter card = new TreasureHunter();

        private TreasureHunter()
            : base("Treasure Hunter", Expansion.Adventures, coinCost: 3, isAction:true, plusActions:1, plusCoins:1)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }
    
    public class TreasureTrove
       : Card
    {
        public static TreasureTrove card = new TreasureTrove();

        private TreasureTrove()
            : base("Treasure Trove", Expansion.Adventures, coinCost: 5, isTreasure:true, plusCoins:2)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Warrior
       : Card
    {
        public static Warrior card = new Warrior();

        private Warrior()
            : base("Warrior", Expansion.Adventures, coinCost: 4, isAction:true, plusCards:2)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class WineMerchant
       : Card
    {
        public static WineMerchant card = new WineMerchant();

        private WineMerchant()
            : base("Wine Merchant", Expansion.Adventures, coinCost: 4, isAction:true, isReserve:true, plusBuy:1, plusCoins:4)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }
}
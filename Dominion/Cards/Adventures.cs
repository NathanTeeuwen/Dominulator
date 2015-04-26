using System;

namespace Dominion.CardTypes
{
    using Dominion;
  
    public class Alms
        : Event
    {
        public static Alms card = new Alms();

        private Alms()
            : base("Alms", Expansion.Adventures)
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

    public class Borrow
        : Event
    {
        public static Borrow card = new Borrow();

        private Borrow()
            : base("Borrow", Expansion.Adventures)
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

    public class Expedition
        : Event
    {
        public static Expedition card = new Expedition();

        private Expedition()
            : base("Expedition", Expansion.Adventures)
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

    public class Inheritance
        : Event
    {
        public static Inheritance card = new Inheritance();

        private Inheritance()
            : base("Inheritance", Expansion.Adventures)
        {
        }
    }

    public class LostArts
        : Event
    {
        public static LostArts card = new LostArts();

        private LostArts()
            : base("Lost Arts", Expansion.Adventures)
        {
        }
    }

    public class Mission
        : Event
    {
        public static Mission card = new Mission();

        private Mission()
            : base("Mission", Expansion.Adventures)
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

    public class Quest
        : Event
    {
        public static Quest card = new Quest();

        private Quest()
            : base("Quest", Expansion.Adventures)
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

    public class ScoutingParty
        : Event
    {
        public static ScoutingParty card = new ScoutingParty();

        private ScoutingParty()
            : base("Scouting Party", Expansion.Adventures)
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

    public class Trade
        : Event
    {
        public static Trade card = new Trade();

        private Trade()
            : base("Trade", Expansion.Adventures)
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
    public class Amulet
        : Card
    {
        public static Amulet card = new Amulet();

        private Amulet()
            : base("Amulet", Expansion.Adventures, coinCost: 3, isAction: true, isDuration:true)
        {
        }
       
        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            PlayerActionChoice actionChoice = currentPlayer.RequestPlayerChooseBetween(
                gameState,
                acceptableChoice => acceptableChoice == PlayerActionChoice.GainCard ||
                                    acceptableChoice == PlayerActionChoice.PlusCoin ||
                                    acceptableChoice == PlayerActionChoice.Trash);

            switch (actionChoice)
            {
                case PlayerActionChoice.GainCard: currentPlayer.GainCardFromSupply(Silver.card, gameState); break;
                case PlayerActionChoice.PlusCoin: currentPlayer.AddCoins(1); break;
                case PlayerActionChoice.Trash: currentPlayer.RequestPlayerTrashCardsFromHand(gameState, 1, false); break;
                default: throw new Exception("Invalid case");
            }
        }
    }

    public class Artificer
        : Card
    {
        public static Artificer card = new Artificer();

        private Artificer()
            : base("Artificer", Expansion.Adventures, coinCost: 5, plusCoins: 1, plusCards:1, plusActions:1, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }        
    }

    public class BridgeTroll
       : Card
    {
        public static BridgeTroll card = new BridgeTroll();

        private BridgeTroll()
            : base("Bridge Troll", Expansion.Adventures, coinCost: 5, plusBuy:1, isAction: true, isDuration:true)
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
            : base("Champion", Expansion.Adventures, coinCost: 6, isAction: true, isDuration: true, plusActions:1, isKingdomCard:false, isInSupply:false)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class CoinOfTheRealm
       : Card
    {
        public static CoinOfTheRealm card = new CoinOfTheRealm();

        private CoinOfTheRealm()
            : base("Coin Of The Realm", Expansion.Adventures, coinCost: 2, isTreasure: true, isReserve:true, plusCoins:1)
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
            : base("Disciple", Expansion.Adventures, coinCost: 5, isAction: true, isKingdomCard:false, isInSupply:false)
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

    public class Duplicate
       : Card
    {
        public static Duplicate card = new Duplicate();

        private Duplicate()
            : base("Duplicate", Expansion.Adventures, coinCost: 4, isAction: true, isReserve: true)
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

        public override void DoSpecializedDurationActionAtBeginningOfTurn(PlayerState currentPlayer, GameState gameState)
        {
            DoNowAndAtStartOfTurn(currentPlayer, gameState);
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            DoNowAndAtStartOfTurn(currentPlayer, gameState);            
        }

        private void DoNowAndAtStartOfTurn(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.DrawAdditionalCardsIntoHand(2, gameState);
            currentPlayer.RequestPlayerDiscardCardsFromHand(gameState, 2, isOptional: false);
        }
    }

    public class Fugitive
       : Card
    {
        public static Fugitive card = new Fugitive();

        private Fugitive()
            : base("Fugitive", Expansion.Adventures, coinCost: 4, isAction: true, plusCards: 2, plusActions: 1, isKingdomCard: false, isInSupply: false)
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

    public class Giant
       : Card
    {
        public static Giant card = new Giant();

        private Giant()
            : base("Giant", Expansion.Adventures, coinCost: 5, isAction: true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Guide
       : Card
    {
        public static Guide card = new Guide();

        private Guide()
            : base("Guide", Expansion.Adventures, coinCost: 3, isAction: true, isReserve: true, plusCards: 1, plusActions:1)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class HauntedWoods
        : Card
    {
        public static HauntedWoods card = new HauntedWoods();

        private HauntedWoods()
            : base("Haunted Woods", Expansion.Adventures, coinCost: 5, isAttack:true, isDuration:true)
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
            : base("Hero", Expansion.Adventures, coinCost: 5, plusCoins: 2, isAction: true, isTraveller: true, isKingdomCard: false, isInSupply: false)
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

    public class Hireling
       : Card
    {
        public static Hireling card = new Hireling();

        private Hireling()
            : base("Hireling", Expansion.Adventures, coinCost: 6, isDuration:true)
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

    public class Messenger
        : Card
    {
        public static Messenger card = new Messenger();

        private Messenger()
            : base("Messenger", Expansion.Adventures, coinCost: 4, isAction: true, plusBuy:1, plusCoins:2)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            if (currentPlayer.actions.ShouldPutDeckInDiscard(gameState))
            {
                currentPlayer.MoveDeckToDiscard(gameState);
            }
        }

        public override void DoSpecializedWhenBuy(PlayerState currentPlayer, GameState gameState)
        {
            // if the first card bought this turn is messenger ...
            if (currentPlayer.CardsBoughtThisTurn.Count == 1)
            {
                Card gainedCard = currentPlayer.RequestPlayerGainCardFromSupply(gameState, card => card.CurrentCoinCost(currentPlayer) <= 4, "Gain a card costing up to 4");
                if (gainedCard != null)
                {
                    foreach(var player in gameState.players.OtherPlayers)
                    {
                        player.GainCardFromSupply(gainedCard, gameState);
                    }
                }
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

        public override void DoSpecializedWhenBuy(PlayerState currentPlayer, GameState gameState)
        {
            currentPlayer.GainCardFromSupply(Cards.Port, gameState);
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
            : base("Ratcatcher", Expansion.Adventures, coinCost: 2, isReserve:true, isAction: true, plusCards:1, plusActions:1)
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
            Card trashedCard = null;
            if (currentPlayer.actions.ShouldTrashCard(gameState, Cards.Raze))
            {
                if (currentPlayer.MoveCardFromPlayToTrash(gameState))
                    trashedCard = Cards.Raze;
            }
            else
            {
                trashedCard = currentPlayer.RequestPlayerTrashCardFromHand(gameState, c => true, isOptional: false);
            }

            if (trashedCard == null)
                return;

            int cardsToLookAt = trashedCard.CurrentCoinCost(currentPlayer);
            if (cardsToLookAt == 0)
                return;

            currentPlayer.LookAtCardsFromDeck(cardsToLookAt, gameState);
            // TODO: ask player to put one card in hand.
            currentPlayer.MoveLookedAtCardsToDiscard(gameState);

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
            : base("Solider", Expansion.Adventures, coinCost: 3, isAction: true, plusCoins: 2, isKingdomCard: false, isInSupply: false)
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

    public class SwampHag
       : Card
    {
        public static SwampHag card = new SwampHag();

        private SwampHag()
            : base("Swamp Hag", Expansion.Adventures, coinCost: 5, isAction: true, isAttack:true, isDuration:true)
        {
        }

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Teacher
       : Card
    {
        public static Teacher card = new Teacher();

        private Teacher()
            : base("Teacher", Expansion.Adventures, coinCost: 6, isAction: true, isKingdomCard: false, isInSupply: false)
        {
        }        

        public override void DoSpecializedAction(PlayerState currentPlayer, GameState gameState)
        {
            throw new NotImplementedException();
        }
    }

    public class Transmogrify
       : Card
    {
        public static Transmogrify card = new Transmogrify();

        private Transmogrify()
            : base("Transmogrify", Expansion.Adventures, coinCost: 4, isAction: true, isReserve:true, plusActions:1)
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
            : base("Treasure Hunter", Expansion.Adventures, coinCost: 3, isAction: true, plusActions: 1, plusCoins: 1, isKingdomCard: false, isInSupply: false)
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
            currentPlayer.GainCardFromSupply(Cards.Gold, gameState);
            currentPlayer.GainCardFromSupply(Cards.Copper, gameState);
        }
    }

    public class Warrior
       : Card
    {
        public static Warrior card = new Warrior();

        private Warrior()
            : base("Warrior", Expansion.Adventures, coinCost: 4, isAction: true, plusCards: 2, isKingdomCard: false, isInSupply: false)
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
using Dominion;
using Dominion.Strategy;
using Dominion.Strategy.DefaultPlayRules.Cards;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules
{    
    static class DefaultResponses
    {
        public static MapOfCards<IPlayerAction> GetCardResponses(PlayerAction playerAction)
        {
            var result = new MapOfCards<IPlayerAction>();

            result[Dominion.Cards.Ambassador] = new AmbassadorAlwaysReturnBestTrash(playerAction);
            result[Dominion.Cards.Alchemist] = new Alchemist(playerAction);
            result[Dominion.Cards.BandOfMisfits] = new BandOfMisfits(playerAction);
            result[Dominion.Cards.Cartographer] = new Cartographer(playerAction);
            result[Dominion.Cards.Catacombs] = new Catacombs(playerAction);
            result[Dominion.Cards.Chancellor] = new Chancellor(playerAction);
            result[Dominion.Cards.Count] = new Count(playerAction);
            result[Dominion.Cards.Golem] = new Golem(playerAction);
            result[Dominion.Cards.HorseTraders] = new HorseTraders(playerAction);
            result[Dominion.Cards.IllGottenGains] = new IllGottenGainsAlwaysGainCopper(playerAction);
            result[Dominion.Cards.Library] = new Library(playerAction);
            result[Dominion.Cards.MarketSquare] = new MarketSquare(playerAction);
            result[Dominion.Cards.Mystic] = new MysticAndWishingWell(playerAction);
            result[Dominion.Cards.Nobles] = new Nobles(playerAction);
            result[Dominion.Cards.Rebuild] = new Rebuild(playerAction);
            result[Dominion.Cards.ScryingPool] = new ScryingPool(playerAction);
            result[Dominion.Cards.Scheme] = new Scheme(playerAction);
            result[Dominion.Cards.Treasury] = new Treasury(playerAction);
            result[Dominion.Cards.Trader] = new Trader(playerAction);
            result[Dominion.Cards.Watchtower] = new Watchtower(playerAction);
            result[Dominion.Cards.WalledVillage] = new WalledVillage(playerAction);
            result[Dominion.Cards.WishingWell] = result[Dominion.Cards.Mystic];

            return result;
        }

        public static MapOfCards<GameStatePlayerActionPredicate> GetCardShouldPlayDefaults(PlayerAction playerAction)
        {
            var result = new MapOfCards<GameStatePlayerActionPredicate>();

            result[Dominion.Cards.Apprentice] = Strategy.HasCardToTrashInHand;
            result[Dominion.Cards.Remodel] = Strategy.HasCardToTrashInHand;
            result[Dominion.Cards.Salvager] = Strategy.HasCardToTrashInHand;
            result[Dominion.Cards.Bishop] = Strategy.HasCardToTrashInHand;
            result[Dominion.Cards.Lookout] = Lookout.ShouldPlay;

            return result;
        }
    }
}

using Dominion;
using System;
using System.Linq;

namespace Program.DefaultStrategies
{    
    static class DefaultResponses
    {
        public static MapOfCards<IPlayerAction> GetCardResponses(PlayerAction playerAction)
        {
            var result = new MapOfCards<IPlayerAction>();

            result[Cards.Ambassador] = new Ambassador(playerAction);
            result[Cards.Alchemist] = new Alchemist(playerAction);
            result[Cards.BandOfMisfits] = new BandOfMisfits(playerAction);
            result[Cards.Cartographer] = new Cartographer(playerAction);
            result[Cards.Catacombs] = new Catacombs(playerAction);
            result[Cards.Chancellor] = new Chancellor(playerAction);
            result[Cards.Count] = new Count(playerAction);
            result[Cards.Golem] = new Golem(playerAction);
            result[Cards.HorseTraders] = new HorseTraders(playerAction);
            result[Cards.IllGottenGains] = new IllGottenGainsAlwaysGainCopper(playerAction);
            result[Cards.Library] = new Library(playerAction);
            result[Cards.MarketSquare] = new MarketSquare(playerAction);
            result[Cards.Mystic] = new MysticAndWishingWell(playerAction);
            result[Cards.Nobles] = new Nobles(playerAction);
            result[Cards.Rebuild] = new Rebuild(playerAction);
            result[Cards.ScryingPool] = new ScryingPool(playerAction);
            result[Cards.Scheme] = new Scheme(playerAction);
            result[Cards.Treasury] = new Treasury(playerAction);
            result[Cards.Trader] = new Trader(playerAction);
            result[Cards.Watchtower] = new Watchtower(playerAction);
            result[Cards.WalledVillage] = new WalledVillage(playerAction);
            result[Cards.WishingWell] = result[Cards.Mystic];

            return result;
        }

        public static MapOfCards<GameStatePlayerActionPredicate> GetCardShouldPlayDefaults(PlayerAction playerAction)
        {
            var result = new MapOfCards<GameStatePlayerActionPredicate>();

            result[Cards.Remodel] = Strategies.HasCardToTrashInHand;
            result[Cards.Salvager] = Strategies.HasCardToTrashInHand;
            result[Cards.Bishop] = Strategies.HasCardToTrashInHand;
            result[Cards.Lookout] = Lookout.ShouldPlay;

            return result;
        }
    }
}

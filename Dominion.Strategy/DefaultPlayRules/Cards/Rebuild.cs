using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Rebuild
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Rebuild(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return playerAction.DefaultGetCardFromSupplyToGain(gameState, acceptableCard, isOptional);
        }

        public override Card NameACard(GameState gameState)
        {
            PlayerState self = gameState.Self;

            int pointLead = Strategy.PlayersPointLead(gameState);

            //Name Duchy
            if (Strategy.CountOfPile(Dominion.Cards.Duchy, gameState) > 0 &&
                Strategy.CountInDeckAndDiscard(Dominion.Cards.Estate, gameState) > 0 &&
                (Strategy.CountInDeckAndDiscard(Dominion.Cards.Province, gameState) == 0 ||
                    Strategy.CountInDeck(Dominion.Cards.Province, gameState) == 0 &&
                    Strategy.CountInDeck(Dominion.Cards.Duchy, gameState) > 0 &&
                    Strategy.CountInDeck(Dominion.Cards.Estate, gameState) > 0))
            {
                return Dominion.Cards.Duchy;
            }

            //Name Province if you are ensured of gaining a Province
            if (Strategy.CountInDeck(Dominion.Cards.Estate, gameState) == 0 &&
                Strategy.CountInDeck(Dominion.Cards.Province, gameState) >= 0 &&
                Strategy.CountInDeck(Dominion.Cards.Duchy, gameState) > 0)
            {
                return Dominion.Cards.Province;
            }

            //Name Province if you are ensured of gaining a Province
            if (Strategy.CountInDeckAndDiscard(Dominion.Cards.Estate, gameState) == 0
                && Strategy.CountInDeckAndDiscard(Dominion.Cards.Province, gameState) >= 0
                && Strategy.CountInDeckAndDiscard(Dominion.Cards.Duchy, gameState) > 0)
            {
                return Dominion.Cards.Province;
            }

            //Name Estate if you can end it with a win                    
            if (Strategy.CountInHand(Dominion.Cards.Rebuild, gameState) + 1 >= Strategy.CountOfPile(Dominion.Cards.Province, gameState) &&
                pointLead > 0)
            {
                return Dominion.Cards.Estate;
            }

            //Name Estate if it's the only thing left in your draw pile and the Duchies are gone
            if (Strategy.CountOfPile(Dominion.Cards.Duchy, gameState) == 0 &&
                Strategy.CountInDeck(Dominion.Cards.Province, gameState) == 0 &&
                Strategy.CountInDeck(Dominion.Cards.Estate, gameState) > 0)
            {
                return Dominion.Cards.Estate;
            }

            //Name Province if Duchy is in Draw and Draw contains more P than E
            if (Strategy.CountOfPile(Dominion.Cards.Duchy, gameState) == 0 &&
                Strategy.CountInDeck(Dominion.Cards.Duchy, gameState) > 0 &&
                Strategy.CountInDeck(Dominion.Cards.Province, gameState) > Strategy.CountInDeck(Dominion.Cards.Estate, gameState))
            {
                return Dominion.Cards.Province;
            }

            //Name Estate if you're ahead and both P and E are left in draw
            if (Strategy.CountOfPile(Dominion.Cards.Duchy, gameState) == 0 &&
                Strategy.CountInDeck(Dominion.Cards.Province, gameState) > 0 &&
                Strategy.CountInDeck(Dominion.Cards.Estate, gameState) > 0 &&
                pointLead > 2)
            {
                return Dominion.Cards.Estate;
            }

            //Name Estate over Province if you're way ahead
            if (Strategy.CountOfPile(Dominion.Cards.Duchy, gameState) == 0 &&
                Strategy.CountInDeckAndDiscard(Dominion.Cards.Province, gameState) > 0 &&
                Strategy.CountInDeckAndDiscard(Dominion.Cards.Duchy, gameState) < 3 &&
                Strategy.CountInDeckAndDiscard(Dominion.Cards.Estate, gameState) > 0 &&
                pointLead > 4)
            {
                return Dominion.Cards.Estate;
            }

            //Province -> Province when ahead without any Duchies left
            if (Strategy.CountOfPile(Dominion.Cards.Duchy, gameState) == 0 &&
                Strategy.CountAllOwned(Dominion.Cards.Duchy, gameState) == 0 &&
                pointLead > 0)
            {
                return Dominion.Cards.Estate;
            }

            //Province -> Province when ahead without any Duchies not in hand
            if (Strategy.CountOfPile(Dominion.Cards.Duchy, gameState) == 0 &&
                Strategy.CountInDeckAndDiscard(Dominion.Cards.Duchy, gameState) == 0 &&
                Strategy.CountInDeckAndDiscard(Dominion.Cards.Province, gameState) > 0 &&
                pointLead > 2)
            {
                return Dominion.Cards.Estate;
            }

            if (Strategy.CountInDeckAndDiscard(Dominion.Cards.Province, gameState) > 0)
            {
                return Dominion.Cards.Province;
            }

            return Dominion.Cards.Estate;
        }
    }
}
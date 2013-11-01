using Dominion;
using System;
using System.Linq;

namespace Program.DefaultStrategies
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
            if (Strategy.CountOfPile(Cards.Duchy, gameState) > 0 &&
                Strategy.CountInDeckAndDiscard(Cards.Estate, gameState) > 0 &&
                (Strategy.CountInDeckAndDiscard(Cards.Province, gameState) == 0 ||
                    Strategy.CountInDeck(Cards.Province, gameState) == 0 &&
                    Strategy.CountInDeck(Cards.Duchy, gameState) > 0 &&
                    Strategy.CountInDeck(Cards.Estate, gameState) > 0))
            {
                return Cards.Duchy;
            }

            //Name Province if you are ensured of gaining a Province
            if (Strategy.CountInDeck(Cards.Estate, gameState) == 0 &&
                Strategy.CountInDeck(Cards.Province, gameState) >= 0 &&
                Strategy.CountInDeck(Cards.Duchy, gameState) > 0)
            {
                return Cards.Province;
            }

            //Name Province if you are ensured of gaining a Province
            if (Strategy.CountInDeckAndDiscard(Cards.Estate, gameState) == 0
                && Strategy.CountInDeckAndDiscard(Cards.Province, gameState) >= 0
                && Strategy.CountInDeckAndDiscard(Cards.Duchy, gameState) > 0)
            {
                return Cards.Province;
            }

            //Name Estate if you can end it with a win                    
            if (Strategy.CountInHand(Cards.Rebuild, gameState) + 1 >= Strategy.CountOfPile(Cards.Province, gameState) &&
                pointLead > 0)
            {
                return Cards.Estate;
            }

            //Name Estate if it's the only thing left in your draw pile and the Duchies are gone
            if (Strategy.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategy.CountInDeck(Cards.Province, gameState) == 0 &&
                Strategy.CountInDeck(Cards.Estate, gameState) > 0)
            {
                return Cards.Estate;
            }

            //Name Province if Duchy is in Draw and Draw contains more P than E
            if (Strategy.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategy.CountInDeck(Cards.Duchy, gameState) > 0 &&
                Strategy.CountInDeck(Cards.Province, gameState) > Strategy.CountInDeck(Cards.Estate, gameState))
            {
                return Cards.Province;
            }

            //Name Estate if you're ahead and both P and E are left in draw
            if (Strategy.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategy.CountInDeck(Cards.Province, gameState) > 0 &&
                Strategy.CountInDeck(Cards.Estate, gameState) > 0 &&
                pointLead > 2)
            {
                return Cards.Estate;
            }

            //Name Estate over Province if you're way ahead
            if (Strategy.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategy.CountInDeckAndDiscard(Cards.Province, gameState) > 0 &&
                Strategy.CountInDeckAndDiscard(Cards.Duchy, gameState) < 3 &&
                Strategy.CountInDeckAndDiscard(Cards.Estate, gameState) > 0 &&
                pointLead > 4)
            {
                return Cards.Estate;
            }

            //Province -> Province when ahead without any Duchies left
            if (Strategy.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategy.CountAllOwned(Cards.Duchy, gameState) == 0 &&
                pointLead > 0)
            {
                return Cards.Estate;
            }

            //Province -> Province when ahead without any Duchies not in hand
            if (Strategy.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategy.CountInDeckAndDiscard(Cards.Duchy, gameState) == 0 &&
                Strategy.CountInDeckAndDiscard(Cards.Province, gameState) > 0 &&
                pointLead > 2)
            {
                return Cards.Estate;
            }

            if (Strategy.CountInDeckAndDiscard(Cards.Province, gameState) > 0)
            {
                return Cards.Province;
            }

            return Cards.Estate;
        }
    }
}
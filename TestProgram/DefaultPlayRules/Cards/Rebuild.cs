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

            int pointLead = Strategies.PlayersPointLead(gameState);

            //Name Duchy
            if (Strategies.CountOfPile(Cards.Duchy, gameState) > 0 &&
                Strategies.CountInDeckAndDiscard(Cards.Estate, gameState) > 0 &&
                (Strategies.CountInDeckAndDiscard(Cards.Province, gameState) == 0 ||
                    Strategies.CountInDeck(Cards.Province, gameState) == 0 &&
                    Strategies.CountInDeck(Cards.Duchy, gameState) > 0 &&
                    Strategies.CountInDeck(Cards.Estate, gameState) > 0))
            {
                return Cards.Duchy;
            }

            //Name Province if you are ensured of gaining a Province
            if (Strategies.CountInDeck(Cards.Estate, gameState) == 0 &&
                Strategies.CountInDeck(Cards.Province, gameState) >= 0 &&
                Strategies.CountInDeck(Cards.Duchy, gameState) > 0)
            {
                return Cards.Province;
            }

            //Name Province if you are ensured of gaining a Province
            if (Strategies.CountInDeckAndDiscard(Cards.Estate, gameState) == 0
                && Strategies.CountInDeckAndDiscard(Cards.Province, gameState) >= 0
                && Strategies.CountInDeckAndDiscard(Cards.Duchy, gameState) > 0)
            {
                return Cards.Province;
            }

            //Name Estate if you can end it with a win                    
            if (Strategies.CountInHand(Cards.Rebuild, gameState) + 1 >= Strategies.CountOfPile(Cards.Province, gameState) &&
                pointLead > 0)
            {
                return Cards.Estate;
            }

            //Name Estate if it's the only thing left in your draw pile and the Duchies are gone
            if (Strategies.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategies.CountInDeck(Cards.Province, gameState) == 0 &&
                Strategies.CountInDeck(Cards.Estate, gameState) > 0)
            {
                return Cards.Estate;
            }

            //Name Province if Duchy is in Draw and Draw contains more P than E
            if (Strategies.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategies.CountInDeck(Cards.Duchy, gameState) > 0 &&
                Strategies.CountInDeck(Cards.Province, gameState) > Strategies.CountInDeck(Cards.Estate, gameState))
            {
                return Cards.Province;
            }

            //Name Estate if you're ahead and both P and E are left in draw
            if (Strategies.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategies.CountInDeck(Cards.Province, gameState) > 0 &&
                Strategies.CountInDeck(Cards.Estate, gameState) > 0 &&
                pointLead > 2)
            {
                return Cards.Estate;
            }

            //Name Estate over Province if you're way ahead
            if (Strategies.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategies.CountInDeckAndDiscard(Cards.Province, gameState) > 0 &&
                Strategies.CountInDeckAndDiscard(Cards.Duchy, gameState) < 3 &&
                Strategies.CountInDeckAndDiscard(Cards.Estate, gameState) > 0 &&
                pointLead > 4)
            {
                return Cards.Estate;
            }

            //Province -> Province when ahead without any Duchies left
            if (Strategies.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategies.CountAllOwned(Cards.Duchy, gameState) == 0 &&
                pointLead > 0)
            {
                return Cards.Estate;
            }

            //Province -> Province when ahead without any Duchies not in hand
            if (Strategies.CountOfPile(Cards.Duchy, gameState) == 0 &&
                Strategies.CountInDeckAndDiscard(Cards.Duchy, gameState) == 0 &&
                Strategies.CountInDeckAndDiscard(Cards.Province, gameState) > 0 &&
                pointLead > 2)
            {
                return Cards.Estate;
            }

            if (Strategies.CountInDeckAndDiscard(Cards.Province, gameState) > 0)
            {
                return Cards.Province;
            }

            return Cards.Estate;
        }
    }
}
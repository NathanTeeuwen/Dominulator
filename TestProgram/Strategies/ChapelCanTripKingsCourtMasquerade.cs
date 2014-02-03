using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    // begin flushing out solution for: http://forum.dominionstrategy.com/index.php?topic=462.msg7678#msg7678

    class ChapelCanTripKingsCourtMasquerade
        : Strategy
    {

        public static PlayerAction Player()
        {
            return PlayerCustom("ChapelCanTripKingsCourtMasquerade");
        }

        public static PlayerAction PlayerCustom(string playerName, bool shouldApprentice = false)
        {
            return new PlayerAction(
                        playerName,
                        purchaseOrder: PurchaseOrder(shouldApprentice),
                        actionOrder: ActionOrder());
        }

        private static ICardPicker PurchaseOrder(bool shouldApprentice)
        {
            var openning = new CardPickByPriority(
                          CardAcceptance.For(Cards.IronWorks, 1),
                          CardAcceptance.For(Cards.Chapel, 1));


            var ironworksGains = new CardPickForCondition( 
                gameState => CardBeingPlayedIs(Cards.IronWorks, gameState),
                new CardPickByPriority(
                    CardAcceptance.For(Cards.GreatHall, gameState => !HasChainingDeck(gameState)),
                    CardAcceptance.For(Cards.WorkersVillage)
                ));                               

            return new CardPickConcatenator(
                openning, 
                ironworksGains);
            /*
            return new CardPickByPriority(
                          CardAcceptance.For(Cards.IronWorks, 1),
                          CardAcceptance.For(Cards.Chapel, 1),
                          CardAcceptance.For(Cards.GreatHall, gameState => CardBeingPlayedIs(Cards.IronWorks, gameState) && !HasChainingDeck(gameState)),
                          CardAcceptance.For(Cards.Peddler, 3),
                          CardAcceptance.For(Cards.Militia, gameState => HasChainingDeck(gameState)),
                          CardAcceptance.For(Cards.KingsCourt, gameState => HasChainingDeck(gameState) && CountAllOwned(Cards.KingsCourt, gameState) == 0),
                          CardAcceptance.For(Cards.KingsCourt, gameState => HasChainingDeck(gameState) && CountAllOwned(Cards.KingsCourt, gameState) == 0),
                          CardAcceptance.For(Cards.Peddler),
                          CardAcceptance.For(Cards.WorkersVillage),
                          CardAcceptance.For(Cards.Pawn),                          
                          );*/
        }

        private static bool HasChainingDeck(GameState gameState)
        {
            return CountAllOwned(Cards.Chapel, gameState) +
                   CountAllOwned(Cards.IronWorks, gameState) +
                   CountAllOwned(Cards.Copper, gameState) +
                   CountAllOwned(Cards.Estate, gameState) <= 4;
        }

        private static ICardPicker ActionOrder()
        {
            return new CardPickByPriority(
                 CardAcceptance.For(Cards.GreatHall),
                 CardAcceptance.For(Cards.Peddler),
                 CardAcceptance.For(Cards.WorkersVillage),
                 CardAcceptance.For(Cards.Pawn),
                 CardAcceptance.For(Cards.IronWorks),
                 CardAcceptance.For(Cards.Chapel)
                 );
        }
    }
}

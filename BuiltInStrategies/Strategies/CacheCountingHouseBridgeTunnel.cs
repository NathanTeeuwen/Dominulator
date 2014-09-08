using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{        
    public class CacheCountingHouseBridgeTunnel
        : Strategy 
    {
            
        public static PlayerAction Player()
        {
            return new MyPlayerAction();
        }

        class MyPlayerAction
            : PlayerAction
        {
            public MyPlayerAction()
                : base("CacheCountingHouseBridgeTunnel",                        
                    purchaseOrder: PurchaseOrder(),                        
                    actionOrder: ActionOrder())             
            {
            }              
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(                        
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.CountingHouse, gameState) >= 1),
                        CardAcceptance.For(Cards.Tunnel, gameState => gameState.Self.AvailableBuys > 1 && CountOfPile(Cards.Province, gameState) <= 6),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 6),
                        CardAcceptance.For(Cards.Tunnel, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Cache, gameState => CountAllOwned(Cards.Cache, gameState) < CountAllOwned(Cards.CountingHouse, gameState)),
                        CardAcceptance.For(Cards.CountingHouse),                        
                        CardAcceptance.For(Cards.Bridge, 1),
                        CardAcceptance.For(Cards.Silver),
                        CardAcceptance.For(Cards.Copper));
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.CountingHouse),
                        CardAcceptance.For(Cards.Bridge));
        }       
    }
}
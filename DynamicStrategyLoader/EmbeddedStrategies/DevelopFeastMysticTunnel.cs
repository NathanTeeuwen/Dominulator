using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{    
    public class DevelopFeastMysticTunnel
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
                : base(
                    "DevelopFeastMysticTunnel",                        
                    purchaseOrder: PurchaseOrder(),                    
                    actionOrder: ActionOrder(),
                    trashOrder: TrashOrder())
            {
            }          
        }

        static ICardPicker PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) >= 2),                        
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4 && gameState.CurrentCardBeingPlayed != Cards.Develop ),
                        CardAcceptance.For(Cards.Tunnel, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 3),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Mystic),
                        CardAcceptance.For(Cards.Feast, gameState => CountAllOwned(Cards.Mystic, gameState) < 2),
                        CardAcceptance.For(Cards.Develop, 1),
                        CardAcceptance.For(Cards.Silver));
        }

        static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For(Cards.Mystic),
                CardAcceptance.For(Cards.Develop, gameState => CountOfPile(Cards.Province, gameState) > 6 && ShouldPlayDevelop(gameState)),                
                CardAcceptance.For(Cards.Feast));                    
        }      

        static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(                
                CardAcceptance.For(Cards.Estate, gameState => CountAllOwned(Cards.Province, gameState) == 0),
                CardAcceptance.For(Cards.Feast),
                CardAcceptance.For(Cards.Mystic),                
                CardAcceptance.For(Cards.Copper));
        }     

        static bool ShouldPlayDevelop(GameState gameState)
        {
            return CountInHand(Cards.Estate, gameState) >= 1 ||
                   CountInHand(Cards.Feast, gameState) >= 1 ||                   
                   CountInHand(Cards.Copper, gameState) >= 1 && (gameState.Self.ExpectedCoinValueAtEndOfTurn != 3); 
        }
    }
}

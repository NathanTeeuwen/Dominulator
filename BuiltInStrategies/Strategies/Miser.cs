using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    public class Miser
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
            : base("Miser",
                purchaseOrder: PurchaseOrder())
            {
            }

            public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
            {
                if (acceptableChoice(PlayerActionChoice.PutCopperOnTavernMat))
                {

                    bool wantToBuyAnotherMiser = CountAllOwned(Cards.Miser, gameState) < 2 &&
                          gameState.Self.AvailableCoins < Cards.Miser.CurrentCoinCost(gameState.Self) &&
                          CardTypes.Miser.PlayerMiserValue(gameState.Self) + gameState.Self.AvailableCoins >=  Cards.Miser.CurrentCoinCost(gameState.Self);
               
                    bool preferCountCoins = wantToBuyAnotherMiser;

                    if (!preferCountCoins && CountInHand(Cards.Copper, gameState) > 0)
                        return PlayerActionChoice.PutCopperOnTavernMat;
                }
                
                if (acceptableChoice(PlayerActionChoice.PlusCoinPerCoppperOnTavernMat))
                {
                    return PlayerActionChoice.PlusCoinPerCoppperOnTavernMat;
                }
            
                return base.ChooseBetween(gameState, acceptableChoice);
            }
        }

        private static CardPickByPriority PurchaseOrder()
        {
          return new CardPickByPriority(
                      CardAcceptance.For(Cards.Province),
                      CardAcceptance.For(Cards.Miser, 3),
                      CardAcceptance.For(Cards.WalledVillage),
                      CardAcceptance.For(Cards.Silver, 1),
                      CardAcceptance.For(Cards.Copper, 8));
        }         
    }
}
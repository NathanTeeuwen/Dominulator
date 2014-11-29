using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{    
    public class BigMoneyWithCard
        : Strategy
    {
            
        public static PlayerAction Player(Card card, 
            string playerName = null, 
            int cardCount = 1, 
            int afterSilverCount = 0,                 
            int countGoldBeforeProvince = 3,
            int countRemainingProvinceBeforeDuchy = 4,
            int countRemainingProvinceBeforeEstateOverGold = 1,
            int countRemainingProvinceBeforeEstateOverSilver = 3,
            int afterGoldCount = int.MaxValue,
            bool enablePenultimateProvinceRule = false)
        {
            return new PlayerAction(
                        playerName == null ? "BigMoney" + card.GetType().Name : playerName,                            
                        purchaseOrder: PurchaseOrder(
                            card, 
                            cardCount, 
                            afterSilverCount, 
                            afterGoldCount == int.MaxValue && card.DefaultCoinCost >= Cards.Gold.DefaultCoinCost ? 0 : afterGoldCount,
                            countGoldBeforeProvince,
                            countRemainingProvinceBeforeDuchy,
                            countRemainingProvinceBeforeEstateOverGold,
                            countRemainingProvinceBeforeEstateOverSilver),
                        actionOrder:ActionOrder(card),
                        enablePenultimateProvinceRule:enablePenultimateProvinceRule);
        }

        public static ICardPicker PurchaseOrder(
            Card card, 
            int cardCount, 
            int afterSilverCount, 
            int afterGoldCount, 
            int countGoldBeforeProvince, 
            int countRemainingProvinceBeforeDuchy,
            int countRemainingProvinceBeforeEstateOverGold,
            int countRemainingProvinceBeforeEstateOverSilver)
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) >= countGoldBeforeProvince),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= countRemainingProvinceBeforeDuchy),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= countRemainingProvinceBeforeEstateOverGold),
                        CardAcceptance.For(card, gameState => CountAllOwned(card, gameState) < cardCount && CountAllOwned(Cards.Gold, gameState) >= afterGoldCount),                           
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(card, gameState => CountAllOwned(card, gameState) < cardCount && CountAllOwned(Cards.Silver, gameState) >= afterSilverCount),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 3),
                        CardAcceptance.For(Cards.Potion, 1, gameState => card.potionCost > 0),
                        CardAcceptance.For(Cards.Silver));
        }

        public static ICardPicker ActionOrder(Card card)
        {
            return new CardPickByPriority(
                CardAcceptance.For(card));
        }            
    }           
}

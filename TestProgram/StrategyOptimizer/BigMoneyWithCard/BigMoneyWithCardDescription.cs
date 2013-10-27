using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    class BigMoneyWithCardDescription
    {
        public Card card;
        public int cardCount = 1;
        public int afterSilverCount = 0;          
        public int countGoldBeforeProvince = 3;
        public int countRemainingProvinceBeforeDuchy = 4;
        public int countRemainingProvinceBeforeEstateOverGold = 1;
        public int countRemainingProvinceBeforeEstateOverSilver = 3;

        public BigMoneyWithCardDescription(Card card)
        {
            this.card = card;
        }

        public PlayerAction ToPlayerAction()
        {
            return Strategies.BigMoneyWithCard.Player(
                this.card, 
                "BigMoneyWith" + card.name, 
                this.cardCount, 
                this.afterSilverCount, 
                int.MaxValue,
                countGoldBeforeProvince,
                countRemainingProvinceBeforeDuchy,
                countRemainingProvinceBeforeEstateOverGold,
                countRemainingProvinceBeforeEstateOverSilver);
        }

        public BigMoneyWithCardDescription Clone()
        {
            var result = new BigMoneyWithCardDescription(this.card);            
            result.cardCount = this.cardCount;
            result.afterSilverCount = this.afterSilverCount;
            result.countGoldBeforeProvince = this.countGoldBeforeProvince;
            result.countRemainingProvinceBeforeDuchy = this.countRemainingProvinceBeforeDuchy;
            result.countRemainingProvinceBeforeEstateOverGold = this.countRemainingProvinceBeforeEstateOverGold;
            result.countRemainingProvinceBeforeEstateOverSilver = this.countRemainingProvinceBeforeEstateOverSilver;

            return result;
        }

        public void Write(System.IO.TextWriter textwriter)
        {
            textwriter.Write(
                //"{0}, cardCount: {1}, afterSilverCount: {2}, countGoldBeforeProvince: {3}, countRemainingProvinceBeforeDuchy: {4}, countRemainingProvinceBeforeEstateOverGold: {5}, countRemainingProvinceBeforeEstateOverSilver {6}",
                "{0}, cardCount: {1}, {2}, {3}, {4}, {5}, {6}",
                this.card,
                this.cardCount,
                this.afterSilverCount,
                this.countGoldBeforeProvince,
                this.countRemainingProvinceBeforeDuchy,
                this.countRemainingProvinceBeforeEstateOverGold,
                this.countRemainingProvinceBeforeEstateOverSilver);            
        }

        public double GetScoreVs(PlayerAction action, bool showReport = false)
        {
            return Program.ComparePlayers(
                this.ToPlayerAction(), 
                action, 
                numberOfGames: showReport? 1000 : 33, 
                logGameCount: 0, 
                showCompactScore: showReport, 
                showVerboseScore: false, 
                createHtmlReport: false, 
                shouldParallel: showReport ? true : false);
        }
    }
}

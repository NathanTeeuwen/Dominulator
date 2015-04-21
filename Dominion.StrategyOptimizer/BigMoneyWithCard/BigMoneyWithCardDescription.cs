using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program.GeneticAlgorithm;

namespace Program
{
    class BigMoneyWithCardDescription
        : Parameters
    {
        public Card card;

        public BigMoneyWithCardDescription(Card card, params Parameter[] parameters)
            : base(parameters)
        {
            this.card = card;
        }

        public BigMoneyWithCardDescription(Card card)
            : this( card,
               new Parameter(1, 1, int.MaxValue), // cardCount
               new Parameter(0), // afterSilverCount
               new Parameter(3), // countGoldBeforeProvince
               new Parameter(4), // countRemainingProvinceBeforeDuchy
               new Parameter(1), // countRemainingProvinceBeforeEstateOverGold
               new Parameter(3)  // countRemainingProvinceBeforeEstateOverSilver
               )
        {            
        }

        public PlayerAction ToPlayerAction()
        {
            return Strategies.BigMoneyWithCard.Player(
                this.card, 
                "BigMoneyWith" + card.name,
                cardCount: this.parameters[0].Value,
                afterSilverCount: this.parameters[1].Value,
                countGoldBeforeProvince: this.parameters[2].Value,
                countRemainingProvinceBeforeDuchy: this.parameters[3].Value,
                countRemainingProvinceBeforeEstateOverGold: this.parameters[4].Value,
                countRemainingProvinceBeforeEstateOverSilver: this.parameters[5].Value);
        }

        public BigMoneyWithCardDescription Clone()
        {
            var result = new BigMoneyWithCardDescription(this.card, this.CloneParameters());            

            return result;
        }

        public void Write(System.IO.TextWriter textwriter)
        {
            textwriter.Write(
                //"{0}, cardCount: {1}, afterSilverCount: {2}, countGoldBeforeProvince: {3}, countRemainingProvinceBeforeDuchy: {4}, countRemainingProvinceBeforeEstateOverGold: {5}, countRemainingProvinceBeforeEstateOverSilver {6}",
                "{0}, {1}, {2}, {3}, {4}, {5}, {6}",
                this.card.name,
                this.parameters[0].Value,
                this.parameters[1].Value,
                this.parameters[2].Value,
                this.parameters[3].Value,
                this.parameters[4].Value,
                this.parameters[5].Value);            
        }        

        public double GetScoreVs(PlayerAction action, bool showReport = false)
        {
            var player1Action = this.ToPlayerAction();
            var player2Action = action;

            GameConfigBuilder builder = new GameConfigBuilder();
            builder.SetKingdomCards(player1Action, player1Action);

            var gameConfig = builder.ToGameConfig();
            var rotateWhoStartsFirst = true;
            var numberOfGames = 100;

            var strategyComparison = new Dominion.Data.StrategyComparison(player1Action, player2Action, gameConfig, rotateWhoStartsFirst, numberOfGames);

            var results = strategyComparison.ComparePlayers(
                gameIndex => null,
                gameIndex => null,
                shouldParallel: true,
                gatherStats: false,
                createGameLog: null);

            return results.WinDifference;
        }
    }
}

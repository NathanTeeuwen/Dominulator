using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Dominion;
using Dominion.Strategy;

namespace Program.WebService
{
    public class GetKingdomCardImages
        : ComparisonDescription,
          IRequestWithJsonResponse
    {
        public object GetResponse(WebService service)
        {
            var playerActions = new List<PlayerAction>();
            var player1 = this.Player1Action;
            if (player1 != null)
                playerActions.Add(player1);
            var player2 = this.Player2Action;
            if (player2 != null)
                playerActions.Add(player2);

            var builder = new GameConfigBuilder();
            PlayerAction.SetKingdomCards(builder, playerActions.ToArray());
            return builder.ToGameConfig().kingdomPiles.OrderBy(card => card.DefaultCoinCost).Select(card => GetCardImageName(card)).ToArray();
        }

        private string GetCardImageName(Card card)
        {
            return "cards/" + card.ProgrammaticName + ".jpg";
        }
    }
}
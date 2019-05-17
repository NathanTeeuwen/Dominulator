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
    [Serializable]
    public class GetAvailableKingdomCards
        : IRequestWithJsonResponse
    {
        public string name { get; set; }

        public object GetResponse(WebService service)
        {
            return Cards.AllKingdomCardsList.Select(c => GetCardForJason(c)).ToArray();
        }

        private static object GetCardForJason(Card card)
        {
            var result = new Dictionary<string, object>();

            result["name"] = card.name;
            result["id"] = card.ProgrammaticName;
            result["coin"] = card.DefaultCoinCost;
            result["potion"] = card.potionCost;
            result["expansion"] = card.expansion.ToString();
            result["isAction"] = card.isAction;
            result["isAttack"] = card.isAttack;
            result["isReaction"] = card.isReaction;
            result["isDuration"] = card.isDuration;

            return result;
        }
    }
}
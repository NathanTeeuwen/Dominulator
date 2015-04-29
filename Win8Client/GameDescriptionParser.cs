using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Dominion;

namespace Win8Client
{
    public static class GameDescriptionParser
    {
        const string jsonNameRequiredExpansions = "expansions";
        const string jsonNamePayload = "payload";
        const string jsonNameRating = "rating";

        public static JsonObject ToJson(Dominion.GameDescription gameDescription, int starRating)
        {
            JsonObject root = new Windows.Data.Json.JsonObject();

            root.Add(jsonNamePayload, ToJson(gameDescription));

            JsonArray expansionArray = new JsonArray();
            foreach (var expansion in gameDescription.GetRequiredExpansions())
            {
                expansionArray.Add(JsonValue.CreateStringValue(expansion.ExpansionToString()));
            }
            root.Add(jsonNameRequiredExpansions, expansionArray);

            root.Add(jsonNameRating, JsonValue.CreateNumberValue(starRating));

            return root;
        }

        const string jsonNameUseShelters = "useShelters";
        const string jsonNameUseColonyAndPlatinum = "useColonyAndPlatinum";
        const string jsonNameKingdomPiles = "kingdomPiles";
        const string jsonNameBane = "baneCard";
        const string jsonNameEvents = "events";
        public static JsonObject ToJson(Dominion.GameDescription gameDescription)
        {
            JsonObject root = new Windows.Data.Json.JsonObject();

            root.Add(jsonNameUseShelters, JsonValue.CreateBooleanValue(gameDescription.useShelters));
            root.Add(jsonNameUseColonyAndPlatinum, JsonValue.CreateBooleanValue(gameDescription.useColonyAndPlatinum));
            string banePileName = gameDescription.BanePileName();
            if (banePileName != null)
            {
                root.Add(jsonNameBane, JsonValue.CreateStringValue(banePileName));
            }

            JsonArray kingdomArray = new JsonArray();
            foreach (var cardName in gameDescription.KingdomPileNames())
            {
                kingdomArray.Add(JsonValue.CreateStringValue(cardName));
            }
            root.Add(jsonNameKingdomPiles, kingdomArray);

            JsonArray eventArray = new JsonArray();
            foreach (var cardName in gameDescription.EventNames())
            {
                eventArray.Add(JsonValue.CreateStringValue(cardName));
            }
            root.Add(jsonNameEvents, eventArray);

            return root;
        }        

        public static Dominion.GameDescription FromJson(string jsonString)
        {
            try
            {
                JsonObject root = Windows.Data.Json.JsonObject.Parse(jsonString);

                bool useShelters = root.GetNamedBoolean(jsonNameUseShelters, defaultValue: false);
                bool useColonyAndPlatinum = root.GetNamedBoolean(jsonNameUseColonyAndPlatinum, defaultValue: false);            
                string baneCardName = root.GetNamedString(jsonNameBane, defaultValue: null);
                JsonArray kingdomArray = root.GetNamedArray(jsonNameKingdomPiles);
                string[] kingdomPileNames = kingdomArray.Select(jsonValue => jsonValue.GetString()).ToArray();

                JsonArray eventArray = root.GetNamedArray(jsonNameEvents);
                string[] eventNames = eventArray.Select(jsonValue => jsonValue.GetString()).ToArray();
                
                return new Dominion.GameDescription(kingdomPileNames, eventNames, baneCardName, useShelters, useColonyAndPlatinum);
            }
            catch(System.Exception)
            {
                return null;
            }
        }
    }
}

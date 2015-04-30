using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Dominion;

namespace Win8Client
{

    public class GameDescriptionAndRating
    {
        public GameDescription gameDescription;
        public double rating;
    }


    public static class WebService
    {
        public static JsonObject ToJsonForGetExpansions(AppDataContext appDataContext)
        {
            JsonObject root = new Windows.Data.Json.JsonObject();

            JsonArray expansionArray = new JsonArray();

            foreach (var expansion in appDataContext.Expansions)
            {
                JsonObject expansionObject = new JsonObject();
                expansionObject.Add("name", JsonValue.CreateStringValue(expansion.DominionExpansion.ToProgramaticName()));
                expansionObject.Add("present", JsonValue.CreateBooleanValue(expansion.IsEnabled.Value));
                expansionArray.Add(expansionObject);
            }
            root.Add(jsonNameRequiredExpansions, expansionArray);

            return root;
        }

        const string jsonNameRequiredExpansions = "expansions";
        const string jsonNameDeck = "deck";
        const string jsonNameRating = "rating";

        public static JsonObject ToJson(Dominion.GameDescription gameDescription, int starRating)
        {
            JsonObject root = new Windows.Data.Json.JsonObject();

            root.Add(jsonNameDeck, ToJson(gameDescription));

            JsonArray expansionArray = new JsonArray();
            Dominion.Expansion[] presentExpansions;
            Dominion.Expansion[] missingExpansions;
            gameDescription.GetRequiredExpansions(out presentExpansions, out missingExpansions);

            foreach (var expansion in presentExpansions)
            {
                JsonObject expansionObject = new JsonObject();
                expansionObject.Add("name", JsonValue.CreateStringValue(expansion.ToProgramaticName()));
                expansionObject.Add("present", JsonValue.CreateBooleanValue(true));
                expansionArray.Add(expansionObject);
            }

            foreach (var expansion in missingExpansions)
            {
                JsonObject expansionObject = new JsonObject();
                expansionObject.Add("name", JsonValue.CreateStringValue(expansion.ToProgramaticName()));
                expansionObject.Add("present", JsonValue.CreateBooleanValue(false));
                expansionArray.Add(expansionObject);
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
            string banePileName = gameDescription.BanePileProgrammaticName();
            if (banePileName != null)
            {
                root.Add(jsonNameBane, JsonValue.CreateStringValue(banePileName));
            }

            JsonArray kingdomArray = new JsonArray();
            foreach (var cardName in gameDescription.KingdomPileProgramaticNames())
            {
                kingdomArray.Add(JsonValue.CreateStringValue(cardName));
            }
            root.Add(jsonNameKingdomPiles, kingdomArray);

            JsonArray eventArray = new JsonArray();
            foreach (var cardName in gameDescription.EventProgramaticNames())
            {
                eventArray.Add(JsonValue.CreateStringValue(cardName));
            }
            root.Add(jsonNameEvents, eventArray);

            return root;
        }        

        public static GameDescriptionAndRating GetGameDescriptionFromJson(string jsonString)
        {
            try
            {
                JsonObject root = Windows.Data.Json.JsonObject.Parse(jsonString);

                JsonObject jsonDeck = root.GetNamedObject(jsonNameDeck);


                bool useShelters = jsonDeck.GetNamedBoolean(jsonNameUseShelters, defaultValue: false);
                bool useColonyAndPlatinum = jsonDeck.GetNamedBoolean(jsonNameUseColonyAndPlatinum, defaultValue: false);
                string baneCardName = jsonDeck.GetNamedString(jsonNameBane, defaultValue: "");
                JsonArray kingdomArray = jsonDeck.GetNamedArray(jsonNameKingdomPiles);
                string[] kingdomPileNames = kingdomArray.Select(jsonValue => jsonValue.GetString()).ToArray();

                JsonArray eventArray = jsonDeck.GetNamedArray(jsonNameEvents);
                string[] eventNames = eventArray.Select(jsonValue => jsonValue.GetString()).ToArray();

                double rating = root.GetNamedNumber(jsonNameRating);

                return new GameDescriptionAndRating() 
                {
                    gameDescription = new Dominion.GameDescription(kingdomPileNames, eventNames, baneCardName, useShelters, useColonyAndPlatinum),
                    rating = rating
                };
                    
            }
            catch(System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return null;
            }
        }

        private static string webUrl = "http://1-dot-mystic-planet-93017.appspot.com/dominiondeck";

        public static async Task SendGameConfigToServer(AppDataContext appDataContext)
        {
            GameDescription gameDescription = appDataContext.GetGameConfig().gameDescription;

            JsonObject json = WebService.ToJson(gameDescription, appDataContext.DeckRating.Value);
            string parameter = json.Stringify();

            var fullUrl = new System.Text.StringBuilder();
            fullUrl.Append(WebService.webUrl);
            fullUrl.Append("?action=RECORD&values=");
            fullUrl.Append(parameter.Replace(" ", "%20"));

            try
            {
                using (var client = new Windows.Web.Http.HttpClient())
                using (var request = new Windows.Web.Http.HttpRequestMessage())
                {
                    request.RequestUri = new System.Uri(fullUrl.ToString());
                    using (Windows.Web.Http.HttpResponseMessage responseMessage = await client.SendRequestAsync(request).AsTask())
                    {
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            string strResult = await responseMessage.Content.ReadAsStringAsync().AsTask();
                            System.Diagnostics.Debug.WriteLine("RECORD Reponse from server:");
                            System.Diagnostics.Debug.WriteLine(strResult);
                        }
                    }
                }
            }
            catch (System.Exception)
            { }
        }

        public static async Task<GameDescriptionAndRating> GetGameConfigFomServer(AppDataContext appDataContext)
        {
            JsonObject jsonParameter = ToJsonForGetExpansions(appDataContext);
            string parameter = jsonParameter.Stringify();

            var fullUrl = new System.Text.StringBuilder();
            fullUrl.Append(WebService.webUrl);
            fullUrl.Append("?action=GET&values=");
            fullUrl.Append(parameter.Replace(" ", "%20"));

            try
            {
                using (var client = new Windows.Web.Http.HttpClient())
                using (var request = new Windows.Web.Http.HttpRequestMessage())
                {
                    request.RequestUri = new System.Uri(fullUrl.ToString());                    
                    using (Windows.Web.Http.HttpResponseMessage responseMessage = await client.SendRequestAsync(request).AsTask())
                    {
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            string strResult = await responseMessage.Content.ReadAsStringAsync().AsTask();
                            GameDescriptionAndRating description = WebService.GetGameDescriptionFromJson(strResult);
                            System.Diagnostics.Debug.WriteLine("Get Response from server:");
                            System.Diagnostics.Debug.WriteLine(strResult);
                            return description;
                        }
                    }
                }
            } catch (System.Exception)
            {

            }
            return null;
        }
    }
}

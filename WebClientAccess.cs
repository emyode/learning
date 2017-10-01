using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Web.Http;

namespace Skynet.WebClient
{
    public class WebClientAccess
    {
        private const string LuisUrl = "https://eastus2.api.cognitive.microsoft.com/luis/v2.0/apps/2cacea05-744f-4a0b-98a5-70ef00eb47e1?subscription-key=ec4818f5e8554d109d60ea8dff98707f&verbose=true&timezoneOffset=0&spellCheck=true&q=";

        private static async Task<string> Get(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(new Uri(url));
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        public static async Task<Command> Order(string sentence)
        {
            var result = new Command
            {
                Entity = Entity.All,
                Intent = Intent.None
            };
            try
            {
                var queryUrl = LuisUrl + sentence;
                var resultJson = await Get(queryUrl);

                //read json and turn into command
                JsonObject obj = JsonObject.Parse(resultJson);
                var intent = obj["topScoringIntent"].GetObject()["intent"].GetString();
                switch (intent)
                {
                    case "LightOn":
                        result.Intent = Intent.LightOn;
                        break;
                    case "LightOff":
                        result.Intent = Intent.LightOff;
                        break;
                    default:
                        break;
                }

                var entity = obj["entities"];
                if (entity.GetArray().Any())
                {
                    var entityLight = obj["entities"].GetArray()[0].GetObject()["type"].ToString().Replace("\"","");
                    switch (entityLight)
                    {
                        case "AllLight":
                            result.Entity = Entity.All;
                            break;
                        case "Green":
                            result.Entity = Entity.Green;
                            break;
                        case "Yellow":
                            result.Entity = Entity.Yellow;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                //TODO treat error here
            }

            return result;
        }
    }
}

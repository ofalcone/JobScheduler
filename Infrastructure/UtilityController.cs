using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public static class UtilityController
    {
        public async static Task<U> CallWebApi<T, U>(T inputModel, string controllerName)
        {
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(inputModel), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync($"{CurrentHttpContext.AppBaseUrl}/api/{controllerName}", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<U>(apiResponse);
                    return result;
                }
            }
        }
    }
}

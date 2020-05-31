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
        public async static Task<U> CallWebApi<T, U>(string controllerName, HttpMethodsEnum httpMethodsEnum, T inputModel = default)
        {
            using (var httpClient = new HttpClient())
            {
                switch (httpMethodsEnum)
                {
                    case HttpMethodsEnum.GET:
                        return await WebApiGet<U>(controllerName, httpClient);
                    case HttpMethodsEnum.DELETE:
                        break;
                    case HttpMethodsEnum.POST:
                        return await WebApiPost<T, U>(controllerName, inputModel, httpClient);
                    case HttpMethodsEnum.PUT:
                        break;
                    default:
                        break;
                }
              
            }

            return default;
        }

        private static async Task<U> WebApiGet<U>(string controllerName, HttpClient httpClient)
        {
            using (var response = await httpClient.GetAsync($"{CurrentHttpContext.AppBaseUrl}/api/{controllerName}"))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<U>(apiResponse);
                return result;
            }
        }

        private static async Task<U> WebApiPost<T, U>(string controllerName, T inputModel, HttpClient httpClient)
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

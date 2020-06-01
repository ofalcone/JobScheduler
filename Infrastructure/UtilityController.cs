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
                    case HttpMethodsEnum.GET_BY_ID:
                        return await WebApiGetById<T, U>(controllerName, httpClient, inputModel);
                    //case HttpMethodsEnum.DELETE:
                        //return await WebApiDelete<T, U>(controllerName, httpClient, inputModel);
                    case HttpMethodsEnum.DELETE_CONFIRMED:
                        return await WebApiDeleteConfirm<T, U>(controllerName, httpClient, inputModel);
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

        //private static async Task<U> WebApiDelete<T, U>(string controllerName, HttpClient httpClient, T inputModel = default)
        //{
        //}

        private static async Task<U> WebApiDeleteConfirm<T, U>(string controllerName, HttpClient httpClient, T inputModel = default)
        {
            if (EqualityComparer<T>.Default.Equals(inputModel, default))
            {
                //gestire gli input non validi
                return default;
            }
            try
            {
                using (var response = await httpClient.DeleteAsync($"{CurrentHttpContext.AppBaseUrl}/api/{controllerName}/{inputModel}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<U>(apiResponse);
                    return result;
                }
            }
            catch
            {
                return default;
            }
        }

        private static async Task<U> WebApiGetById<T, U>(string controllerName, HttpClient httpClient, T inputModel = default)
        {
            if (EqualityComparer<T>.Default.Equals(inputModel, default))
            {
                //gestire gli input non validi
                return default;
            }

            try
            {
                using (var response = await httpClient.GetAsync($"{CurrentHttpContext.AppBaseUrl}/api/{controllerName}/{inputModel}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<U>(apiResponse);
                    return result;
                }
            }
            catch
            {
                return default;
            }
        }

        private static async Task<U> WebApiGet<U>(string controllerName, HttpClient httpClient)
        {
            try
            {
                using (var response = await httpClient.GetAsync($"{CurrentHttpContext.AppBaseUrl}/api/{controllerName}"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<U>(apiResponse);
                    return result;
                }
            }
            catch
            {
                return default;
            }
        }

        private static async Task<U> WebApiPost<T, U>(string controllerName, T inputModel, HttpClient httpClient)
        {
            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(inputModel), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync($"{CurrentHttpContext.AppBaseUrl}/api/{controllerName}", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<U>(apiResponse);
                    return result;
                }
            }
            catch
            {
                return default;
            }
        }
    }
}

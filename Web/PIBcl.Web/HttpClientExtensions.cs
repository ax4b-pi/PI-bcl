using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DuCorp.Web
{
    public static class HttpClientExtensions
    {
        public static void SetCanal(this HttpClient httpClient, string canal)
        {
            httpClient.DefaultRequestHeaders.Add("x-canal", canal.ToLower());
        }

        public static void SetToken(this HttpClient httpClient, string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public static void SetApiName(this HttpClient httpClient, string apiName)
        {
            httpClient.DefaultRequestHeaders.Add("x-apiname", apiName.ToLower());
        }

        public async static Task<T> GetStringAsync<T>(this HttpClient httpClient, string uri) 
        {
            var response = await httpClient.GetAsync(uri);

            if (response.StatusCode == HttpStatusCode.NotFound ||
                response.StatusCode == HttpStatusCode.NoContent ||
                response.StatusCode == HttpStatusCode.BadRequest)
                return default(T);

            var stringContent = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(stringContent);
        }
    }
}

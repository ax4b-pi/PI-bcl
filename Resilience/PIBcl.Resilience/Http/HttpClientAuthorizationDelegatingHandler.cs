using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Identity.Client;
using DuCorp.Core;
using System.Linq;
using PIBcl.Auth;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace DuCorp.Resilience.Http
{
    public class HttpClientAuthorizationDelegatingHandler
       : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IApiInfo _apiInfo;
        private readonly DuCorp.Core.IUser _user;
        private readonly IDistributedCache _cache;

        public HttpClientAuthorizationDelegatingHandler(IHttpContextAccessor httpContextAccesor, IApiInfo apiInfo, DuCorp.Core.IUser user, IDistributedCache cache)
        {
            _httpContextAccesor = httpContextAccesor;
            _apiInfo = apiInfo;
            _user = user;
            _cache = cache;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var apiName = request.Headers.SingleOrDefault(h => h.Key == "x-apiname");

            if (null != apiName.Value)
            {
                string scope = _apiInfo.ApiPermissions[apiName.Value.SingleOrDefault()];

                string cacheKey = $"{_apiInfo.Code}_{scope}";

                //var tokenString = await _cache.GetStringAsync(cacheKey);

                // TODO: Identificar se é necessário realizar o Impersonate ou Não.
                if (null != _user && _user.Id != Guid.Empty)
                {
                    var userId = _user.Id.ToString();
                    var userName = _user.Name;
                    var userDoc = _user.Claims.FirstOrDefault(c => c.Type == "extension_Documento");
                    var userMail = _user.Claims.FirstOrDefault(c => c.Type == "emails");


                    var bytes = Encoding.UTF8.GetBytes($"{userId}#{userName}#{userDoc}#{userMail}");
                    var impersonateToken = Convert.ToBase64String(bytes);

                    request.Headers.Add("x-impersonateToken", impersonateToken);
                }

                string accessToken = await _cache.GetStringAsync(cacheKey);

                if (string.IsNullOrEmpty(accessToken))
                {
                    var tokenManager = new TokenManager(_apiInfo.OAuth2Authority,
                  _apiInfo.ClientId, _apiInfo.ClientSecret, "http://localhost:44370");

                    var tokenCache = tokenManager.CreateTokenCache(_apiInfo.ClientId);

                    AuthenticationResult result = await
                       tokenManager.AcquireTokenForClientAsync(new string[] { scope }, tokenCache);

                    if (result != null)
                    {
                        accessToken = result.AccessToken;
                        
                        await _cache.SetStringAsync(cacheKey,
                               JsonConvert.SerializeObject(accessToken), new DistributedCacheEntryOptions()
                               {
                                   AbsoluteExpiration = result.ExpiresOn
                               });
                    }
                }

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Replace("\"", ""));

                // somente necessário pra identificar os scopes de autenticação da API em Flow ou CLient Credentials.
                request.Headers.Remove("x-apiname");


                if (request.Method.Method == "Put")
                {
                    request.Headers.Add("x-requestid", Guid.NewGuid().ToString());
                }

            }
            
            return await base.SendAsync(request, cancellationToken);
        }

        async Task<string> GetToken()
        {
            const string ACCESS_TOKEN = "access_token";

            return await _httpContextAccesor.HttpContext
                .GetTokenAsync(ACCESS_TOKEN);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DuCorp.Auth
{
   public class TokenManager
   {
      private ConfidentialClientApplication _cca;
      private readonly string _authority;
      private readonly string _clientId;
      private readonly string _secret;
      private readonly string _redirectUri;
      private readonly ClientAssertionCertificate _certificado;

      public TokenManager(string authority, string clientId, string secret, string redirectUri = null)
      {
         _authority = authority;
         _clientId = clientId;
         _secret = secret;
         _redirectUri = redirectUri;
      }

      public TokenManager(string authority, string clientId, ClientAssertionCertificate certificado, string redirectUri = null)
      {
         _authority = authority;
         _clientId = clientId;
         _certificado = certificado;
         _redirectUri = redirectUri;
      }

      public TokenCache CreateTokenCache(string key, HttpContext httpContext = null)
         => new TokenSessionCache(key, httpContext).GetTokenCacheInstance();



      public async Task<AuthenticationResult> AcquireTokenForClientAsync(IEnumerable<string> scopes, TokenCache tokenCache)
      {
         if (!string.IsNullOrEmpty(_secret))
            _cca = new ConfidentialClientApplication(_clientId, _authority, _redirectUri, new ClientCredential(_secret), null, tokenCache);
         else if (null != _certificado)
            _cca = new ConfidentialClientApplication(_clientId, _authority, _redirectUri, new ClientCredential(_certificado), null, tokenCache);

         try
         {
            return await _cca.AcquireTokenForClientAsync(scopes);
         }
         catch (MsalUiRequiredException ex)
         {
            throw ex;
         }
      }
   }
}

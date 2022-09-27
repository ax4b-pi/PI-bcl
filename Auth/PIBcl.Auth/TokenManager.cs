using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIBcl.Auth
{
   public class TokenManager
   {
      private IConfidentialClientApplication _cca;
      private readonly string _authority;
      private readonly string _clientId;
      private readonly string _secret;
      private readonly string _redirectUri;
      private readonly ConfidentialClientApplicationBuilder _certificado;

      public TokenManager(string authority, string clientId, string secret, string redirectUri = null)
      {
         _authority = authority;
         _clientId = clientId;
         _secret = secret;
         _redirectUri = redirectUri;
      }

      public TokenManager(string authority, string clientId, ConfidentialClientApplicationBuilder certificado, string redirectUri = null)
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
            //_cca = new ConfidentialClientApplication(_clientId, _authority, _redirectUri, new ClientCredential(_secret), null, tokenCache);
            //_cca =ConfidentialClientApplication(_clientId, _authority, _redirectUri, new ClienteCredential(_secret), null, tokenCache);
            _cca = ConfidentialClientApplicationBuilder.Create(_clientId)
               .WithAuthority(_authority)
               .WithRedirectUri(_redirectUri)
              .WithClientSecret(_secret)
            .Build();
         else if (null != _certificado)
             _cca = ConfidentialClientApplicationBuilder.Create(_clientId)
               .WithAuthority(_authority)
               .WithRedirectUri(_redirectUri)
              //.WithCertificate(new X509Certificate2(_certificado))
            .Build();
            //_cca = new ConfidentialClientApplicationBuilder(_clientId, _authority, _redirectUri, new ClientCredential(_certificado), null, tokenCache);

         try
         {            
            return await _cca.AcquireTokenForClient(scopes).ExecuteAsync().ConfigureAwait(false);
         }
         catch (MsalUiRequiredException ex)
         {
            throw ex;
         }
      }
   }
}

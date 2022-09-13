using System;
using System.Collections.Generic;
using System.Reflection;


namespace DuCorp.Core
{
   public interface IApiInfo
   {
      string AuthorityInstance { get; }
      string Policy { get; }
      string TenantUrl { get; }
      string TenantID { get; }
      string OpenIDAuthority { get; }
      string OAuth2Authority { get; }
      string ClientId { get; }
      string Code { get; }
      string Title { get; }
      string Version { get; }
      string ClientSecret { get; }
      Assembly ApplicationAssembly { get; }
      ApiType Type { get; }
      IDictionary<string, string> Scopes { get; }
      SwaggerAuthInfo SwaggerAuthInfo { get; }

      IDictionary<string, string> ApiPermissions { get; }
   }
   
   public class SwaggerAuthInfo
   {
      public string ClientId { get; }
      public string Secret { get; }
      public string Realm { get; }

      public SwaggerAuthInfo(
          string clientId,
          string secret,
          string realm
          )
      {
         ClientId = clientId;
         Secret = secret;
         Realm = realm;
      }
   }

   public enum ApiType
   {
      ApiGateway,
      Microservice
   }
}

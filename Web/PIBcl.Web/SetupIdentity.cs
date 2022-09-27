using PIBcl.Core;
using PIBcl.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;
using System;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
   public static class SetupIdentity
   {
      public static IServiceCollection AddCustomIdentity(
          this IServiceCollection services,
          IApiInfo apiInfo, IConfiguration configuration = null
          )
      {

         services.AddAuthorization(options =>
         {
            var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                AzureADDefaults.JwtBearerAuthenticationScheme,
                AzureADB2CDefaults.JwtBearerAuthenticationScheme);

            defaultAuthorizationPolicyBuilder =
                defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();


            options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();

         });

         services.AddAuthentication()
            .AddJwtBearer(AzureADB2CDefaults.JwtBearerAuthenticationScheme, options =>
            {

               options.RequireHttpsMetadata = true;

               options.Authority = apiInfo.OpenIDAuthority;
               options.Audience = apiInfo.ClientId;
               
            }).AddJwtBearer(AzureADDefaults.JwtBearerAuthenticationScheme, options =>
            {

               options.RequireHttpsMetadata = true;

               options.Authority = apiInfo.OAuth2Authority;
               options.Audience = apiInfo.ClientId;
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async ctx =>
                    {

                        if (ctx.Principal.Identities.Count() > 0)
                        {
                            var claim =
                                ctx.Principal.Identities.First(c => c.FindFirst(i => i.Type == ClaimTypes.NameIdentifier).Type == ClaimTypes.NameIdentifier);

                            claim.RemoveClaim(claim.Claims.First(c => c.Type == ClaimTypes.NameIdentifier));
                        }

                        var authHeader = ctx.HttpContext.Request.Headers["x-impersonateToken"];
                        if (authHeader != StringValues.Empty)
                        {
                            var impersonateToken = authHeader.FirstOrDefault();
                            byte[] bytes = Convert.FromBase64String(impersonateToken);
                            string str = Encoding.UTF8.GetString(bytes);

                            var arr = str.Split('#');

                            var claims = new List<Claim>
                            {
                                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", arr[1]),
                                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", arr[0]),
                                new Claim("extension_Documento", arr[2]),
                                new Claim("emails", arr[3])
                            };
                            var appIdentity = new ClaimsIdentity(claims);

                            
                            ctx.Principal.AddIdentity(appIdentity);                            

                        }                        
                    }
                };

            });


         services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
         services.AddScoped<PIBcl.Core.IUser, HttpContextUser>();

         services.AddSession();

         return services;
      }
   }
}

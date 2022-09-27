using PIBcl.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Swashbuckle.AspNetCore.Examples;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using System;

namespace PIBcl.Swagger
{
    public static class Startup
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services,
            IApiInfo apiInfo
        ) => services
              .AddSwaggerGen(options =>
              {
                  options.DescribeAllParametersInCamelCase();

                  options.SwaggerDoc(apiInfo.Version, new OpenApiInfo
                  {
                      Title = apiInfo.Title,
                      Version = apiInfo.Version,
                      Description = apiInfo.Version
                  });

                  if (apiInfo.OpenIDAuthority != null)
                  {
                      //options.AddSecurityDefinition("oauth2", new OA)

                      options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                      {
                          Type = SecuritySchemeType.OAuth2,
                          Flows = new OpenApiOAuthFlows(),
                          OpenIdConnectUrl = new Uri("@apiInfo.AuthorityInstance/tfp/@apiInfo.TenantUrl/@apiInfo.Policy/oauth2/v2.0/authorize"),
                          //AuthorizationUrl = $"{apiInfo.AuthorityInstance}/tfp/{apiInfo.TenantUrl}/{apiInfo.Policy}/oauth2/v2.0/authorize",
                          //Scopes = apiInfo.Scopes                          
                      });
                  }

                    options.OperationFilter<AuthorizeCheckOperationFilter>(apiInfo);

                    //options.OperationFilter<AuthorizeCheckOperationFilter>(apiInfo);                    
                    options.OperationFilter<ExamplesOperationFilter>();

                    if (apiInfo.Type == ApiType.Microservice)
                        options.OperationFilter<CanalDeNegocioOperationFilter>();

                    options.CustomSchemaIds(x => x.FullName);
                    options.OperationFilter<DescriptionOperationFilter>();

              });
        // .AddSwaggerGen(options =>
        // {
        //    options.DescribeAllParametersInCamelCase();

        //    //options.DescribeAllEnumsAsStrings();


        //   options.SwaggerDoc(apiInfo.Version, new Info
        //   {
        //      title = apiInfo.Title,
        //      version = apiInfo.Version,
        //      description = apiInfo.Version
        //      //description = apiInfo.Version
        //   });

        //   if (apiInfo.OpenIDAuthority != null)
        //   {
        //      options.AddSecurityDefinition("oauth2", new OAuth2Scheme
        //      {
        //         Type = "oauth2",
        //         Flow = "implicit",
        //         AuthorizationUrl = $"{apiInfo.AuthorityInstance}/tfp/{apiInfo.TenantUrl}/{apiInfo.Policy}/oauth2/v2.0/authorize",
        //        Scopes = apiInfo.Scopes
        //      });
        //   }

        //   options.OperationFilter<AuthorizeCheckOperationFilter>(apiInfo);
        //   options.OperationFilter<ExamplesOperationFilter>();

        //   if (apiInfo.Type == ApiType.Microservice)
        //     options.OperationFilter<CanalDeNegocioOperationFilter>();

        //   //options.CustomSchemaIds(x => x.FullName);
        //   //options.OperationFilter<DescriptionOperationFilter>();
        // });

        public static IApplicationBuilder UseCustomSwagger(
            this IApplicationBuilder app,
            IApiInfo apiInfo
        ) => app
            .UseSwagger()
            .UseSwaggerUI(c =>
            {

                c.SwaggerEndpoint($"/swagger/{apiInfo.Version}/swagger.json", $"{apiInfo.Title} {apiInfo.Version}");

                if (apiInfo.OpenIDAuthority != null)
                {
                    c.OAuthClientId(apiInfo.SwaggerAuthInfo.ClientId);
                    c.OAuthClientSecret(apiInfo.SwaggerAuthInfo.Secret);
                    c.OAuthRealm(apiInfo.SwaggerAuthInfo.Realm);
                    c.OAuthAppName($"{apiInfo.Title} - ${apiInfo.Version} - Swagger UI");

                    c.OAuthAdditionalQueryStringParams(
                     new Dictionary<string, string> {
                      { "resource", apiInfo.ClientId },
                      { "p", apiInfo.Policy }
                     });
                }

            });
    }
}

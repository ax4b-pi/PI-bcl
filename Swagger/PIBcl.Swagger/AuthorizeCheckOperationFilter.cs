using PIBcl.Core;
using System.Collections.Generic;
using System.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;

namespace PIBcl.Swagger
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        private readonly IApiInfo _apiInfo;

        public AuthorizeCheckOperationFilter(IApiInfo apiInfo)
        {
            _apiInfo = apiInfo;
        }        
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!context.HasAuthorize()) return;

            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
            operation.Security = new List<OpenApiSecurityRequirement>();
            operation.Security.Add(                                
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                           Type = SecuritySchemeType.OAuth2
                        },
                        _apiInfo.Scopes.Keys.ToArray()                        
                    }                
                }
            );            
        }
    }
}

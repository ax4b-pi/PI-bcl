using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIBcl.Swagger
{
    public class CanalDeNegocioOperationFilter : IOperationFilter
    {
        public CanalDeNegocioOperationFilter()
        {
        }
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "x-canal",
                In = ParameterLocation.Header,
                Required = true,
                //Type = "string",
            });
        }
    }
}

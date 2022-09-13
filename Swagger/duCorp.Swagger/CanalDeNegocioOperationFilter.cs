using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Text;

namespace DuCorp.Swagger
{
   public class CanalDeNegocioOperationFilter : IOperationFilter
   {
      public CanalDeNegocioOperationFilter()
      {
      }

      public void Apply(Operation operation, OperationFilterContext context)
      {
         if (operation.Parameters == null)
            operation.Parameters = new List<IParameter>();

         operation.Parameters.Add(new HeaderParameter()
         {
            Name = "x-canal",
            In = "header",
            Type = "string",
            Required = true
         });
      }

      class HeaderParameter : NonBodyParameter
      {
      }
   }
}

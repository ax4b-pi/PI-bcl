using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DuCorp.Swagger
{
   internal static class OperationFilterContextExtensions
   {
      internal static bool HasAuthorize(this OperationFilterContext context)
      {
         var apiDescription = context.ApiDescription;
        
         return
             apiDescription.ControllerAttributes().OfType<AuthorizeAttribute>().Any() ||
             apiDescription.ActionAttributes().OfType<AuthorizeAttribute>().Any();
      }
   }
}

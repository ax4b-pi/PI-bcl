using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PIBcl.Web
{
   public static class ModelStateExtensions
   {
      public static BadRequestObjectResult GenerateBadRequestFromExceptions(
          this ModelStateDictionary modelState
      )
      {
         var errors = modelState.Values
             .SelectMany(value => value.Errors)
             .Select(error => error.Exception.InnerException.Message)
             .ToArray();

         return new BadRequestObjectResult(errors);
      }
   }
}

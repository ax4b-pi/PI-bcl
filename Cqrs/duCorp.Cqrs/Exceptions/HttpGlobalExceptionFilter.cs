using System.Linq;
using System.Net;
using DuCorp.Cqrs.Exceptions;
using DuCorp.Cqrs.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace DuCorp.Cqrs.Exceptions
{
   public class HttpGlobalExceptionFilter : IExceptionFilter
   {
      private readonly IHostingEnvironment _env;
      private readonly ILogger<HttpGlobalExceptionFilter> _logger;

      public HttpGlobalExceptionFilter(
          IHostingEnvironment env,
          ILogger<HttpGlobalExceptionFilter> logger
          )
      {
         _env = env;
         _logger = logger;
      }

      public void OnException(ExceptionContext context)
      {
         _logger.LogError(new EventId(context.Exception.HResult),
             context.Exception,
             context.Exception.Message);

         if (context.Exception.GetType() == typeof(PipelineException))
         {
            var validationException = context.Exception.InnerException as ValidationException;
            if (validationException != null)
            {
               var json = new JsonErrorResponse
               {
                  Messages = validationException.Errors
                       .Select(e => e.ErrorMessage)
                       .ToArray()
               };

               context.Result = new BadRequestObjectResult(json);
            }
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
         }
         else
         {
            var json = new JsonErrorResponse();

                if (context.Exception.GetType() == typeof(ResourceNotFoundException))
                {
                    json.Messages = new[]
                    {
                        context.Exception.Message

               };

                    context.Result = new ObjectResult(json) { StatusCode = 404 };
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                else
                {
                    if (context.Exception.GetType().BaseType == typeof(DomainApplicationException))
                    {
                        json.Messages = new[]
                    {
                        context.Exception.Message

                    };

                        context.Result = new ObjectResult(json) { StatusCode = 400 };
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                    else
                    {
                        json.Messages = new[]
                     {
                        context.Exception.Message

                    };
                        context.Result = new ObjectResult(json) { StatusCode = 500 };
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }

                }

            
         }
         context.ExceptionHandled = true;
      }

      public class JsonErrorResponse
      {
         public string[] Messages { get; set; }
      }
   }
}

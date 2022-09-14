using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;


namespace PIBcl.Web.Exceptions
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
            var msg = context.Exception.Message + " LOG CUSTOMIZADO";

            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                msg);

            int statusCode = 500;

            if (context.Exception.GetType() == typeof(HttpRequestException))
            {
                var ex = (HttpRequestException)context.Exception;

                var match = Regex.Match(ex.Message, @"\d+");

                if (null != match)
                {
                    statusCode = int.Parse(match.Value);
                }
            }

            var json = new JsonErrorResponse
            {
                Messages = new[]
                   {
                        context.Exception.Message
                    }
            };

            if (_env.IsDevelopment())
            {
                json.DeveloperMessage = context.Exception;
            }

            context.Result = new ObjectResult(json) { StatusCode = statusCode };
            context.HttpContext.Response.StatusCode = statusCode;

            context.ExceptionHandled = true;
        }


    }
}

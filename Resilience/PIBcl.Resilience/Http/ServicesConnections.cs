using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace PIBcl.Resilience.Http
{
   public static class ServicesConnections
   {

      public static IServiceCollection AddServiceConnection<TIServiceInterface, TServiceImplementation>(this IServiceCollection services)
         where TIServiceInterface : class
         where TServiceImplementation : class, TIServiceInterface
      {

         services.TryAddTransient<HttpClientAuthorizationDelegatingHandler>();
         services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

         services.AddHttpClient<TIServiceInterface, TServiceImplementation>()
               .SetHandlerLifetime(TimeSpan.FromMinutes(5))
               .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
               .AddPolicyHandler(GetRetryPolicy())
               .AddPolicyHandler(GetCircuitBreakerPolicy());

         return services;
      }

      private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
      {
         var random = new Random();

         return HttpPolicyExtensions
           .HandleTransientHttpError()
           .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                                + TimeSpan.FromMilliseconds(random.Next(0, 100)));

      }

      private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
      {
         return HttpPolicyExtensions
             .HandleTransientHttpError()
             .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
      }
   }
}

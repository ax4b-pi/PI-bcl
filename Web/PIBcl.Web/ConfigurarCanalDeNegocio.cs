using PIBcl.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace PIBcl.Web
{
   public static class ConfigurarCanalDeNegocio
   {
      public static IServiceCollection AddRegraCanalDeNegocio(
         this IServiceCollection services
         )
      {
         services.AddSingleton(typeof(CanalDeNegocio));

         services.AddMvc(options =>
            options.Filters.Add(typeof(ValidaCanalDeNegocioFilter))
            );
         //services.AddScoped<ValidaCanalDeNegocioFilter>();
         return services;
      }

      public static IApplicationBuilder UseRegraCanalDeNegocio(
         this IApplicationBuilder app
         )
      {
         return app;
      }
   }

}

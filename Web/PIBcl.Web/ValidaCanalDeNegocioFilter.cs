using PIBcl.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIBcl.Web
{

    public class ValidaCanalDeNegocioFilter : IActionFilter
    {

        private CanalDeNegocio _canalDeNegocio;
        private IApiInfo _apiInfo;

        public ValidaCanalDeNegocioFilter(IApiInfo apiInfo,
           CanalDeNegocio canalDeNegocio)
        {
            _apiInfo = apiInfo;
            _canalDeNegocio = canalDeNegocio;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var headers = request.Headers;

            if (!request.Path.Equals("/") && _apiInfo.Type == ApiType.Microservice)
            {
                if (!headers.Keys.Contains("x-canal"))
                {
                    throw new Exception("Não foi informado o Canal de Negócio");
                }

                var nomeDoCanal = headers["x-canal"][0].ToLower();

                var canais = new List<string> { "somar", "one", "corp" };

                //TODO: Otimizar hardcode da leitura do canal
                if (!canais.Contains(nomeDoCanal))
                //if (!nomeDoCanal.Equals("somar", StringComparison.InvariantCultureIgnoreCase) &&
                //    !nomeDoCanal.Equals("one", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new Exception($"O Canal de Negócio {nomeDoCanal} é inválido.");
                }

                _canalDeNegocio.Nome = headers["x-canal"][0];
            }
        }
    }
}

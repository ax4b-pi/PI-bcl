using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace PIBcl.EventBus.IntegrationEventServices
{
    public class BulkIntegrationExecutionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IIntegrationEventService _integrationEventService;
        private readonly ILogger<BulkIntegrationExecutionBehavior<TRequest, TResponse>> _logger;

        public BulkIntegrationExecutionBehavior(IIntegrationEventService integrationEventService,
            ILogger<BulkIntegrationExecutionBehavior<TRequest, TResponse>> logger)
        {
            _integrationEventService = integrationEventService ?? throw new ArgumentNullException(nameof(integrationEventService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                _logger.LogInformation($"Iniciando o empilhamento do evento de integração: {typeof(TRequest).Name}");

                TResponse response = await next();

                _logger.LogInformation($"Concluindo o empilhamento do evento: {typeof(TRequest).Name}");

                await _integrationEventService.PublishEventsThroughEventBusAsync();


                return response;
            }
            catch (Exception)
            {
                _logger.LogInformation($"Erro ao empilhar e publicar o evento de integração: {typeof(TRequest).Name}");

                throw;
            }
        }
    }
}

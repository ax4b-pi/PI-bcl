﻿using Autofac;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIBcl.EventBus.AzureServiceBus
{
    public class AzureEventServiceBus : IEventBus
    {

        private readonly IServiceBusPersisterConnection _serviceBusPersisterConnection;
        private readonly ILogger<AzureEventServiceBus> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly SubscriptionClient _subscriptionClient;
        private readonly ILifetimeScope _autofac;
        private readonly string AUTOFAC_SCOPE_NAME = "root";
        private const string INTEGRATION_EVENT_SUFIX = "IntegrationEvent";

        public AzureEventServiceBus(IServiceBusPersisterConnection serviceBusPersisterConnection,
         ILogger<AzureEventServiceBus> logger, IEventBusSubscriptionsManager subsManager, string subscriptionClientName,
         ILifetimeScope autofac)
        {
            _serviceBusPersisterConnection = serviceBusPersisterConnection;
            _logger = logger;
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            _autofac = autofac;

            if (!string.IsNullOrEmpty(subscriptionClientName))
            {
                _subscriptionClient = new SubscriptionClient(serviceBusPersisterConnection.ServiceBusConnectionStringBuilder,
                subscriptionClientName);

                RemoveDefaultRule();
                RegisterSubscriptionClientMessageHandler();
            }            
        }

        public void Publish(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name.Replace(INTEGRATION_EVENT_SUFIX, "");
            var jsonMessage = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            var message = new Message
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = body,
                Label = eventName,
            };

            var topicClient = _serviceBusPersisterConnection.CreateModel();

            topicClient.SendAsync(message)
                .GetAwaiter()
                .GetResult();
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name.Replace(INTEGRATION_EVENT_SUFIX, "");

            var containsKey = _subsManager.HasSubscriptionsForEvent<T>();
            if (!containsKey)
            {
                try
                {
                    _subscriptionClient.AddRuleAsync(new RuleDescription
                    {
                        Filter = new CorrelationFilter { Label = eventName },
                        Name = eventName
                    }).GetAwaiter().GetResult();
                }
                catch (ServiceBusException)
                {
                    _logger.LogInformation($"Já existe um assinante para o evento {eventName}.");
                }
            }

            _subsManager.AddSubscription<T, TH>();
        }

        public void Subscribe<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            _subsManager.AddSubscription<TH>(eventName);
        }

        public void Unsubscribe<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            _subsManager.RemoveSubscription<TH>(eventName);
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name.Replace(INTEGRATION_EVENT_SUFIX, "");

            try
            {
                _subscriptionClient
                 .RemoveRuleAsync(eventName)
                 .GetAwaiter()
                 .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                _logger.LogInformation($"Não foi encontrada um assinante para o evento {eventName}.");
            }

            _subsManager.RemoveSubscription<T, TH>();
        }

        private void RegisterSubscriptionClientMessageHandler()
        {
            _subscriptionClient.RegisterMessageHandler(
                async (message, token) =>
                {
                    var eventName = $"{message.Label}{INTEGRATION_EVENT_SUFIX}";
                    var messageData = Encoding.UTF8.GetString(message.Body);

                    
                    if (await ProcessEvent(eventName, messageData))
                    {
                        await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                    }
                },
               new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = 10, AutoComplete = false });
        }

        private async Task<bool> ProcessEvent(string eventName, string message)
        {
            var processed = false;
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _autofac.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
                {
                    var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        if (subscription.IsDynamic)
                        {
                            if (!(scope.ResolveOptional(subscription.HandlerType) is IDynamicIntegrationEventHandler handler)) continue;
                            dynamic eventData = JObject.Parse(message);
                            await handler.Handle(eventData);
                        }
                        else
                        {
                            var handler = scope.ResolveOptional(subscription.HandlerType);
                            if (handler == null) continue;
                            var eventType = _subsManager.GetEventTypeByName(eventName);
                            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                            await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                        }
                    }
                }
                processed = true;
            }
            return processed;
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encontrou uma exceção {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Cotexto da execução para troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        private void RemoveDefaultRule()
        {
            try
            {
                _subscriptionClient
                 .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                 .GetAwaiter()
                 .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                _logger.LogInformation($"A mensagem { RuleDescription.DefaultRuleName } não foi encontrada.");
            }
        }
    }
}

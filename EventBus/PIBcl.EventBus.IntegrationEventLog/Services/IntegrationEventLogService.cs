using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PIBcl.EventBus.IntegrationEventLog.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private readonly DocumentClient _cosmosClient;
        private readonly IntegrationEventLogServiceSettings _settings;
        private const string COLLECTION_ID = "IntegrationEventLogEntry";
        private static List<Type> _eventTypes;
        private readonly IConfiguration _configuration;

        private List<Type> EventTypes {

            get {

                if (null == _eventTypes)
                {
                    _eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
                        .GetTypes()
                        .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
                        .ToList();
                }

                return _eventTypes;
            }
        }

       
        public IntegrationEventLogService(IConfiguration configuration)
        {
            _configuration = configuration;
            
            _settings =
                _configuration.GetSection("IntegrationEventLogServiceSettings")
                .Get<IntegrationEventLogServiceSettings>();

            _cosmosClient = new DocumentClient(new Uri(_settings.EndPoint), _settings.Key);

        }

        
        public Task MarkEventAsFailedAsync(Guid eventId, string partitionKey = null)
        {
            return UpdateEventStatus(eventId, EventStateEnum.PublishedFailed, partitionKey);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId, string partitionKey = null)
        {
            return UpdateEventStatus(eventId, EventStateEnum.InProgress, partitionKey);
        }

        public Task MarkEventAsPublishedAsync(Guid eventId, string partitionKey = null)
        {
            return UpdateEventStatus(eventId, EventStateEnum.Published, partitionKey);
        }

        public Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync()
        {
            var eventsLogEntry = _cosmosClient.CreateDocumentQuery<IntegrationEventLogEntry>(
                UriFactory.CreateDocumentCollectionUri(
                    _settings.DataBase.Replace("{environment}", _configuration["Environment"])
                    .Replace("{versionColor}", _configuration["VersionColor"]), COLLECTION_ID), new FeedOptions()
                {
                    JsonSerializerSettings = new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    },
                    EnableCrossPartitionQuery = true
                })
             .Where(e => e.State == EventStateEnum.NotPublished)
             .OrderBy(o => o.CreationTime).ToList();

            foreach (var logEntry in eventsLogEntry)
            {
                logEntry.DeserializeJsonContent();
            }
            
            return Task.FromResult<IEnumerable<IntegrationEventLogEntry>>(eventsLogEntry);
        }

        public async Task SaveEventAsync(IntegrationEvent @event)
        {
            var eventLogEntry = new IntegrationEventLogEntry(@event);

            await _cosmosClient
                .CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_settings.DataBase.Replace("{environment}", _configuration["Environment"])
                    .Replace("{versionColor}", _configuration["VersionColor"]), COLLECTION_ID), eventLogEntry, new RequestOptions()
                {
                    JsonSerializerSettings = new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    }
                });
        }

        private async Task UpdateEventStatus(Guid eventId, EventStateEnum status, string partitionKey = null)
        {
            Uri documentUri = UriFactory.CreateDocumentUri(_settings.DataBase.Replace("{environment}", _configuration["Environment"])
                    .Replace("{versionColor}", _configuration["VersionColor"]), COLLECTION_ID, eventId.ToString());

            Document document = await _cosmosClient.ReadDocumentAsync(documentUri,
                     new RequestOptions { PartitionKey = new PartitionKey(partitionKey) });

            var eventLogEntry = JsonConvert.DeserializeObject<IntegrationEventLogEntry>(document.ToString(), new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            eventLogEntry.State = status;

            if (status == EventStateEnum.InProgress)
                eventLogEntry.TimesSent++;

            await _cosmosClient.ReplaceDocumentAsync(documentUri, eventLogEntry);
        }
    }
}

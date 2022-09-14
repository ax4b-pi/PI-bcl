using Microsoft.Azure.ServiceBus;
using System;

namespace PIBcl.EventBus.AzureServiceBus
{
    public interface IServiceBusPersisterConnection
    {
        ServiceBusConnectionStringBuilder ServiceBusConnectionStringBuilder { get; }

        ITopicClient CreateModel();
    }
}

using RabbitMQ.Client;
using System;

namespace PIBcl.EventBus.RabbitMQEventBus
{
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}

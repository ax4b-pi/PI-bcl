using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DuCorp.EventBus
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic @event);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIBcl.EventBus
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic @event);
    }
}

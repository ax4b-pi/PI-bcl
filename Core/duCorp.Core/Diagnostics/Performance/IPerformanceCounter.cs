using System;
using System.Collections.Generic;
using System.Text;

namespace DuCorp.Core.Diagnostics.Performance
{
    public interface IPerformanceCounter
    {
        string Name { get; }
        void Start();
        void Stop();

        double TotalMilliseconds { get; }
    }
}

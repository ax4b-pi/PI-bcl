using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PIBcl.Core.Diagnostics.Performance
{
    public interface IPerformanceMonitor
    {
        IPerformanceCounter StartCounter(string name);
        void Save(string eventName, params IPerformanceCounter[] counters);
    }
}

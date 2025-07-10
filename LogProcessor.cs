using OpenTelemetry;
using OpenTelemetry.Logs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WebAPIServer.Net
{
    internal class LogProcessor : BaseProcessor<LogRecord>
    {
        public override void OnEnd(LogRecord record)
        {
            if (Activity.Current != null)
            {
                var logState = new List<KeyValuePair<string, object>>
                {
                    new("ProcessID", Environment.ProcessId),
                    new("DotnetFramework", RuntimeInformation.FrameworkDescription),
                    new("Runtime", RuntimeInformation.RuntimeIdentifier)
                };

                if (record.StateValues != null)
                {
                    var state = record.StateValues.ToList();
                    record.StateValues = new ReadOnlyCollectionBuilder<KeyValuePair<string, object>>(state.Concat(logState)).ToReadOnlyCollection();
                }
            }
            
            base.OnEnd(record);
        }
    }
}

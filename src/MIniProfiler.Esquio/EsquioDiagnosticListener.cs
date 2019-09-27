using Esquio;
using StackExchange.Profiling;
using StackExchange.Profiling.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MIniProfiler.Esquio
{
    internal class EsquioDiagnosticListener
        : IMiniProfilerDiagnosticListener
    {
        public string ListenerName => "Esquio";

        public void OnCompleted() { }

        public void OnError(Exception error) => Trace.WriteLine(error);


        public void OnNext(KeyValuePair<string, object> entry)
        {
            var (key, value) = entry;

            if (key == EsquioConstants.ESQUIO_BEGINFEATURE_ACTIVITY_NAME)
            {
                dynamic featureData = (dynamic)value;

                var timing = MiniProfiler.Current.CustomTiming(ListenerName, $"Esquio Feature Evaluation  {featureData.ProductName}:{featureData.FeatureName}", "Feature Evaluation");

            }
        }
    }
}

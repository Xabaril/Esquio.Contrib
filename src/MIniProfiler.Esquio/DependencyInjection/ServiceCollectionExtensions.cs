using Microsoft.Extensions.DependencyInjection;
using StackExchange.Profiling.Internal;
using System;

namespace MIniProfiler.Esquio.DependencyInjection
{
    /// <summary>
    /// Extension methods for the MiniProfiler.Esquio.
    /// </summary>
    public static class MiniProfilerServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Esquio profiling for MiniProfiler via DiagnosticListener.
        /// </summary>
        /// <param name="builder">The <see cref="IMiniProfilerBuilder" /> to add services to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is <c>null</c>.</exception>
        public static IMiniProfilerBuilder AddEntityFramework(this IMiniProfilerBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Services.AddSingleton<IMiniProfilerDiagnosticListener, EsquioDiagnosticListener>();

            return builder;
        }
    }
}

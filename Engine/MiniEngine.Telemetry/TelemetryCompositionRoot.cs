using LightInject;

namespace MiniEngine.Telemetry
{
    public sealed class TelemetryCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
#if TRACE
            serviceRegistry.Register<IMeterRegistry, PrometheusMeterRegistry>();
            serviceRegistry.Register<IMetricServer, PrometheusMetricServer>();
#else
            serviceRegistry.Register<IMeterRegistry, NullMeterRegistry>();
            serviceRegistry.Register<IMetricServer, NullMetricServer>();
#endif
        }

    }
}

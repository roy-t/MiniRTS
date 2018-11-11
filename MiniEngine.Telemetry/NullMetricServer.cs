namespace MiniEngine.Telemetry
{
    public sealed class NullMetricServer : IMetricServer
    {
        public void Start(int port) { }
        public void Stop() { }
    }
}

namespace MiniEngine.Telemetry
{
    public interface IMetricServer
    {
        void Start(int port);
        void Stop();
    }
}

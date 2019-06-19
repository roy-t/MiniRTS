namespace MiniEngine.Telemetry
{
    public interface IMeterRegistry
    {
        void CreateCounter(string name, params string[] labelNames);
        void CreateGauge(string name, params string[] labelNames);
        void IncreaseCounter(string name, int value, params string[] labelValues);
        void IncreaseCounter(string name, params string[] labelValues);
        void SetGauge(string name, double value, params string[] labelValues);
        void StartGauge(string name);
        void StopGauge(string name, params string[] labelValues);
    }
}
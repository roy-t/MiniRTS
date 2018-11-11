namespace MiniEngine.Telemetry
{
    public sealed class NullMeterRegistry : IMeterRegistry
    {
        public void CreateCounter(string name, params string[] labelNames) { }
        public void CreateGauge(string name, params string[] labelNames) { }
        public void IncreaseCounter(string name, int value, params string[] labelValues) { }
        public void IncreaseCounter(string name, params string[] labelValues) { }
        public void SetGauge(string name, double value, params string[] labelValues) { }
        public void StartGauge(string name) { }
        public void StopGauge(string name, params string[] labelValues) { }
    }
}

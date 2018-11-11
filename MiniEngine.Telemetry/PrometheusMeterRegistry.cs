using Prometheus;
using System.Collections.Generic;
using System.Diagnostics;

namespace MiniEngine.Telemetry
{
    public class PrometheusMeterRegistry : IMeterRegistry
    {
        private readonly Dictionary<string, Gauge> Gauges;
        private readonly Dictionary<string, Stopwatch> Stopwatches;
        private readonly Dictionary<string, Counter> Counters;

        public PrometheusMeterRegistry()
        {
            this.Gauges = new Dictionary<string, Gauge>(0);
            this.Stopwatches = new Dictionary<string, Stopwatch>(0);
            this.Counters = new Dictionary<string, Counter>(0);
        }

        public void CreateGauge(string name, params string[] labelNames)
        {
            var gauge = Metrics.CreateGauge(name, string.Empty, labelNames);            
            this.Gauges.Add(name, gauge);
            this.Stopwatches.Add(name, new Stopwatch());
        }

        public void SetGauge(string name, double value, params string[] labelValues) => this.Gauges[name].WithLabels(labelValues).Set(value);

        public void StartGauge(string name) => this.Stopwatches[name].Restart();

        public void StopGauge(string name, params string[] labelValues)
        {
            var stopwatch = this.Stopwatches[name];
            stopwatch.Stop();
            this.Gauges[name].WithLabels(labelValues).Set(stopwatch.Elapsed.TotalSeconds);
        }

        public void CreateCounter(string name, params string[] labelNames)
        {
            var counter = Metrics.CreateCounter(name, string.Empty, labelNames);
            this.Counters.Add(name, counter);
        }

        public void IncreaseCounter(string name, params string[] labelValues) => this.Counters[name].WithLabels(labelValues).Inc();

        public void IncreaseCounter(string name, int value, params string[] labelValues) => this.Counters[name].WithLabels(labelValues).Inc(value);
    }
}
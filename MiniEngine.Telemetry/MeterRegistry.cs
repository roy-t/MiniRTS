using System.Collections.Generic;

namespace MiniEngine.Telemetry
{
    public sealed class MeterRegistry : IMeterRegistry
    {
        private readonly List<Counter> Counters;
        private readonly List<CountEntry> Counts;
        
        private readonly List<Gauge> Gauges;
        private readonly List<MeasurementEntry> Measurements;

        public MeterRegistry()
        {
            this.Counters = new List<Counter>();
            this.Counts = new List<CountEntry>();
            this.Gauges = new List<Gauge>();
            this.Measurements = new List<MeasurementEntry>();
        }

        public Counter CreateCounter(string tag)
        {
            var counter = new Counter(tag);
            this.Counters.Add(counter);
            this.Counts.Add(new CountEntry(tag, 0));

            return counter;
        }

        public Gauge CreateGauge(string tag)
        {
            var gauge = new Gauge(tag);
            this.Gauges.Add(gauge);
            this.Measurements.Add(new MeasurementEntry(tag, 0));

            return gauge;
        }

        public IReadOnlyList<CountEntry> GetCounts()
        {
            for (var i = 0; i < this.Counters.Count; i++)
            {
                var counter = this.Counters[i];
                this.Counts[i].Count = counter.Count;
            }

            return this.Counts;
        }

        public IReadOnlyList<MeasurementEntry> GetMeasurements()
        {
            for (var i = 0; i < this.Gauges.Count; i++)
            {
                var gauge = this.Gauges[i];
                this.Measurements[i].Measurement = gauge.Measurement;
            }

            return this.Measurements;
        }

        public void ResetCounters()
        {
            for (var i = 0; i < this.Counters.Count; i++)
            {
                this.Counters[i].Reset();
                this.Counts[i].Count = 0;
            }

            // Gauges do not need to be reset as their contents is overwritten every frame
        }
    }
}

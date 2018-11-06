using MiniEngine.Units;
using System.Diagnostics;

namespace MiniEngine.Telemetry
{
    public sealed class Gauge
    {
        private readonly Stopwatch StopWatch;

        public Gauge(string name, Tag[] tags)
        {
            this.Name = name;
            this.Tags = tags;
            this.StopWatch = new Stopwatch();
        }

        public string Name { get; }
        public Tag[] Tags { get; }
        public Seconds Measurement { get; private set; }


        [Conditional("TRACE")]
        public void BeginMeasurement() => this.StopWatch.Restart();

        [Conditional("TRACE")]
        public void EndMeasurement()
        {
            this.StopWatch.Stop();
            this.Measurement = this.StopWatch.Elapsed;            
        }
    }
}

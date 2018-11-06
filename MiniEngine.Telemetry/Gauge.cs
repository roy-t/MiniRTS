using MiniEngine.Units;
using System;
using System.Diagnostics;

namespace MiniEngine.Telemetry
{
    public sealed class Gauge
    {
        private readonly Stopwatch StopWatch;

        public Gauge(string tag)
        {
            this.Tag = tag;
            this.StopWatch = new Stopwatch();
        }

        public string Tag { get; }
        public Seconds Measurement { get; private set; }


        public void Measure(Action action)
        {
            this.StopWatch.Restart();
            action();
            this.StopWatch.Stop();
            this.Measurement = this.StopWatch.Elapsed;            
        }        
    }
}

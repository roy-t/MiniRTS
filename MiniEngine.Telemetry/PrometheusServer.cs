using System;
using System.Globalization;
using System.Text;

namespace MiniEngine.Telemetry
{
    public sealed class PrometheusServer : ITelemetryServer
    {
        private readonly IMeterRegistry MeterRegistry;
        private readonly HttpServer Server;

        private readonly StringBuilder StringBuilder;

        public PrometheusServer(IMeterRegistry meterRegistry)
        {
            this.MeterRegistry = meterRegistry;
            this.StringBuilder = new StringBuilder();
            this.Server = new HttpServer(7070);
        }

        public void Start() => this.Server.Start(this.CreateBody);
        public void Stop() => this.Server.Stop();

        private string CreateBody()
        {            
            this.StringBuilder.Clear();
            var counts = this.MeterRegistry.GetCounts();
            var measurements = this.MeterRegistry.GetMeasurements();

            foreach(var count in counts)
            {
                this.StringBuilder.AppendLine($"# HELP {count.Name}");
                this.StringBuilder.AppendLine($"# TYPE {count.Name} counter");
                this.StringBuilder.AppendLine($"{count.Name}{PrometheusUtilities.ExpandTags(count.Tags)} {count.Count}");
            }

            foreach(var measurement in measurements)
            {
                this.StringBuilder.AppendLine($"# HELP {measurement.Name}");
                this.StringBuilder.AppendLine($"# TYPE {measurement.Name} gauge");
                this.StringBuilder.AppendLine($"{measurement.Name}{PrometheusUtilities.ExpandTags(measurement.Tags)} {measurement.Measurement.Value.ToString("R", CultureInfo.InvariantCulture)}");
            }

            this.StringBuilder.Replace(Environment.NewLine, "\n");
            return this.StringBuilder.ToString();
        }
    }
}

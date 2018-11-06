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
                AppendLinuxLine($"# HELP {count.Name}");
                AppendLinuxLine($"# TYPE {count.Name} counter");
                AppendLinuxLine($"{count.Name}{PrometheusUtilities.ExpandTags(count.Tags)} {count.Count}");
            }

            foreach(var measurement in measurements)
            {
                AppendLinuxLine($"# HELP {measurement.Name}");
                AppendLinuxLine($"# TYPE {measurement.Name} gauge");
                AppendLinuxLine($"{measurement.Name}{PrometheusUtilities.ExpandTags(measurement.Tags)} {measurement.Measurement.Value.ToString("R", CultureInfo.InvariantCulture)}");
            }

            return this.StringBuilder.ToString();
        }

        private void AppendLinuxLine(string line)
        {
            this.StringBuilder.Append(line);
            this.StringBuilder.Append('\n');
        }
    }
}

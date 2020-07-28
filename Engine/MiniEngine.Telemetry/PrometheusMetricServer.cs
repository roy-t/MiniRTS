using System;
using System.Diagnostics;
using Prometheus;

namespace MiniEngine.Telemetry
{
    public sealed class PrometheusMetricServer : IMetricServer
    {
        private MetricServer server;

        public void Start(int port)
        {
            this.server?.Stop();

            this.server = new MetricServer(port);
            try
            {
                this.server.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to start metrics server, continuing without one. Exception: " + ex);
                Debug.WriteLine("Try running 'netsh http add urlacl url=http://+:7070/metrics user=USER'");
            }
        }

        public void Stop() => this.server.Stop();
    }
}

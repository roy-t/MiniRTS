using System.Collections.Generic;

namespace MiniEngine.Telemetry
{
    public interface IMeterRegistry
    {
        Counter CreateCounter(string tag);
        Gauge CreateGauge(string tag);

        void ResetCounters();

        IReadOnlyList<CountEntry> GetCounts();
        IReadOnlyList<MeasurementEntry> GetMeasurements();
    }
}

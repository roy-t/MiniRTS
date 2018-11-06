using System.Collections.Generic;

namespace MiniEngine.Telemetry
{
    public interface IMeterRegistry
    {
        Counter CreateCounter(string name, params Tag[] tags);
        Gauge CreateGauge(string name, params Tag[] tags);

        void ResetCounters();

        IReadOnlyList<CountEntry> GetCounts();
        IReadOnlyList<MeasurementEntry> GetMeasurements();
    }
}

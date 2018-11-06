using MiniEngine.Units;

namespace MiniEngine.Telemetry
{
    public sealed class MeasurementEntry
    {
        public MeasurementEntry(string tag, Seconds measurement)
        {
            this.Tag = tag;
            this.Measurement = measurement;
        }

        public string Tag { get; }
        public Seconds Measurement { get; internal set; }

        public override string ToString() => $"{this.Tag} : {this.Measurement.Value:F2}s";
    }
}

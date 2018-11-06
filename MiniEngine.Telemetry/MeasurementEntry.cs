using MiniEngine.Units;

namespace MiniEngine.Telemetry
{
    public sealed class MeasurementEntry
    {
        public MeasurementEntry(string name, Tag[] tags, Seconds measurement)
        {
            this.Name = name;
            this.Tags = tags;
            this.Measurement = measurement;
        }

        public string Name { get; }
        public Tag[] Tags { get; }
        public Seconds Measurement { get; internal set; }

        public override string ToString() => $"{this.Name}{PrometheusUtilities.ExpandTags(this.Tags)} {this.Measurement.Value:F2}s";
    }
}

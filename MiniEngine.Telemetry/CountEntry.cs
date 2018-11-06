namespace MiniEngine.Telemetry
{
    public sealed class CountEntry
    {
        public CountEntry(string name, Tag[] tags, int count)
        {
            this.Name = name;
            this.Tags = tags;
            this.Count = count;
        }

        public string Name { get; }
        public Tag[] Tags { get; }
        public int Count { get; internal set; }

        public override string ToString() => $"{this.Name}{PrometheusUtilities.ExpandTags(this.Tags)} {this.Count}";
    }
}

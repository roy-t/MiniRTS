namespace MiniEngine.Telemetry
{
    public sealed class CountEntry
    {
        public CountEntry(string tag, int count)
        {
            this.Tag = tag;
            this.Count = count;
        }

        public string Tag { get; }
        public int Count { get; internal set; }
    }
}

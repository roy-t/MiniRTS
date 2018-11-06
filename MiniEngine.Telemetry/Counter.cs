using System.Diagnostics;

namespace MiniEngine.Telemetry
{
    public sealed class Counter
    {
        internal Counter(string name, Tag[] tags)
        {
            this.Name = name;
            this.Tags = tags;
            this.Count = 0;
        }

        public string Name { get; }
        public Tag[] Tags { get; }
        public int Count { get; private set; }

        [Conditional("TRACE")]
        public void Increase() => ++this.Count;
        [Conditional("TRACE")]
        public void IncreaseWith(int count) => this.Count += count;
        
        internal void Reset() => this.Count = 0;
    }
}

namespace MiniEngine.Telemetry
{
    public sealed class Counter
    {
        internal Counter(string tag)
        {
            this.Tag = tag;
            this.Count = 0;
        }

        public string Tag { get; }
        public int Count { get; private set; }

        public void Increase() => ++this.Count;
        public void IncreaseWith(int count) => this.Count += count;
        
        internal void Reset() => this.Count = 0;
    }
}

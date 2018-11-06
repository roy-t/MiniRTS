namespace MiniEngine.Telemetry
{
    public sealed class Tag
    {
        public Tag(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; }
        public string Value { get; }

        public override string ToString() => $"{this.Name}=\"{this.Value}\"";
    }    
}

namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class Parameter : ISource
    {
        public Parameter(string type, string name)
        {
            this.Type = type;
            this.Name = name;
        }

        public string Type { get; }
        public string Name { get; }

        public void Generate(SourceWriter writer)
            => writer.Write($"{this.Type} {this.Name}");
    }
}

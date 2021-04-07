namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class Field : ISource
    {
        public Field(string type, string name, params string[] modifiers)
        {
            this.Type = type;
            this.Name = name;
            this.Modifiers = modifiers;
        }

        public string[] Modifiers { get; }
        public string Type { get; }
        public string Name { get; }

        public void Generate(SourceWriter writer)
        {
            writer.WriteModifiers(this.Modifiers);
            writer.WriteLine($"{this.Type} {this.Name}");
        }
    }
}

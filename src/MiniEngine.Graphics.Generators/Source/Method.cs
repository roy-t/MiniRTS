namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class Method : ISource
    {
        public Method(string type, string name, params string[] modifiers)
        {
            this.Type = type;
            this.Name = name;
            this.Modifiers = modifiers;
            this.Parameters = new ParameterList();
            this.Body = new Body();
        }

        public string Type { get; }
        public string Name { get; }
        public string[] Modifiers { get; }
        public ParameterList Parameters { get; }
        public Body Body { get; }

        public void Generate(SourceWriter writer)
        {
            writer.WriteModifiers(this.Modifiers);
            writer.Write($"{this.Type} {this.Name}");
            this.Parameters.Generate(writer);
            writer.WriteLine();

            writer.StartScope();
            this.Body.Generate(writer);
            writer.EndScope();
        }
    }
}

using Microsoft.CodeAnalysis;

namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class Property : ISource
    {
        public Property(string type, string name, bool isReadOnly, params string[] modifiers)
        {
            this.Type = type;
            this.Name = name;
            this.IsReadOnly = isReadOnly;
            this.Modifiers = modifiers;
        }

        public string Type { get; }
        public string Name { get; }
        public bool IsReadOnly { get; }
        public string[] Modifiers { get; }

        public Optional<string[]> GetModifiers { get; set; }
        public Optional<string[]> SetModifiers { get; set; }

        public Optional<Body> Getter { get; set; }
        public Optional<Body> Setter { get; set; }

        public void SetGetter(Body body) => this.Getter = new Optional<Body>(body);
        public void SetSetter(Body body) => this.Setter = new Optional<Body>(body);

        public void Generate(SourceWriter writer)
        {
            writer.WriteModifiers(this.Modifiers);
            writer.Write($"{this.Type} {this.Name}");
            if (this.IsAutoProperty())
            {
                writer.Write(" { ");
                if (this.GetModifiers.HasValue)
                {
                    writer.WriteModifiers(this.GetModifiers.Value);

                }
                writer.Write("get;");

                if (!this.IsReadOnly)
                {
                    if (this.SetModifiers.HasValue)
                    {
                        writer.WriteModifiers(this.SetModifiers.Value);
                    }
                    writer.Write("set;");
                }

                writer.Write(" } ");
            }
            else
            {
                writer.WriteLine();
                writer.StartScope();

                if (this.Getter.HasValue)
                {
                    if (this.GetModifiers.HasValue)
                    {
                        writer.WriteModifiers(this.GetModifiers.Value);
                    }
                    writer.WriteLine("get");
                    writer.StartScope();
                    this.Getter.Value.Generate(writer);
                    writer.EndScope();
                }

                if (this.Setter.HasValue)
                {
                    if (this.SetModifiers.HasValue)
                    {
                        writer.WriteModifiers(this.SetModifiers.Value);
                    }
                    writer.WriteLine("set");
                    writer.StartScope();
                    this.Setter.Value.Generate(writer);
                    writer.EndScope();
                }

                writer.EndScope();
            }
        }

        public bool IsAutoProperty() => !(this.Getter.HasValue && this.Setter.HasValue);
    }
}

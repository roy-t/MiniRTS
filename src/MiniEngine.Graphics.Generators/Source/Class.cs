using System.Collections.Generic;

namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class Class : ISource
    {
        public Class(string name, params string[] modifiers)
        {
            this.Name = name;
            this.Modifiers = modifiers;
            this.Fields = new List<Field>();
            this.Constructors = new List<Constructor>();
            this.Properties = new List<Property>();
        }

        public string Name { get; }
        public string[] Modifiers { get; }

        public List<Field> Fields { get; }
        public List<Constructor> Constructors { get; }
        public List<Property> Properties { get; }


        public void Generate(SourceWriter writer)
        {
            writer.WriteModifiers(this.Modifiers);
            writer.WriteLine($"class {this.Name}");
            writer.StartScope();

            foreach (var field in this.Fields)
            {
                field.Generate(writer);
            }

            if (this.Fields.Count > 0) { writer.WriteLine(); }

            foreach (var constructor in this.Constructors)
            {
                constructor.Generate(writer);
                writer.WriteLine();
            }

            foreach (var property in this.Properties)
            {
                property.Generate(writer);
                writer.WriteLine();
            }

            writer.EndScope();
        }
    }
}

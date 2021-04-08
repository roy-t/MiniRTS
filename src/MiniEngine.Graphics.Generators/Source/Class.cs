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
            this.Methods = new List<Method>();
            this.InheritsFrom = new List<string>();
        }

        public string Name { get; }
        public string[] Modifiers { get; }

        public List<Field> Fields { get; }
        public List<Constructor> Constructors { get; }
        public List<Property> Properties { get; }
        public List<Method> Methods { get; }
        public List<string> InheritsFrom { get; }

        public void Generate(SourceWriter writer)
        {
            writer.WriteModifiers(this.Modifiers);
            writer.WriteLine($"class {this.Name}");
            if (this.InheritsFrom.Count > 0)
            {
                writer.StartIndent();
                writer.Write(": ");
                writer.Write(string.Join(", ", this.InheritsFrom));
                writer.EndIndent();
                writer.WriteLine();
            }
            writer.StartScope();

            foreach (var field in this.Fields)
            {
                field.Generate(writer);
            }

            writer.ConditionalEmptyLine(this.Fields.Count > 0);

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

            foreach (var method in this.Methods)
            {
                method.Generate(writer);
                writer.WriteLine();
            }

            writer.EndScope();
        }
    }
}

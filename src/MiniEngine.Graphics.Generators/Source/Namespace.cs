using System.Collections.Generic;

namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class Namespace : ISource
    {
        public Namespace(string name)
        {
            this.Name = name;
            this.Classes = new List<Class>();
        }

        public string Name { get; }

        public List<Class> Classes { get; }

        public void Generate(SourceWriter writer)
        {
            writer.WriteLine($"namespace {this.Name}");
            writer.StartScope();

            foreach (var @class in this.Classes)
            {
                @class.Generate(writer);
            }

            writer.EndScope();
        }
    }
}

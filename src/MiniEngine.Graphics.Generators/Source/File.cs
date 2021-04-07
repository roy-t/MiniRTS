using System.Collections.Generic;

namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class File : ISource
    {
        public File(string fileName)
        {
            this.FileName = fileName;
            this.Usings = new List<Using>();
            this.Namespaces = new List<Namespace>();
        }

        public string FileName { get; }
        public List<Using> Usings { get; }
        public List<Namespace> Namespaces { get; }

        public void Generate(SourceWriter writer)
        {
            foreach (var @using in this.Usings)
            {
                @using.Generate(writer);
            }

            foreach (var @namespace in this.Namespaces)
            {
                @namespace.Generate(writer);
            }
        }
    }
}

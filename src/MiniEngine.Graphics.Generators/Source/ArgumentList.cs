using System.Collections.Generic;

namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class ArgumentList : ISource
    {
        public ArgumentList()
        {
            this.Arguments = new List<string>();
        }

        public List<string> Arguments { get; }

        public void Generate(SourceWriter writer)
        {
            var arguments = string.Join(", ", this.Arguments);
            writer.Write($"({arguments})");
        }
    }
}

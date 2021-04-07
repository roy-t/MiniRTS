using System.Collections.Generic;

namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class ParameterList : ISource
    {
        public ParameterList()
        {
            this.Parameters = new List<Parameter>();
        }

        public List<Parameter> Parameters { get; }

        public void Add(string type, string name)
            => this.Parameters.Add(new Parameter(type, name));

        public void Generate(SourceWriter writer)
        {
            writer.Write("(");
            for (var i = 0; i < this.Parameters.Count; i++)
            {
                this.Parameters[i].Generate(writer);
                if (i < this.Parameters.Count - 1)
                {
                    writer.Write(", ");
                }
            }

            writer.Write(")");
        }
    }
}

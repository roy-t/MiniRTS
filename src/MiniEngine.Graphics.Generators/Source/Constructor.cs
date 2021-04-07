using Microsoft.CodeAnalysis;

namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class Constructor : ISource
    {
        public Constructor(string @class)
        {
            this.Class = @class;
            this.Parameters = new ParameterList();
            this.Body = new Body();
        }

        public string Class { get; }
        public ParameterList Parameters { get; }
        public Body Body { get; }

        public Optional<IConstructorChainCall> Chain { get; set; }

        public void Generate(SourceWriter writer)
        {
            writer.Write($"{this.Class}");
            this.Parameters.Generate(writer);
            if (this.Chain.HasValue)
            {
                writer.StartIndent();
                writer.WriteLine();
                writer.EndIndent();
            }
            writer.WriteLine();
            writer.StartScope();
            this.Body.Generate(writer);
            writer.EndScope();
        }
    }
}

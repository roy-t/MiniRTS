namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class BaseConstructorCall : IConstructorChainCall
    {
        public BaseConstructorCall(params string[] arguments)
        {
            this.Arguments = new ArgumentList();
            foreach (var argument in arguments)
            {
                this.Arguments.Arguments.Add(argument);
            }
        }

        public ArgumentList Arguments { get; }

        public void Generate(SourceWriter writer)
        {
            writer.Write(" : base");
            this.Arguments.Generate(writer);
        }
    }
}

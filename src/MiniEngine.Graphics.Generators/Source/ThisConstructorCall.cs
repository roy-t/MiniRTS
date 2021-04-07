namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class ThisConstructorCall : IConstructorChainCall
    {
        public ThisConstructorCall(params string[] arguments)
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
            writer.Write(" : this");
            this.Arguments.Generate(writer);
        }
    }
}

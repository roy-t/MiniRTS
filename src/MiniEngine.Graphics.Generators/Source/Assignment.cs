namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class Assignment : IExpression
    {
        public Assignment(string left, string @operator, string right)
        {
            this.Left = left;
            this.Operator = @operator;
            this.Right = right;
        }

        public string Left { get; }
        public string Operator { get; }
        public string Right { get; }

        public void Generate(SourceWriter writer)
            => writer.WriteLine($"{this.Left} {this.Operator} {this.Right};");
    }
}

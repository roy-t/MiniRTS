namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class Statement : IExpression
    {
        public Statement(string text)
        {
            this.Text = text;
        }

        public string Text { get; }

        public void Generate(SourceWriter writer) => writer.WriteLine($"{this.Text};");
    }
}

using System.Collections.Generic;

namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class Body : IExpression
    {
        public Body()
        {
            this.Expressions = new List<IExpression>();
        }

        public List<IExpression> Expressions { get; }

        public void Generate(SourceWriter writer)
        {
            foreach (var expression in this.Expressions)
            {
                expression.Generate(writer);
            }
        }
    }
}

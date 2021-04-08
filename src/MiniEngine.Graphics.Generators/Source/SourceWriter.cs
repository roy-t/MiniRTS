using System.Text;

namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class SourceWriter
    {
        private readonly StringBuilder Text;
        private int currentIndentation;
        private int targetIndentation;

        public SourceWriter()
        {
            this.Text = new StringBuilder();
            this.currentIndentation = 0;
            this.targetIndentation = 0;
        }

        public void Write(string text)
        {
            this.Indent();
            this.Text.Append(text);
        }

        public void WriteLine(string text)
        {
            this.Indent();
            this.Text.AppendLine(text);
            this.currentIndentation = 0;
        }

        public void WriteLine()
        {
            this.Text.AppendLine();
            this.currentIndentation = 0;
        }

        public void ConditionalEmptyLine(bool condition)
        {
            if (condition)
            {
                this.WriteLine();
            }
        }

        public void WriteModifiers(string[] modifiers)
        {
            if (modifiers.Length > 0)
            {
                var text = string.Join(" ", modifiers);
                this.Write($"{text} ");
            }
        }

        public void StartIndent() => this.targetIndentation++;
        public void EndIndent() => this.targetIndentation--;

        public void StartScope()
        {
            this.WriteLine("{");
            this.StartIndent();
        }

        public void EndScope()
        {
            this.EndIndent();
            this.WriteLine("}");
        }

        public override string ToString() => this.Text.ToString();

        public static string ToString(ISource source)
        {
            var writer = new SourceWriter();
            source.Generate(writer);
            return writer.ToString();
        }

        private void Indent()
        {
            while (this.currentIndentation < this.targetIndentation)
            {
                this.Text.Append("    ");
                this.currentIndentation++;
            }
        }
    }
}

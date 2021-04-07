using System.Text;

namespace MiniEngine.Graphics.Generators.Source
{
    public sealed class SourceWriter
    {
        private readonly StringBuilder Text;
        private int targetIndentation;
        private int currentIndentation;

        public SourceWriter()
        {
            this.Text = new StringBuilder();
            this.targetIndentation = 0;
            this.currentIndentation = 0;
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
            this.targetIndentation = 0;
        }

        public void WriteLine()
        {
            this.Text.AppendLine();
            this.targetIndentation = 0;
        }

        public void WriteModifiers(string[] modifiers)
        {
            var text = string.Join(" ", modifiers);
            this.Write($"{text} ");
        }

        public void StartIndent() => this.currentIndentation++;
        public void EndIndent() => this.currentIndentation--;

        public void StartScope()
        {
            this.WriteLine("{");
            this.StartIndent();
        }

        public void EndScope()
        {
            this.WriteLine("}");
            this.EndIndent();
        }

        public override string ToString() => this.Text.ToString();

        private void Indent()
        {
            while (this.targetIndentation < this.currentIndentation)
            {
                this.Text.Append("    ");
                this.targetIndentation++;
            }
        }
    }
}

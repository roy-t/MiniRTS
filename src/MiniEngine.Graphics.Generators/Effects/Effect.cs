using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using ShaderTools.CodeAnalysis.Hlsl.Syntax;
using ShaderTools.CodeAnalysis.Syntax;
using ShaderTools.CodeAnalysis.Text;

namespace MiniEngine.Graphics.Generators.Effects
{
    public sealed class Effect
    {
        public Effect(string filePath)
        {
            var fileSystem = new ContentFileSystem();
            var sourceCode = System.IO.File.ReadAllText(filePath);
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(new SourceFile(SourceText.From(sourceCode), filePath), null, fileSystem);

            this.PublicProperties = BreadthFirstTypeSearch<VariableDeclarationStatementSyntax>(syntaxTree.Root)
                .Where(node => node.Parent.IsKind(SyntaxKind.CompilationUnit))
                .Select(node => node.Declaration)
                .Where(node => node.Modifiers.Count == 0)
                .Select(node => new EffectProperty(node))
                .Where(property => char.IsUpper(property.Name.First()))
                .ToList();

            this.Name = System.IO.Path.GetFileName(filePath);
            this.Path = filePath;
            this.SourceCode = sourceCode;
        }

        public IReadOnlyList<EffectProperty> PublicProperties { get; }

        public string Name { get; }

        public string Path { get; }

        public string SourceCode { get; }

        public override string ToString() => this.Name;

        private static IReadOnlyList<T> BreadthFirstTypeSearch<T>(params SyntaxNodeBase[] sources)
            where T : SyntaxNodeBase
        {
            var targets = new List<T>();
            var queue = new Queue<SyntaxNodeBase>(sources);

            while (queue.Count > 0)
            {
                var syntaxNode = queue.Dequeue();
                if (syntaxNode is T target)
                {
                    targets.Add(target);
                }

                foreach (var child in syntaxNode.ChildNodes)
                {
                    queue.Enqueue(child);
                }
            }

            return targets;
        }
    }
}

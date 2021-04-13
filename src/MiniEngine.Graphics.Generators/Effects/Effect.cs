using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using ShaderTools.CodeAnalysis.Hlsl.Syntax;
using ShaderTools.CodeAnalysis.Hlsl.Text;
using ShaderTools.CodeAnalysis.Syntax;
using ShaderTools.CodeAnalysis.Text;

namespace MiniEngine.Graphics.Generators.Effects
{
    public sealed class Effect
    {
        public Effect(string filePath)
        {
            var fileSystem = new ContentFileSystem();
            var sourceCode = File.ReadAllText(filePath);
            var syntaxTree = SyntaxFactory.ParseSyntaxTree(new SourceFile(SourceText.From(sourceCode), filePath), null, fileSystem);

            var properties = DescendantNodesOfType<VariableDeclarationStatementSyntax>(syntaxTree.Root)
                .Where(node => node.Parent.IsKind(SyntaxKind.CompilationUnit))
                .Select(node => node.Declaration)
                .Where(node => node.Modifiers.Count == 0)
                .SelectMany(node => EffectProperty.Parse(node))
                .Where(property => char.IsUpper(property.Name.First()))
                .ToList();

            this.PublicProperties = properties.Where(p => p.IsSamplerState() == false).ToList();
            this.Samplers = properties.Where(p => p.IsSamplerState() == true).ToList();

            this.Techniques = DescendantNodesOfType<TechniqueSyntax>(syntaxTree.Root)
                .Select(node => node.Name.ValueText).ToList();

            this.Name = Path.GetFileName(filePath);
            this.SourceCode = sourceCode;
        }

        public IReadOnlyList<EffectProperty> PublicProperties { get; }

        public IReadOnlyList<EffectProperty> Samplers { get; }


        public IReadOnlyList<string> Techniques { get; }

        public string Name { get; }

        public string SourceCode { get; }

        public override string ToString() => this.Name;

        private static IEnumerable<T> DescendantNodesOfType<T>(SyntaxNodeBase root)
            => root.DescendantNodes().OfType<T>();
    }
}

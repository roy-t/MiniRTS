using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using ShaderTools.CodeAnalysis.Hlsl.Syntax;
using ShaderTools.CodeAnalysis.Text;

namespace MiniEngine.Graphics.Generators.Effects
{
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Take parser from https://github.com/tgjones/HlslTools/blob/master/src/ShaderTools.CodeAnalysis.Hlsl/Parser/HlslParser.cs?
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }

            var effectFiles = new List<string>();
            var contentFiles = context.AdditionalFiles.Where(file => file.Path.EndsWith(".mgcb")).ToList().AsReadOnly();
            foreach (var file in contentFiles)
            {
                var directory = Path.GetDirectoryName(file.Path);
                var content = file.GetText(context.CancellationToken);

                var parser = new MGCBParser();
                parser.Parse(content.Lines);

                var effects = parser.Blocks.Where(b => b.Importer.Argument.Equals("EffectImporter"));
                foreach (var effect in effects)
                {
                    var path = Path.GetFullPath(Path.Combine(directory, effect.Build.Argument));
                    effectFiles.Add(path);
                }
            }

            //var sourceCode = File.ReadAllText(testFile);

            //// Build syntax tree.
            //var syntaxTree = SyntaxFactory.ParseSyntaxTree(new SourceFile(SourceText.From(sourceCode), testFile), fileSystem: new TestFileSystem());

            var fileSystem = new ContentFileSystem();
            foreach (var effect in effectFiles)
            {
                var sourceCode = File.ReadAllText(effect);
                var syntaxTree = SyntaxFactory.ParseSyntaxTree(new SourceFile(SourceText.From(sourceCode), effect), null, fileSystem, context.CancellationToken);

                //if (syntaxTree.GetDiagnostics().Any())
                //{
                //    ReportProgress(context, DiagnosticSeverity.Error, "GENG02", "HLSL parser error", $"Could not parse file {effect}");
                //}
                //else
                //{
                //    // syntaxTree.Root.ChildNodes.Where(n => n.IsKind(SyntaxKind.VariableDeclarationStatement))
                //    
                //}

                // TODO: if I add this report it fails to build graphics?
                //ReportProgress(context, effect);
            }
        }


        private static void ReportProgress(GeneratorExecutionContext context, string effectFile)
            => context.ReportDiagnostic(
        Diagnostic.Create(new DiagnosticDescriptor("GENG01", "Effect Generator", $"{effectFile} was used to generate an effect wrapper",
            "Generator", DiagnosticSeverity.Warning, true), null));

        private static void ReportProgress(GeneratorExecutionContext context, DiagnosticSeverity severity, string code, string title, string message)
            => context.ReportDiagnostic(
        Diagnostic.Create(new DiagnosticDescriptor(code, title, message, "Generator", severity, true), null));

    }
}

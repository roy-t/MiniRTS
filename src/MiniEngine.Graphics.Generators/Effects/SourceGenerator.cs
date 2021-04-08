using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using MiniEngine.Graphics.Generators.Source;

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
            //if (!System.Diagnostics.Debugger.IsAttached)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

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

            var generator = new EffectWrapperGenerator();
            foreach (var effectFile in effectFiles)
            {
                var effect = new Effect(effectFile);
                var sourceFile = generator.Generate(effect);
                var sourceText = SourceWriter.ToString(sourceFile);

                context.AddSource(sourceFile.FileName, sourceText);
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

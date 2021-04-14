using System;
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
        public void Initialize(GeneratorInitializationContext context) { }

        public void Execute(GeneratorExecutionContext context)
        {
            var effectFiles = new List<string>();
            var contentFiles = context.AdditionalFiles.Where(file => file.Path.EndsWith(".mgcb")).ToList().AsReadOnly();
            try
            {
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

                Report(context, "GENG01", $"Generated {effectFiles.Count} effect wrappers", DiagnosticSeverity.Warning);
            }
            catch (Exception ex)
            {
                Report(context, "GENG0X", $"Error: {ex.Message}", DiagnosticSeverity.Error);
            }
        }

        private static void Report(GeneratorExecutionContext context, string code, string message, DiagnosticSeverity severity)
            => context.ReportDiagnostic(
                Diagnostic.Create(new DiagnosticDescriptor(code, "Effect Generator", message, "Generator", severity, true), null));
    }
}

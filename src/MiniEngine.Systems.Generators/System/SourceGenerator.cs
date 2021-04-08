using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MiniEngine.Systems.Generators.System
{
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is SyntaxReceiver receiver)
            {
                var generator = new SourceTextGenerator(context.Compilation);
                foreach (var target in receiver.Targets.Values)
                {
                    ReportProgress(context, target);

                    var sourceText = GenerateSourceText(generator, target);
                    context.AddSource($"{target.Class.Identifier}.Generated.cs", sourceText);

                    ReportGeneratorDiagnostics(context, generator);
                }
            }
        }

        private static void ReportProgress(GeneratorExecutionContext context, SystemClass target)
            => context.ReportDiagnostic(
                Diagnostic.Create(new DiagnosticDescriptor("GENS01", "System Generator", $"{target.Class.Identifier} was extended with a system binding that processes {target.Methods.Count} method{(target.Methods.Count > 1 ? "s" : "")}",
                    "Generator", DiagnosticSeverity.Warning, true), null));

        private static SourceText GenerateSourceText(SourceTextGenerator generator, SystemClass target)
            => generator.Generate(target);

        private static void ReportGeneratorDiagnostics(GeneratorExecutionContext context, SourceTextGenerator generator)
            => generator.Diagnostics.ToList().ForEach(diagnostic => context.ReportDiagnostic(diagnostic));
    }
}

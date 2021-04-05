using Microsoft.CodeAnalysis;

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



            ReportProgress(context, "foo.hlsl");
        }


        private static void ReportProgress(GeneratorExecutionContext context, string effectFile)
            => context.ReportDiagnostic(
        Diagnostic.Create(new DiagnosticDescriptor("GENG01", "Effect Generator", $"{effectFile} was used to generate an effect wrapper",
            "Generator", DiagnosticSeverity.Warning, true), null));

    }
}

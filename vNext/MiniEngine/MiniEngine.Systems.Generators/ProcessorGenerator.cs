using Microsoft.CodeAnalysis;

namespace MiniEngine.Systems.Generators
{
    [Generator]
    public class ProcessorGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is MethodReceiver receiver)
            {
                var generator = new CodeGenerator(context.Compilation);
                foreach (var target in receiver.Targets.Values)
                {
                    var diagnostic = Diagnostic.Create(
                                 new DiagnosticDescriptor("GEN001", "Generated Processors", $"Generated bindings for {target.Methods.Count} method(s) in {target.Class.Identifier}", "Generator", DiagnosticSeverity.Warning, true), null);
                    context.ReportDiagnostic(diagnostic);

                    //System.Diagnostics.Debugger.Launch();
                    var sourceText = generator.Generate(target);

                    foreach (var generateorDiagnostic in generator.Diagnostics)
                    {
                        context.ReportDiagnostic(generateorDiagnostic);
                    }

                    context.AddSource($"{target.Class.Identifier}.Generated.cs", sourceText);
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(() => new MethodReceiver());
    }
}

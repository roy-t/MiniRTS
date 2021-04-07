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
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // Take parser from https://github.com/tgjones/HlslTools/blob/master/src/ShaderTools.CodeAnalysis.Hlsl/Parser/HlslParser.cs?
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Launch();
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

            foreach (var effectFile in effectFiles)
            {
                var effect = new Effect(effectFile);

                var foo = GenerateEffectFile(effect);
                Console.WriteLine(foo);
            }

            // TODO: set/apply techniques
            // TODO: actually generate source code
        }

        private string GenerateEffectFile(Effect effect)
        {
            var name = Path.GetFileNameWithoutExtension(effect.Name);
            var file = new Source.File($"{name}.generated.cs");
            file.Usings.Add(new Using("Microsoft.Xna.Framework"));
            file.Usings.Add(new Using("Microsoft.Xna.Framework.Graphics"));
            file.Usings.Add(new Using("MiniEngine.Graphics.Effects"));

            var @namespace = new Namespace("MiniEngine.Graphics.Generated");
            file.Namespaces.Add(@namespace);

            var @class = new Class(name, "public", "sealed");
            @namespace.Classes.Add(@class);

            var constructor = new Constructor(@class.Name, "public");
            @class.Constructors.Add(constructor);
            constructor.Parameters.Add("EffectFactory", "factory");
            constructor.Chain = new Optional<IConstructorChainCall>(new BaseConstructorCall($"factory.Load<{name}>()"));

            foreach (var prop in effect.PublicProperties)
            {
                var fieldName = prop.Name + "Parameter";
                @class.Fields.Add(new Field("EffectParameter", fieldName, "private", "readonly"));
                constructor.Body.Expressions.Add(new Assignment($"this.{fieldName}", "=", $"this.Effect.Parameters[\"{prop.Name}\"]"));

                var property = new Property(prop.GetXNAType(), prop.Name, false, "public");
                @class.Properties.Add(property);
                var propertySetter = new Body();
                property.SetSetter(propertySetter);
                propertySetter.Expressions.Add(new Statement($"this.{fieldName}.SetValue(value)"));
            }

            var writer = new SourceWriter();
            file.Generate(writer);

            return writer.ToString();
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

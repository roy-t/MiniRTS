using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MiniEngine.Systems.Generators
{
    internal class CodeGenerator
    {
        private readonly Compilation Compilation;

        public CodeGenerator(Compilation compilation)
        {
            this.Compilation = compilation;
            this.Diagnostics = new List<Diagnostic>();
        }

        public List<Diagnostic> Diagnostics { get; }

        public SourceText Generate(TargetClass target)
        {
            var nameSpace = Utilities.GetNamespace(this.Compilation, target.Class);

            (var fields, var assignments) = this.Constructor(target);
            var processors = this.Procesors(target);

            var className = $"{ target.Class.Identifier}";
            var bindingClassName = $"{ target.Class.Identifier }Binding";

            var sourceText = SourceText.From($@"
using System;
using System.Collections.Generic;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;

namespace {nameSpace}
{{
    public sealed class {bindingClassName} : ISystemBinding
    {{
        private readonly {className} System;
        {fields}
        public {bindingClassName}({className} system, Dictionary<Type, IComponentContainer> containers)
        {{
            this.System = system;
            {assignments}
        }}

        public void Process()
        {{
            this.System.OnSet();
            {processors}
        }}
    }}

    public partial class {className} : ISystemBindingProvider
    {{
        public ISystemBinding Bind(Dictionary<Type, IComponentContainer> containers)
        {{
            return new { bindingClassName }(this, containers);
        }}
    }}
}}", Encoding.UTF8);

            return sourceText;
        }

        private string GenerateNoComponentProcessor(MethodDeclarationSyntax method)
        {
            var parameters = Utilities.GetMethodParameters(this.Compilation, method);
            if (parameters.Count > 0)
            {
                this.Report($"Method {method.Identifier} is marked with {nameof(ProcessAttribute)} but has parameters, did you mean to used {nameof(ProcessAllAttribute)}?");
                return "";
            }

            var pocessMethod = $@"
            this.System.Process();";
            return pocessMethod;
        }

        private string GenerateProcessMethod(MethodDeclarationSyntax method, string subList)
        {
            var parameters = Utilities.GetMethodParameters(this.Compilation, method);

            if (parameters.Count == 0)
            {
                this.Report($"Method {method.Identifier} has no parameters, in which case the {nameof(ProcessAttribute)} attribute should be used.");
                return "";
            }

            var containers = parameters.Select(p => $"{p}Container.{subList}").ToList();
            var parameterNames = string.Join(", ", Enumerable.Range(0, parameters.Count).Select(i => $"p{i}"));

            var related = "";
            if (parameters.Count > 1)
            {
                related = Environment.NewLine + string.Join(Environment.NewLine,
                    Enumerable.Range(1, parameters.Count).Zip(parameters.Skip(1), (i, p) => new Tuple<int, string>(i, p))
                    .Select(t => $"\t\t\t\tvar p{t.Item1} = this.{t.Item2}Container.Get(p0.Entity);"));
            }

            var assignment = "";
            if (!Utilities.ReturnsVoid(this.Compilation, method))
            {
                assignment = "p0.ChangeState.NextState = ";
            }

            var pocessMethod = $@"
            for (var i = 0; i < this.{containers[0]}.Count; i++)
            {{
                var p0 = this.{containers[0]}[i]; {related}
                {assignment}this.System.Process({parameterNames});
            }}";
            return pocessMethod;
        }

        private string Procesors(TargetClass target)
        {
            var processorList = new List<string>();
            foreach (var processor in target.ProcessAllMethods)
            {
                processorList.Add(this.GenerateProcessMethod(processor, "All"));
            }

            foreach (var processor in target.ProcessChangedMethods)
            {
                processorList.Add(this.GenerateProcessMethod(processor, "Changed"));
            }

            foreach (var processor in target.ProcessNewMethods)
            {
                processorList.Add(this.GenerateProcessMethod(processor, "New"));
            }

            foreach (var processor in target.ProcessMethods)
            {
                processorList.Add(this.GenerateNoComponentProcessor(processor));
            }

            return string.Join(Environment.NewLine, processorList);
        }

        private (string containers, string assignments) Constructor(TargetClass target)
        {
            var allParameters = target.Methods.SelectMany(m => Utilities.GetMethodParameters(this.Compilation, m))
               .Distinct()
               .ToList();

            var containers = string.Join(Environment.NewLine + "\t\t",
                allParameters.Select(type => $"private readonly IComponentContainer<{type}> {type}Container;"));

            var assignments = string.Join(Environment.NewLine + "\t\t\t",
                allParameters.Select(type => $"this.{type}Container = containers[typeof({type})].Specialize<{type}>();"));

            return (containers, assignments);
        }

        private void Report(string message)
        {
            var diagnostic = Diagnostic.Create(
                                new DiagnosticDescriptor("GEN002", "Generated Problem", message, "Generator", DiagnosticSeverity.Error, true), null);
            this.Diagnostics.Add(diagnostic);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MiniEngine.Systems.Generators.System
{
    internal class SourceTextGenerator
    {
        private readonly List<Diagnostic> DiagnosticList;
        private readonly Compilation Compilation;

        public SourceTextGenerator(Compilation compilation)
        {
            this.Compilation = compilation;
            this.DiagnosticList = new List<Diagnostic>();
        }

        public IReadOnlyList<Diagnostic> Diagnostics => this.DiagnosticList;

        public SourceText Generate(SystemClass target)
        {
            var nameSpace = Utilities.GetNamespace(this.Compilation, target.Class);

            (var fields, var assignments) = this.Constructor(target);
            var processors = this.Procesors(target);

            var systemClassName = $"{ target.Class.Identifier}";
            var generatedClassName = $"{ target.Class.Identifier }Binding";

            return SystemBindingSnippet.Format(nameSpace, generatedClassName, systemClassName, fields, assignments, processors);
        }

        private (string fields, string assignments) Constructor(SystemClass target)
        {
            var allParameters = target.Methods.SelectMany(m => Utilities.GetParameters(this.Compilation, m))
               .Distinct()
               .ToList();

            var fields = string.Join(Environment.NewLine + "\t\t",
                allParameters.Select(type => $"private readonly IComponentContainer<{type}> {type}Container;"));

            var assignments = string.Join(Environment.NewLine + "\t\t\t",
                allParameters.Select(type => $"this.{type}Container = containers[typeof({type})].Specialize<{type}>();"));

            return (fields, assignments);
        }

        private string Procesors(SystemClass target)
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

        private string GenerateProcessMethod(MethodDeclarationSyntax method, string subList)
        {
            var parameters = Utilities.GetParameters(this.Compilation, method);

            if (parameters.Count == 0)
            {
                this.Report($"Method {method.Identifier} has no parameters, in which case the {nameof(ProcessAttribute)} attribute should be used.");
                return string.Empty;
            }

            var containers = parameters.Select(p => $"{p}Container.{subList}").ToList();
            var parameterNames = string.Join(", ", Enumerable.Range(0, parameters.Count).Select(i => $"p{i}"));

            var related = string.Empty;
            if (parameters.Count > 1)
            {
                related = Environment.NewLine + string.Join(Environment.NewLine,
                    Enumerable.Range(1, parameters.Count).Zip(parameters.Skip(1), (i, p) => new Tuple<int, string>(i, p))
                    .Select(t => $"\t\t\t\tvar p{t.Item1} = this.{t.Item2}Container.Get(p0.Entity);"));
            }

            var assignment = string.Empty;
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

        private string GenerateNoComponentProcessor(MethodDeclarationSyntax method)
        {
            var parameters = Utilities.GetParameters(this.Compilation, method);
            if (parameters.Count > 0)
            {
                this.Report($"Method {method.Identifier} is marked with {nameof(ProcessAttribute)} but requires one or more parameters, the {nameof(ProcessAllAttribute)} should be used.");
                return string.Empty;
            }

            var pocessMethod = $@"
            this.System.Process();";
            return pocessMethod;
        }

        private void Report(string message)
            => this.DiagnosticList.Add(Diagnostic.Create(
                            new DiagnosticDescriptor("GENS02", "System Genrator Problem", message, "Generator", DiagnosticSeverity.Error, true), null));
    }
}

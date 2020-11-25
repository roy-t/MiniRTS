using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MiniEngine.Systems.Generators
{
    public class CodeGenerator
    {
        private readonly Compilation Compilation;

        public CodeGenerator(Compilation compilation)
        {
            this.Compilation = compilation;
        }

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

        private string GenerateProcessMethod(MethodDeclarationSyntax method, string subList)
        {
            var parameters = Utilities.GetMethodParameters(this.Compilation, method);
            var containers = parameters.Select(p => $"{p}Container.{subList}").ToList();
            var parameterNames = string.Join(", ", Enumerable.Range(0, parameters.Count).Select(i => $"p{i}"));

            var related = "";
            if (parameters.Count > 1)
            {
                related = Environment.NewLine + string.Join(Environment.NewLine,
                    Enumerable.Range(1, parameters.Count).Zip(parameters.Skip(1), (i, p) => new Tuple<int, string>(i, p))
                    .Select(t => $"\t\t\t\tvar p{t.Item1} = this.{t.Item2}Container.Get(p0.Entity);"));
            }

            var pocessMethod = $@"
            for (var i = 0; i < this.{containers[0]}.Count; i++)
            {{
                var p0 = this.{containers[0]}[i]; {related}
                this.System.Process({parameterNames});
            }}";
            return pocessMethod;
        }

        private string Procesors(TargetClass target)
        {
            var processorList = new List<string>();
            foreach (var processor in target.ProcessAllMethods)
            {
                processorList.Add(this.GenerateProcessMethod(processor, "New"));
                processorList.Add(this.GenerateProcessMethod(processor, "Changed"));
                processorList.Add(this.GenerateProcessMethod(processor, "Unchanged"));
            }

            foreach (var processor in target.ProcessChangedMethods)
            {
                processorList.Add(this.GenerateProcessMethod(processor, "Changed"));
            }

            foreach (var processor in target.ProcessNewMethods)
            {
                processorList.Add(this.GenerateProcessMethod(processor, "New"));
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
    }
}

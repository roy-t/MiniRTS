using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace MiniEngine.Systems.Generators.System
{
    internal static class SystemBindingSnippet
    {
        private static IEnumerable<string> Usings()
        {
            return new[]
            {
            "using System;",
            "using System.Collections.Generic;",
            "using MiniEngine.Systems;",
            "using MiniEngine.Systems.Components;"
            };
        }

        public static SourceText Format(IEnumerable<string> additionalUsings, string nameSpace, string generatedClassName, string systemClassName, string fields, string assignments, string processors)
        {
            var usings = string.Join(Environment.NewLine, Usings().Union(additionalUsings));
            var sourceText = SourceText.From($@"
{usings}

namespace {nameSpace}
{{
    public sealed class {generatedClassName} : ISystemBinding
    {{
        private readonly {systemClassName} System;
        {fields}
        public {generatedClassName}({systemClassName} system, Dictionary<Type, IComponentContainer> containers)
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

    public partial class {systemClassName} : ISystemBindingProvider
    {{
        public ISystemBinding Bind(Dictionary<Type, IComponentContainer> containers)
        {{
            return new { generatedClassName }(this, containers);
        }}
    }}
}}", Encoding.UTF8);

            return sourceText;
        }
    }
}

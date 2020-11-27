using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace MiniEngine.Systems.Generators.System
{
    internal static class SystemBindingSnippet
    {
        public static SourceText Format(string nameSpace, string generatedClassName, string systemClassName, string fields, string assignments, string processors)
        {
            var sourceText = SourceText.From($@"
using System;
using System.Collections.Generic;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;

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

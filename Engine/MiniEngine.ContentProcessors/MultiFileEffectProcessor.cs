using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace MiniEngine.ContentProcessors
{
    [ContentProcessor(DisplayName = "MiniEngine :: Multi File Effect Processor")]
    public sealed class MultiFileEffectProcessor : EffectProcessor
    {
        private static readonly Regex IncludeRegex = new Regex("^#include\\s\"(?<file>.*)\"", RegexOptions.Multiline | RegexOptions.Compiled);

        public override CompiledEffectContent Process(EffectContent input, ContentProcessorContext context)
        {
            var dependencies = new HashSet<string>();
            ProcessFile(dependencies, Path.GetDirectoryName(input.Identity.SourceFilename), input.EffectCode);

            foreach (var depencency in dependencies)
            {
                context.AddDependency(depencency);
            }

            return base.Process(input, context);
        }

        private static HashSet<string> ProcessFile(HashSet<string> dependencies, string effectDirectory, string effectCode)
        {
            var includes = IncludeRegex.Matches(effectCode).Select(m => m.Groups["file"].Value);

            foreach (var include in includes)
            {
                var path = Path.GetFullPath(Path.Combine(effectDirectory, include));
                if (!dependencies.Contains(path))
                {
                    dependencies.Add(path);
                    ProcessFile(dependencies, Path.GetDirectoryName(path), File.ReadAllText(path));
                }
            }

            return dependencies;
        }
    }
}

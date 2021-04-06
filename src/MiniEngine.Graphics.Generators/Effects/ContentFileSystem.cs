using System.IO;
using Microsoft.CodeAnalysis.Text;
using ShaderTools.CodeAnalysis.Hlsl.Text;

namespace MiniEngine.Graphics.Generators.Effects
{
    public sealed class ContentFileSystem : IIncludeFileSystem
    {
        public bool TryGetFile(string path, out SourceText text)
        {
            var sourceCode = File.ReadAllText(path);
            text = SourceText.From(sourceCode);
            return true;
        }
    }
}

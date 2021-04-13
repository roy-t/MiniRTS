using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace MiniEngine.ContentPipeline.Effects
{
    public sealed class HotEffectContent : CompiledEffectContent
    {
        public HotEffectContent(string sourceFile, byte[] effectCode)
            : base(effectCode)
        {
            this.SourceFile = sourceFile;
        }

        public string SourceFile { get; }
    }
}

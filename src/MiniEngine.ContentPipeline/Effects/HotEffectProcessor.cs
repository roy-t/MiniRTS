using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace MiniEngine.ContentPipeline.Effects
{
    [ContentProcessor(DisplayName = "HotEffectProcessor - MiniEngine")]
    public sealed class HotEffectProcessor : ContentProcessor<EffectContent, HotEffectContent>
    {
        private readonly EffectProcessor BaseProcessor;

        public HotEffectProcessor()
        {
            this.BaseProcessor = new EffectProcessor();
        }

        public override HotEffectContent Process(EffectContent input, ContentProcessorContext context)
        {
            var effect = this.BaseProcessor.Process(input, context);
            return new HotEffectContent(Path.GetFullPath(context.SourceIdentity.SourceFilename), effect.GetEffectCode());
        }
    }
}

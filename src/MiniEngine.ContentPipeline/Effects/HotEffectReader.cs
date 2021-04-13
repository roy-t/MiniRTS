using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.ContentPipeline.Effects
{
    internal sealed class HotEffectReader : ContentTypeReader<Effect>
    {
        protected override Effect Read(ContentReader input, Effect _)
        {
            var sourceFile = input.ReadString();
            var length = input.ReadInt32();
            var effectCode = input.ReadBytes(length);

            var effect = new Effect(input.GetGraphicsDevice(), effectCode);
            effect.Tag = sourceFile;

            return effect;
        }
    }
}
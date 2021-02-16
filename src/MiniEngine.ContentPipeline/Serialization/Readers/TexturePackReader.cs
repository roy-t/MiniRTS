using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.ContentPipeline.Packs;

namespace MiniEngine.ContentPipeline.Serialization.Readers
{
    internal sealed class TexturePackReader : ContentTypeReader<TexturePack>
    {
        protected override TexturePack Read(ContentReader input, TexturePack existingInstance)
        {
            var name = input.ReadString();
            var pack = new TexturePack(name);

            var count = input.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var texture = input.ReadString();
                pack.Names.Add(texture);
                pack.Textures.Add(input.ContentManager.Load<Texture2D>(texture));
            }

            return pack;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace MiniEngine.ContentPipeline.HDR
{
    [ContentImporter(".hdr", DisplayName = "HDR importer - MiniEngine", DefaultProcessor = "TextureProcessor")]
    public sealed class HDRImporter : ContentImporter<TextureContent>
    {
        public override TextureContent Import(string filename, ContentImporterContext context)
        {
            var output = new Texture2DContent() { Identity = new ContentIdentity(filename) };

            var width = 16;
            var height = 16;

            var face = new PixelBitmapContent<Vector4>(width, height);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    face.SetPixel(x, y, Color.Magenta.ToVector4());
                }
            }

            output.Faces[0].Add(face);

            return output;
        }
    }
}

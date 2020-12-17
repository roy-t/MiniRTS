using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.ContentPipeline.Shared;

namespace MiniEngine.ContentPipeline.Serialization.Readers
{
    internal sealed class MaterialReader : ContentTypeReader<Material>
    {
        protected override Material Read(ContentReader input, Material _)
            => Read(input);

        public static Material Read(ContentReader input)
        {
            var albedo = input.ReadExternalReference<Texture2D>();
            var normal = input.ReadExternalReference<Texture2D>();
            var metalicness = input.ReadSingle();
            var roughness = input.ReadSingle();
            var ambientOcclusion = input.ReadSingle();

            return new Material(albedo, normal, metalicness, roughness, ambientOcclusion);
        }
    }
}

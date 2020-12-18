using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace MiniEngine.ContentPipeline.Serialization
{
    internal sealed class MaterialContent
    {
        public MaterialContent(ExternalReference<TextureContent> albedo, ExternalReference<TextureContent> normal, ExternalReference<TextureContent> metalicness, ExternalReference<TextureContent> roughness, ExternalReference<TextureContent> ambientOcclusion)
        {
            this.Albedo = albedo;
            this.Normal = normal;
            this.Metalicness = metalicness;
            this.Roughness = roughness;
            this.AmbientOcclusion = ambientOcclusion;
        }

        public ExternalReference<TextureContent> Albedo { get; }

        public ExternalReference<TextureContent> Normal { get; }

        public ExternalReference<TextureContent> Metalicness { get; }

        public ExternalReference<TextureContent> Roughness { get; }

        public ExternalReference<TextureContent> AmbientOcclusion { get; }
    }
}

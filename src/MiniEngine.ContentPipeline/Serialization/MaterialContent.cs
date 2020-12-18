using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace MiniEngine.ContentPipeline.Serialization
{
    internal sealed class MaterialContent
    {
        public MaterialContent(ExternalReference<TextureContent> albedo, ExternalReference<TextureContent> normal, float metalicness, float roughness, float ambientOcclusion)
        {
            this.Albedo = albedo;
            this.Normal = normal;
            this.Metalicness = metalicness;
            this.Roughness = roughness;
            this.AmbientOcclusion = ambientOcclusion;
        }

        public ExternalReference<TextureContent> Albedo { get; }

        public ExternalReference<TextureContent> Normal { get; }

        public float Metalicness { get; }

        public float Roughness { get; }

        public float AmbientOcclusion { get; }
    }
}

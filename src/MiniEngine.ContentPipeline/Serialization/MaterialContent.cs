using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.ContentPipeline.Serialization
{
    internal sealed class MaterialContent
    {
        public MaterialContent(ExternalReference<Texture2D> albedo, ExternalReference<Texture2D> normal, float metalicness, float roughness, float ambientOcclusion)
        {
            this.Albedo = albedo;
            this.Normal = normal;
            this.Metalicness = metalicness;
            this.Roughness = roughness;
            this.AmbientOcclusion = ambientOcclusion;
        }

        public ExternalReference<Texture2D> Albedo { get; }

        public ExternalReference<Texture2D> Normal { get; }

        public float Metalicness { get; }

        public float Roughness { get; }

        public float AmbientOcclusion { get; }
    }
}

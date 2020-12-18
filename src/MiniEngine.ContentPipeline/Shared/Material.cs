using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.ContentPipeline.Shared
{
    public sealed class Material
    {
        public Material(Texture2D albedo, Texture2D normal, Texture2D metalicness, Texture2D roughness, Texture2D ambientOcclusion)
        {
            this.Albedo = albedo;
            this.Normal = normal;

            this.Metalicness = metalicness;
            this.Roughness = roughness;
            this.AmbientOcclusion = ambientOcclusion;
        }

        public Texture2D Albedo { get; set; }

        public Texture2D Normal { get; set; }

        public Texture2D Metalicness { get; set; }

        public Texture2D Roughness { get; set; }

        public Texture2D AmbientOcclusion { get; set; }
    }
}

using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class Material
    {
        public Material(Texture2D diffuse, Texture2D normal, float metalicness, float roughness, float ambientOcclusion = 1.0f)
        {
            this.Diffuse = diffuse;
            this.Normal = normal;

            this.Metalicness = metalicness;
            this.Roughness = roughness;
            this.AmbientOcclusion = ambientOcclusion;
        }

        public Texture2D Diffuse { get; set; }

        public Texture2D Normal { get; set; }

        public float Metalicness { get; set; }

        public float Roughness { get; set; }

        public float AmbientOcclusion { get; set; }
    }
}

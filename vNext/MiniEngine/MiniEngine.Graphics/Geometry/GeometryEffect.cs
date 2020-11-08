using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class GeometryEffect : EffectWrapper
    {
        public GeometryEffect(EffectFactory factory) : base(factory.Load<GeometryEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["GeometryTechnique"];
        }

        public Matrix World { set => this.Effect.Parameters["World"].SetValue(value); }

        public Matrix WorldViewProjection { set => this.Effect.Parameters["WorldViewProjection"].SetValue(value); }

        public Texture2D Diffuse { set => this.Effect.Parameters["Diffuse"].SetValue(value); }

        public Texture2D Normal { set => this.Effect.Parameters["Normal"].SetValue(value); }

        public float Metalicness { set => this.Effect.Parameters["Metalicness"].SetValue(value); }

        public float Roughness { set => this.Effect.Parameters["Roughness"].SetValue(value); }

        public float AmbientOcclusion { set => this.Effect.Parameters["AmbientOcclusion"].SetValue(value); }
    }
}

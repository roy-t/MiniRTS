using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class GeometryEffect : EffectWrapper
    {
        public GeometryEffect(Effect effect) : base(effect)
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["GeometryTechnique"];
        }

        public Matrix World { set => this.Effect.Parameters["World"].SetValue(value); }

        public Matrix WorldViewProjection { set => this.Effect.Parameters["WorldViewProjection"].SetValue(value); }

        public Texture2D Diffuse { set => this.Effect.Parameters["Diffuse"].SetValue(value); }
        public Texture2D Normal { set => this.Effect.Parameters["Normal"].SetValue(value); }

        public void Apply() => this.ApplyPass();
    }
}

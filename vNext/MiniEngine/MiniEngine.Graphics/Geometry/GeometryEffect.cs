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

        public Matrix WorldViewProjection { set => this.Effect.Parameters["WorldViewProjection"].SetValue(value); }

        public void Apply() => this.ApplyPass();
    }
}

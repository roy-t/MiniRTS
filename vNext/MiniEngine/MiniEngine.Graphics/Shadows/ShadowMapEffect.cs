using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Shadows
{
    public sealed class ShadowMapEffect : EffectWrapper
    {
        private readonly EffectParameter WorldViewProjectionParameter;

        public ShadowMapEffect(EffectFactory factory) : base(factory.Load<ShadowMapEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ShadowMapTechnique"];

            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
        }

        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }
    }
}

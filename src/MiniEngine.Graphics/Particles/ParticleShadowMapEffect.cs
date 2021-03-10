using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Particles
{
    public sealed class ParticleShadowMapEffect : EffectWrapper
    {
        private readonly EffectParameter WorldViewProjectionParameter;
        private readonly EffectParameter DataParameter;

        public ParticleShadowMapEffect(EffectFactory factory) : base(factory.Load<ParticleShadowMapEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ParticleShadowMapTechnique"];
            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
            this.DataParameter = this.Effect.Parameters["Data"];
        }


        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }

        public Texture2D Data { set => this.DataParameter.SetValue(value); }
    }
}

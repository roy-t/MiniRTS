using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Particles
{
    public sealed class ParticleEffect : EffectWrapper
    {
        private readonly EffectParameter WorldViewProjectionParameter;
        private readonly EffectParameter ViewParameter;

        public ParticleEffect(EffectFactory factory) : base(factory.Load<ParticleEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ParticleTechnique"];
            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
            this.ViewParameter = this.Effect.Parameters["View"];
        }


        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }

        public Matrix View { set => this.ViewParameter.SetValue(value); }
    }
}

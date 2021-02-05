using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Particles
{
    public sealed class ParticleEffect : EffectWrapper
    {
        private readonly EffectParameter TextureParameter;
        private readonly EffectParameter WorldViewProjectionParameter;

        public ParticleEffect(EffectFactory factory) : base(factory.Load<ParticleEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ParticleTechnique"];
            this.TextureParameter = this.Effect.Parameters["Texture"];
            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
        }

        public Texture2D Texture { set => this.TextureParameter.SetValue(value); }

        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }
    }
}

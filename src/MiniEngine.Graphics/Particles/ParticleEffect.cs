using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Particles
{
    public sealed class ParticleEffect : EffectWrapper
    {
        private readonly EffectParameter WorldViewProjectionParameter;
        private readonly EffectParameter ViewParameter;
        private readonly EffectParameter MetalicnessParameter;
        private readonly EffectParameter RoughnessParameter;

        public ParticleEffect(EffectFactory factory) : base(factory.Load<ParticleEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ParticleTechnique"];
            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
            this.ViewParameter = this.Effect.Parameters["View"];
            this.MetalicnessParameter = this.Effect.Parameters["Metalicness"];
            this.RoughnessParameter = this.Effect.Parameters["Roughness"];
        }


        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }

        public Matrix View { set => this.ViewParameter.SetValue(value); }

        public float Metalicness { set => this.MetalicnessParameter.SetValue(value); }

        public float Roughness { set => this.RoughnessParameter.SetValue(value); }
    }
}

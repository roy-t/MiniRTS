using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects.Wrappers
{
    public sealed class AverageParticlesEffect : EffectWrapper
    {
        public AverageParticlesEffect()
        {
        }

        public AverageParticlesEffect(Effect effect)
        {
            this.Wrap(effect);
        }
      
        public Texture2D ColorMap
        {
            set => this.effect.Parameters["ColorMap"].SetValue(value);
        }

        public Texture2D WeightMap
        {
            set => this.effect.Parameters["WeightMap"].SetValue(value);
        }

        public void Apply() => this.ApplyPass();
    }
}
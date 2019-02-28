using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects
{
    public sealed class WeightedParticlesEffect : EffectWrapper
    {
        public WeightedParticlesEffect()
        {

        }

        public WeightedParticlesEffect(Effect effect)
        {
            this.Wrap(effect);
        }

        public Matrix World
        {
            set => this.effect.Parameters["World"].SetValue(value);
        }

        public Matrix View
        {
            set => this.effect.Parameters["View"].SetValue(value);
        }

        public Matrix Projection
        {
            set => this.effect.Parameters["Projection"].SetValue(value);
        }

        public Texture2D DepthMap
        {
            set => this.effect.Parameters["DepthMap"].SetValue(value);
        }

        public Texture2D DiffuseMap
        {
            set => this.effect.Parameters["Texture"].SetValue(value);
        }

        public Vector4 Tint
        {
            set => this.effect.Parameters["Tint"].SetValue(value);
        }        

        public void Apply() => this.ApplyPass();
    }
}

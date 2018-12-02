using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects
{
    public sealed class AmbientLightEffect : EffectWrapper
    {
        public AmbientLightEffect()
        {

        }

        public AmbientLightEffect(Effect effect)
        {
            this.Wrap(effect);
        }

        public Color Color
        {
            set => this.effect.Parameters["Color"].SetValue(value.ToVector3());
        }   

        public Texture2D AmbientOcclusionMap
        {
            set => this.effect.Parameters["AmbientOcclusionMap"].SetValue(value);
        }

        public void Apply() => this.ApplyPass();
    }
}

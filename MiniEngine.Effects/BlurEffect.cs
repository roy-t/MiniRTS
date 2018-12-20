using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects
{
    public sealed class BlurEffect : EffectWrapper
    {
        public BlurEffect()
        {
        }

        public BlurEffect(Effect blurEffect)
        {
            this.Wrap(blurEffect);
        }
      
        public Texture2D SourceMap
        {
            set => this.effect.Parameters["SourceMap"].SetValue(value);
        }

        public void Apply() => this.ApplyPass();
    }
}
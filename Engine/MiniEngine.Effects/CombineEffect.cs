using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects
{
    public sealed class CombineEffect : EffectWrapper
    {
        public CombineEffect()
        {
        }

        public CombineEffect(Effect combineEffect)
        {
            this.Wrap(combineEffect);
        }

        public Texture2D DiffuseMap
        {
            set => this.effect.Parameters["DiffuseMap"].SetValue(value);
        }

        public Texture2D LightMap
        {
            set => this.effect.Parameters["LightMap"].SetValue(value);
        }

        public void Apply() => this.ApplyPass();
    }
}
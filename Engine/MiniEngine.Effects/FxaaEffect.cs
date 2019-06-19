using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects
{
    public sealed class FxaaEffect : EffectWrapper
    {
        public FxaaEffect()
        {
        }

        public FxaaEffect(Effect combineEffect)
        {
            this.Wrap(combineEffect);
        }

        public float Strength
        {
            set => this.effect.Parameters["Strength"].SetValue(value);
        }

        public float ScaleX
        {
            set => this.effect.Parameters["ScaleX"].SetValue(value);
        }

        public float ScaleY
        {
            set => this.effect.Parameters["ScaleY"].SetValue(value);
        }

        public Texture2D DiffuseMap
        {
            set => this.effect.Parameters["DiffuseMap"].SetValue(value);
        }

        public Texture2D NormalMap
        {
            set => this.effect.Parameters["NormalMap"].SetValue(value);
        }

        public void Apply() => this.ApplyPass();
    }
}
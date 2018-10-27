using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects
{
    public sealed class CopyEffect : EffectWrapper
    {
        public CopyEffect()
        {
        }

        public CopyEffect(Effect copyEffect)
        {
            this.Wrap(copyEffect);
        }

        public Texture2D DiffuseMap
        {
            set => this.effect.Parameters["DiffuseMap"].SetValue(value);
        }

        public void Apply() => this.ApplyPass();
    }
}
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering.Effects
{
    public sealed class CopyEffect : EffectWrapper
    {
        public CopyEffect()
        {
        }

        public CopyEffect(Effect copyEffect)
        {
            Wrap(copyEffect);
        }

        public Texture2D DiffuseMap
        {
            set => this.effect.Parameters["DiffuseMap"].SetValue(value);
        }

        public void Apply()
        {
            ApplyPass();
        }
    }
}
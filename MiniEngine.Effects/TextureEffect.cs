using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects
{
    public sealed class TextureEffect : EffectWrapper
    {
        public TextureEffect()
        {
        }

        public TextureEffect(Effect effect)
        {
            this.Wrap(effect);
        }
        
        public Texture2D Texture
        {
            set => this.effect.Parameters["Texture"].SetValue(value);
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

        public void Apply() => this.ApplyPass();
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects
{
    public sealed class ColorEffect : EffectWrapper
    {
        public ColorEffect()
        {
        }

        public ColorEffect(Effect effect)
        {
            this.Wrap(effect);
        }
        
        public Color Color
        {
            set => this.effect.Parameters["Color"].SetValue(value.ToVector4());
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

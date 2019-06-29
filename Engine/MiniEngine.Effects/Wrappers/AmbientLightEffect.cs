using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects.Wrappers
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

        public Texture2D NormalMap
        {
            set => this.effect.Parameters["NormalMap"].SetValue(value);
        }
    
        public Texture2D DepthMap
        {
            set => this.effect.Parameters["DepthMap"].SetValue(value);
        }

        public Texture2D FilteredDepthMap
        {
            set => this.effect.Parameters["FilteredDepthMap"].SetValue(value);
        }

        public Texture2D NoiseMap
        {
            set => this.effect.Parameters["NoiseMap"].SetValue(value);
        }

        public Matrix View
        {
            set => this.effect.Parameters["View"].SetValue(value);
        }

        public Matrix Projection
        {
            set => this.effect.Parameters["Projection"].SetValue(value);
        }

        public Matrix InverseViewProjection
        {
            set => this.effect.Parameters["InverseViewProjection"].SetValue(value);
        }

        public Color Color
        {
            set => this.effect.Parameters["Color"].SetValue(value.ToVector3());
        }     

        public Vector3[] Kernel
        {
            set => this.effect.Parameters["Kernel"].SetValue(value);
        }

        public float NormalOffset
        {
            set => this.effect.Parameters["NormalOffset"].SetValue(value);
        }

        public void Apply() => this.ApplyPass();
    }
}

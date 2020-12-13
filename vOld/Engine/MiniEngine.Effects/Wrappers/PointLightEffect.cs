using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects.Wrappers
{
    public sealed class PointLightEffect : EffectWrapper
    {
        public PointLightEffect()
        {

        }

        public PointLightEffect(Effect effect)
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

        public Matrix World
        {
            set => this.effect.Parameters["World"].SetValue(value);
        }

        public Vector3 LightPosition
        {
            set => this.effect.Parameters["LightPosition"].SetValue(value);
        }

        public Color Color
        {
            set => this.effect.Parameters["Color"].SetValue(value.ToVector3());
        }

        public float Radius
        {
            set => this.effect.Parameters["Radius"].SetValue(value);
        }

        public float Intensity
        {
            set => this.effect.Parameters["Intensity"].SetValue(value);
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

        public Vector3 CameraPosition
        {
            set => this.effect.Parameters["CameraPosition"].SetValue(value);
        }

        public void Apply() => this.ApplyPass();
    }
}

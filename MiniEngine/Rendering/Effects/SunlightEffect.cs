using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering.Effects
{
    public sealed class SunlightEffect : EffectWrapper
    {
        public SunlightEffect()
        {

        }

        public SunlightEffect(Effect effect)
        {
            Wrap(effect);
        }

        public Texture2D NormalMap
        {
            set => this.effect.Parameters["NormalMap"].SetValue(value);
        }

        public Texture2D DepthMap
        {
            set => this.effect.Parameters["DepthMap"].SetValue(value);
        }

        public Vector3 SurfaceToLightVector
        {
            set => this.effect.Parameters["SurfaceToLightVector"].SetValue(value);
        }

        public Color LightColor
        {
            set => this.effect.Parameters["LightColor"].SetValue(value.ToVector3());
        }

        public Matrix InverseViewProjection
        {
            set => this.effect.Parameters["InverseViewProjection"].SetValue(value);
        }

        public Vector3 CameraPosition
        {
            set => this.effect.Parameters["CameraPosition"].SetValue(value);
        }

        public Texture2D ShadowMap
        {
            set => this.effect.Parameters["ShadowMap"].SetValue(value);
        }

        public Texture2D ColorMap
        {
            set => this.effect.Parameters["ColorMap"].SetValue(value);
        }

        public Matrix ShadowMatrix
        {
            set => this.effect.Parameters["ShadowMatrix"].SetValue(value);
        }

        public float[] CascadeSplits
        {
            set => this.effect.Parameters["CascadeSplits"].SetValue(new Vector4(value[0], value[1], value[2], value[3]));
        }

        public Vector4[] CascadeOffsets
        {
            set => this.effect.Parameters["CascadeOffsets"].SetValue(value);
        }

        public Vector4[] CascadeScales
        {
            set => this.effect.Parameters["CascadeScales"].SetValue(value);
        }

        public void Apply() => ApplyPass();
    }
}

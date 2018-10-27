using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MiniEngine.Effects
{
    public sealed class ShadowCastingLightEffect : EffectWrapper
    {

        public ShadowCastingLightEffect()
        {

        }

        public ShadowCastingLightEffect(Effect effect)
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

        public Vector3 LightDirection
        {
            set => this.effect.Parameters["LightDirection"].SetValue(value);
        }

        public Vector3 LightPosition
        {
            set => this.effect.Parameters["LightPosition"].SetValue(value);
        }

        public Color Color
        {
            set => this.effect.Parameters["Color"].SetValue(value.ToVector3());
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

        public Matrix LightViewProjection
        {
            set => this.effect.Parameters["LightViewProjection"].SetValue(value);
        }

        public void Apply() => this.ApplyPass();
    }
}

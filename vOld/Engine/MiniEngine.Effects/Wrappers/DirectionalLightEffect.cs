using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects.Wrappers
{
    public sealed class DirectionalLightEffect : EffectWrapper
    {
        public DirectionalLightEffect()
        {

        }

        public DirectionalLightEffect(Effect effect)
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

        public Color Color
        {
            set => this.effect.Parameters["Color"].SetValue(value.ToVector3());
        }

        public Vector3 CameraPosition
        {
            set => this.effect.Parameters["CameraPosition"].SetValue(value);
        }

        public Matrix InverseViewProjection
        {
            set => this.effect.Parameters["InverseViewProjection"].SetValue(value);
        }

        public void Apply() => this.ApplyPass();
    }
}

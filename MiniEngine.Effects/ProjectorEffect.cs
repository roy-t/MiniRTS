using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects
{
    public sealed class ProjectorEffect : EffectWrapper
    {
        public ProjectorEffect()
        {
        }

        public ProjectorEffect(Effect effect)
        {
            this.Wrap(effect);
        }

        public float MaxDistance
        {
            set => this.effect.Parameters["MaxDistance"].SetValue(value);
        }

        public Color Tint
        {
            set => this.effect.Parameters["Tint"].SetValue(value.ToVector4());
        }

        public Vector3 ProjectorPosition
        {
            set => this.effect.Parameters["ProjectorPosition"].SetValue(value);
        }

        public Vector3 ProjectorForward
        {
            set => this.effect.Parameters["ProjectorForward"].SetValue(value);
        }

        public Texture2D DepthMap
        {
            set => this.effect.Parameters["DepthMap"].SetValue(value);
        }

        public Matrix InverseViewProjection
        {
            set => this.effect.Parameters["InverseViewProjection"].SetValue(value);
        }       

        public Texture2D ProjectorMap
        {
            set => this.effect.Parameters["ProjectorMap"].SetValue(value);
        }

        public Matrix ProjectorViewProjection
        {
            set => this.effect.Parameters["ProjectorViewProjection"].SetValue(value);
        }

        public void Apply() => this.ApplyPass();
    }
}

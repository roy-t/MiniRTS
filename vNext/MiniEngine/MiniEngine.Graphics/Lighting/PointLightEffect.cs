using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Lighting
{
    public sealed class PointLightEffect : EffectWrapper
    {
        public PointLightEffect(Effect effect)
            : base(effect)
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["PointLightTechnique"];
        }

        public Texture2D Normal { set => this.Effect.Parameters["Normal"].SetValue(value); }

        public Texture2D Depth { set => this.Effect.Parameters["Depth"].SetValue(value); }

        public Texture2D Material { set => this.Effect.Parameters["Material"].SetValue(value); }

        public Matrix InverseViewProjection { set => this.Effect.Parameters["InverseViewProjection"].SetValue(value); }

        public Vector3 Position { set => this.Effect.Parameters["Position"].SetValue(value); }

        public Color Color { set => this.Effect.Parameters["Color"].SetValue(value.ToVector4()); }
    }
}

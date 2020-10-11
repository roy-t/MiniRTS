using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Lighting
{
    public sealed class AmbientLightEffect : EffectWrapper
    {
        public AmbientLightEffect(Effect effect) : base(effect)
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["AmbientLightTechnique"];
        }

        public Texture2D Normal { set => this.Effect.Parameters["Normal"].SetValue(value); }

        public Texture2D Depth { set => this.Effect.Parameters["Depth"].SetValue(value); }

        public Color Color { set => this.Effect.Parameters["Color"].SetValue(value.ToVector4()); }
    }
}

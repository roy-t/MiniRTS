using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Immediate
{
    public sealed class ImmediateEffect : EffectWrapper
    {
        public ImmediateEffect(EffectFactory factory) : base(factory.Load<ImmediateEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ImmediateTechnique"];
        }

        public Texture2D Diffuse { set => this.Effect.Parameters["Diffuse"].SetValue(value); }

        public Matrix WorldViewProjection { set => this.Effect.Parameters["WorldViewProjection"].SetValue(value); }

        public bool ConvertColorsToLinear { set => this.Effect.Parameters["ConvertColorsToLinear"].SetValue(value); }
    }
}

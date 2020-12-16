using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Immediate
{
    public sealed class ImmediateEffect : EffectWrapper
    {
        private readonly EffectParameter ColorParameter;
        private readonly EffectParameter WorldViewProjectionParameter;
        private readonly EffectParameter ConvertColorsToLinearParameter;

        public ImmediateEffect(EffectFactory factory) : base(factory.Load<ImmediateEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ImmediateTechnique"];

            this.ColorParameter = this.Effect.Parameters["Color"];
            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
            this.ConvertColorsToLinearParameter = this.Effect.Parameters["ConvertColorsToLinear"];
        }

        public Texture2D Color { set => this.ColorParameter.SetValue(value); }

        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }

        public bool ConvertColorsToLinear { set => this.ConvertColorsToLinearParameter.SetValue(value); }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Immediate
{
    public sealed class ImmediateEffect : EffectWrapper
    {
        private readonly EffectParameter DiffuseParameter;
        private readonly EffectParameter WorldViewProjectionParameter;
        private readonly EffectParameter ConvertColorsToLinearParameter;

        public ImmediateEffect(EffectFactory factory) : base(factory.Load<ImmediateEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ImmediateTechnique"];

            this.DiffuseParameter = this.Effect.Parameters["Diffuse"];
            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
            this.ConvertColorsToLinearParameter = this.Effect.Parameters["ConvertColorsToLinear"];
        }

        public Texture2D Diffuse { set => this.DiffuseParameter.SetValue(value); }

        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }

        public bool ConvertColorsToLinear { set => this.ConvertColorsToLinearParameter.SetValue(value); }
    }
}

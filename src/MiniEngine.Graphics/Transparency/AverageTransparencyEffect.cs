using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;


namespace MiniEngine.Graphics.Transparency
{
    public sealed class AverageTransparencyEffect : EffectWrapper
    {
        private readonly EffectParameter AlbedoParameter;
        private readonly EffectParameter WeightsParameter;

        public AverageTransparencyEffect(EffectFactory factory) : base(factory.Load<AverageTransparencyEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["AverageTransparencyTechnique"];
            this.AlbedoParameter = this.Effect.Parameters["Albedo"];
            this.WeightsParameter = this.Effect.Parameters["Weights"];
        }

        public Texture2D Albedo { set => this.AlbedoParameter.SetValue(value); }

        public Texture2D Weights { set => this.WeightsParameter.SetValue(value); }
    }
}


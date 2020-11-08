using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.PostProcess
{
    public sealed class BlurEffect : EffectWrapper
    {
        private readonly EffectParameter DiffuseParameter;
        private readonly EffectParameter SampleRadiusParameter;

        public BlurEffect(EffectFactory factory) : base(factory.Load<BlurEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["BlurTechnique"];

            this.DiffuseParameter = this.Effect.Parameters["Diffuse"];
            this.SampleRadiusParameter = this.Effect.Parameters["SampleRadius"];
        }

        public Texture2D Diffuse { set => this.DiffuseParameter.SetValue(value); }
        public float SampleRadius { set => this.SampleRadiusParameter.SetValue(value); }
    }
}

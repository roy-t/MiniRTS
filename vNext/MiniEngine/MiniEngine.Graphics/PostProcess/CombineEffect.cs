using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.PostProcess
{
    public sealed class CombineEffect : EffectWrapper
    {
        private readonly EffectParameter LightParameter;

        public CombineEffect(EffectFactory factory) : base(factory.Load<CombineEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["CombineTechnique"];

            this.LightParameter = this.Effect.Parameters["Light"];
        }

        public Texture2D Light { set => this.LightParameter.SetValue(value); }
    }
}

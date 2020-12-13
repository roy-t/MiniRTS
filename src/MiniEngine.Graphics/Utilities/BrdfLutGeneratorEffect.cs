using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Utilities
{
    public sealed class BrdfLutGeneratorEffect : EffectWrapper
    {
        public BrdfLutGeneratorEffect(EffectFactory factory) : base(factory.Load<BrdfLutGeneratorEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["BrdfLutGeneratorTechnique"];
        }
    }
}

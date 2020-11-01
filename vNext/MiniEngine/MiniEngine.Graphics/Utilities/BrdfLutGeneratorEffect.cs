using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Utilities
{
    public sealed class BrdfLutGeneratorEffect : EffectWrapper
    {
        public BrdfLutGeneratorEffect(Effect effect)
            : base(effect)
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["BrdfLutGeneratorTechnique"];
        }
    }
}
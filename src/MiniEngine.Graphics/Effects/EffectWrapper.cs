using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;

namespace MiniEngine.Graphics.Effects
{
    [Content]
    public abstract class EffectWrapper : IEffect
    {
        protected readonly Effect Effect;

        public EffectWrapper(Effect effect)
        {
            this.Effect = effect;
        }

        public void Apply() => this.Effect.CurrentTechnique.Passes[0].Apply();
    }
}

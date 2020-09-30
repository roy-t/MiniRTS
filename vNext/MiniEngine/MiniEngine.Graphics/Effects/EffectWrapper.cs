using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Effects
{
    public abstract class EffectWrapper
    {
        protected readonly Effect Effect;

        public EffectWrapper(Effect effect)
        {
            this.Effect = effect;
        }

        protected void ApplyPass() => this.Effect.CurrentTechnique.Passes[0].Apply();
    }
}

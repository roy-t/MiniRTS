using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects
{
    public abstract class EffectWrapper
    {
        protected Effect effect;

        public void Wrap(Effect effect) => this.effect = effect;

        protected void ApplyPass() => this.effect.CurrentTechnique.Passes[0].Apply();
    }
}
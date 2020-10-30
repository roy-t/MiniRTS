using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Effects
{
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

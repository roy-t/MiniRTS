using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;

namespace MiniEngine.Graphics.Effects
{
    [Content]
    public abstract class EffectWrapper
    {
        protected readonly Effect Effect;

        public EffectWrapper(Effect effect)
        {
            this.Effect = effect;
        }
    }
}

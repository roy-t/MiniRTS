using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.PostProcess
{
    public sealed class CombineEffect : EffectWrapper
    {
        public CombineEffect(EffectFactory factory) : base(factory.Load<CombineEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["CombineTechnique"];
        }

        public Texture2D Light { set => this.Effect.Parameters["Light"].SetValue(value); }
    }
}

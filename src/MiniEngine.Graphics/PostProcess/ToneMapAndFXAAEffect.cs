using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.PostProcess
{
    public sealed class ToneMapAndFXAAEffect : EffectWrapper
    {
        private readonly EffectParameter ColorParameter;

        public ToneMapAndFXAAEffect(EffectFactory factory) : base(factory.Load<ToneMapAndFXAAEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ToneMapAndFXAATechnique"];

            this.ColorParameter = this.Effect.Parameters["Color"];
        }

        public Texture2D Color { set => this.ColorParameter.SetValue(value); }
    }
}

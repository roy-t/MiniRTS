using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.PostProcess
{
    public sealed class TonemapEffect : EffectWrapper
    {
        private readonly EffectParameter ColorParameter;

        public TonemapEffect(EffectFactory factory) : base(factory.Load<TonemapEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ToneMapTechnique"];

            this.ColorParameter = this.Effect.Parameters["Color"];
        }

        public Texture2D Color { set => this.ColorParameter.SetValue(value); }
    }
}

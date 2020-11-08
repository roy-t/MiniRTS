using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.PostProcess
{
    public sealed class BlurEffect : EffectWrapper
    {
        public BlurEffect(EffectFactory factory) : base(factory.Load<BlurEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["BlurTechnique"];
        }

        public Texture2D Diffuse { set => this.Effect.Parameters["Diffuse"].SetValue(value); }
        public float SampleRadius { set => this.Effect.Parameters["SampleRadius"].SetValue(value); }
    }
}

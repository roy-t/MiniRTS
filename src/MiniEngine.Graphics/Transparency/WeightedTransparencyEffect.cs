using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;


namespace MiniEngine.Graphics.Transparency
{
    public sealed class WeightedTransparencyEffect : EffectWrapper
    {
        private readonly EffectParameter TextureParameter;
        private readonly EffectParameter WorldViewProjectionParameter;

        public WeightedTransparencyEffect(EffectFactory factory) : base(factory.Load<WeightedTransparencyEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["WeightedTransparencyTechnique"];
            this.TextureParameter = this.Effect.Parameters["Texture"];
            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
        }

        public Texture2D Texture { set => this.TextureParameter.SetValue(value); }

        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }
    }
}


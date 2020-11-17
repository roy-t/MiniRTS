using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Utilities
{
    public sealed class OpaqueEffect : EffectWrapper
    {
        private readonly EffectParameter TextureParameter;

        public OpaqueEffect(EffectFactory factory) : base(factory.Load<OpaqueEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["OpaqueTechnique"];

            this.TextureParameter = this.Effect.Parameters["Texture"];
        }

        public Texture2D Texture { set => this.TextureParameter.SetValue(value); }
    }
}

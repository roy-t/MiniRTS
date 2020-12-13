using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Utilities
{
    public sealed class CubeMapGeneratorEffect : EffectWrapper, I3DEffect
    {
        private readonly EffectParameter EquirectangularTextureParameter;
        private readonly EffectParameter WorldViewProjectionParameter;

        public CubeMapGeneratorEffect(EffectFactory factory) : base(factory.Load<CubeMapGeneratorEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["CubeMapGeneratorTechnique"];

            this.EquirectangularTextureParameter = this.Effect.Parameters["EquirectangularTexture"];
            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
        }

        public Texture2D EquirectangularTexture { set => this.EquirectangularTextureParameter.SetValue(value); }

        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }
    }
}

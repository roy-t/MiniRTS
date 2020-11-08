using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Utilities
{
    public sealed class CubeMapGeneratorEffect : EffectWrapper, I3DEffect
    {
        public CubeMapGeneratorEffect(EffectFactory factory) : base(factory.Load<CubeMapGeneratorEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["CubeMapGeneratorTechnique"];
        }

        public Texture2D EquirectangularTexture { set => this.Effect.Parameters["EquirectangularTexture"].SetValue(value); }

        public Matrix WorldViewProjection { set => this.Effect.Parameters["WorldViewProjection"].SetValue(value); }
    }
}

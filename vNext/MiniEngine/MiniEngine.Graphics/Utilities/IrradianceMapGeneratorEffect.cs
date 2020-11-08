using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Utilities
{
    public sealed class IrradianceMapGeneratorEffect : EffectWrapper, I3DEffect
    {
        public IrradianceMapGeneratorEffect(EffectFactory factory) : base(factory.Load<IrradianceMapGeneratorEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["IrradianceMapGeneratorTechnique"];
        }

        public Texture2D EquirectangularTexture { set => this.Effect.Parameters["EquirectangularTexture"].SetValue(value); }

        public Matrix WorldViewProjection { set => this.Effect.Parameters["WorldViewProjection"].SetValue(value); }

        public float SampleDleta { set => this.Effect.Parameters["SampleDelta"].SetValue(value); }
    }
}

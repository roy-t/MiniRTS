using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Utilities
{
    public sealed class IrradianceMapGeneratorEffect : EffectWrapper, I3DEffect
    {
        private readonly EffectParameter EquirectangularTextureParameter;
        private readonly EffectParameter WorldViewProjectionParameter;
        private readonly EffectParameter SampleDeltaParameter;

        public IrradianceMapGeneratorEffect(EffectFactory factory) : base(factory.Load<IrradianceMapGeneratorEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["IrradianceMapGeneratorTechnique"];

            this.EquirectangularTextureParameter = this.Effect.Parameters["EquirectangularTexture"];
            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
            this.SampleDeltaParameter = this.Effect.Parameters["SampleDelta"];
        }

        public Texture2D EquirectangularTexture { set => this.EquirectangularTextureParameter.SetValue(value); }

        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }

        public float SampleDleta { set => this.SampleDeltaParameter.SetValue(value); }
    }
}

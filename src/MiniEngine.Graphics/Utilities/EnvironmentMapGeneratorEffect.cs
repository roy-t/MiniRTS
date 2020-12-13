using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Utilities
{
    public sealed class EnvironmentMapGeneratorEffect : EffectWrapper, I3DEffect
    {
        private readonly EffectParameter EquirectangularTextureParameter;
        private readonly EffectParameter WorldViewProjectionParameter;
        private readonly EffectParameter RoughnessParameter;

        public EnvironmentMapGeneratorEffect(EffectFactory factory) : base(factory.Load<EnvironmentMapGeneratorEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["EnvironmentMapGeneratorTechnique"];

            this.EquirectangularTextureParameter = this.Effect.Parameters["EquirectangularTexture"];
            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
            this.RoughnessParameter = this.Effect.Parameters["Roughness"];
        }

        public Texture2D EquirectangularTexture { set => this.EquirectangularTextureParameter.SetValue(value); }

        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }

        public float Roughness { set => this.RoughnessParameter.SetValue(value); }
    }
}

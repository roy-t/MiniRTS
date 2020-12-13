using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Skybox
{
    public sealed class SkyboxEffect : EffectWrapper
    {
        private readonly EffectParameter SkyboxParameter;
        private readonly EffectParameter WorldViewProjectionParameter;

        public SkyboxEffect(EffectFactory factory) : base(factory.Load<SkyboxEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["SkyboxTechnique"];

            this.SkyboxParameter = this.Effect.Parameters["Skybox"];
            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
        }

        public TextureCube Skybox { set => this.SkyboxParameter.SetValue(value); }

        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }
    }
}

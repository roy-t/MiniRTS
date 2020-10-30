using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Utilities
{
    public sealed class EnvironmentMapGeneratorEffect : EffectWrapper, I3DEffect
    {
        public EnvironmentMapGeneratorEffect(Effect effect)
            : base(effect)
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["EnvironmentMapGeneratorTechnique"];
        }

        public Texture2D EquirectangularTexture { set => this.Effect.Parameters["EquirectangularTexture"].SetValue(value); }

        public Matrix WorldViewProjection { set => this.Effect.Parameters["WorldViewProjection"].SetValue(value); }

        public float SampleDleta { set => this.Effect.Parameters["SampleDelta"].SetValue(value); }
    }
}
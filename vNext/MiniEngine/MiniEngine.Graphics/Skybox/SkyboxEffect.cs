using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Skybox
{
    public sealed class SkyboxEffect : EffectWrapper
    {
        public SkyboxEffect(Effect effect)
            : base(effect)
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["SkyboxTechnique"];
        }

        public Texture2D Skybox { set => this.Effect.Parameters["Skybox"].SetValue(value); }

        public Matrix WorldViewProjection { set => this.Effect.Parameters["WorldViewProjection"].SetValue(value); }
    }
}

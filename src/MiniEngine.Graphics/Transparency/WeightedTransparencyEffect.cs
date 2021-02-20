using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;
using MiniEngine.Graphics.Shadows;

namespace MiniEngine.Graphics.Transparency
{
    public sealed class WeightedTransparencyEffect : EffectWrapper
    {
        private readonly EffectParameter TextureParameter;
        private readonly EffectParameter WorldViewProjectionParameter;
        private readonly EffectParameter CameraPositionParameter;

        public WeightedTransparencyEffect(EffectFactory factory) : base(factory.Load<WeightedTransparencyEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["WeightedTransparencyTechnique"];
            this.TextureParameter = this.Effect.Parameters["Texture"];
            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
            this.CameraPositionParameter = this.Effect.Parameters["CameraPosition"];

            this.Shadows = new CascadedShadowMapParameters(this.Effect);
        }

        public Texture2D Texture { set => this.TextureParameter.SetValue(value); }

        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }

        public Vector3 CameraPosition { set => this.CameraPositionParameter.SetValue(value); }

        public CascadedShadowMapParameters Shadows { get; }
    }
}


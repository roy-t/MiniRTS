using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Shadows
{
    public sealed class LightPostProcessEffect : EffectWrapper
    {
        private readonly EffectParameter LightParameter;
        private readonly EffectParameter VolumeParameter;
        private readonly EffectParameter InverseViewProjectionParameter;
        private readonly EffectParameter DepthParameter;
        private readonly EffectParameter CameraPositionParameter;
        private readonly EffectParameter FogColorParameter;
        private readonly EffectParameter StrengthParameter;

        public LightPostProcessEffect(EffectFactory factory) : base(factory.Load<LightPostProcessEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["LightPostProcessTechnique"];

            this.LightParameter = this.Effect.Parameters["Light"];
            this.VolumeParameter = this.Effect.Parameters["Volume"];
            this.InverseViewProjectionParameter = this.Effect.Parameters["InverseViewProjection"];
            this.DepthParameter = this.Effect.Parameters["Depth"];
            this.CameraPositionParameter = this.Effect.Parameters["CameraPosition"];
            this.FogColorParameter = this.Effect.Parameters["FogColor"];
            this.StrengthParameter = this.Effect.Parameters["Strength"];
        }

        public Texture2D Light { set => this.LightParameter.SetValue(value); }

        public Texture2D Volume { set => this.VolumeParameter.SetValue(value); }

        public Matrix InverseViewProjection { set => this.InverseViewProjectionParameter.SetValue(value); }

        public Texture2D Depth { set => this.DepthParameter.SetValue(value); }

        public Vector3 CameraPosition { set => this.CameraPositionParameter.SetValue(value); }

        public Color FogColor { set => this.FogColorParameter.SetValue(value.ToVector3()); }

        public float Strength { set => this.StrengthParameter.SetValue(value); }
    }
}


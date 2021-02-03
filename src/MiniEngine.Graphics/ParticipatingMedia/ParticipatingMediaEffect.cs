using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Shadows
{
    public sealed class ParticipatingMediaEffect : EffectWrapper
    {
        private readonly EffectParameter NoiseParameter;
        private readonly EffectParameter VolumeBackParameter;
        private readonly EffectParameter VolumeFrontParameter;
        private readonly EffectParameter InverseViewProjectionParameter;
        private readonly EffectParameter DepthParameter;
        private readonly EffectParameter CameraPositionParameter;
        private readonly EffectParameter StrengthParameter;

        private readonly EffectParameter ShadowMapParameter;
        private readonly EffectParameter ShadowMatrixParameter;
        private readonly EffectParameter SplitsParameter;
        private readonly EffectParameter OffsetsParameter;
        private readonly EffectParameter ScalesParameter;

        private readonly EffectParameter ViewDistanceParameter;
        private readonly EffectParameter MinLightParameter;

        public ParticipatingMediaEffect(EffectFactory factory) : base(factory.Load<ParticipatingMediaEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ParticipatingMediaTechnique"];

            this.NoiseParameter = this.Effect.Parameters["Noise"];
            this.VolumeBackParameter = this.Effect.Parameters["VolumeBack"];
            this.VolumeFrontParameter = this.Effect.Parameters["VolumeFront"];
            this.InverseViewProjectionParameter = this.Effect.Parameters["InverseViewProjection"];
            this.DepthParameter = this.Effect.Parameters["Depth"];
            this.CameraPositionParameter = this.Effect.Parameters["CameraPosition"];
            this.StrengthParameter = this.Effect.Parameters["Strength"];
            this.ShadowMapParameter = this.Effect.Parameters["ShadowMap"];
            this.ShadowMatrixParameter = this.Effect.Parameters["ShadowMatrix"];
            this.SplitsParameter = this.Effect.Parameters["Splits"];
            this.OffsetsParameter = this.Effect.Parameters["Offsets"];
            this.ScalesParameter = this.Effect.Parameters["Scales"];
            this.ViewDistanceParameter = this.Effect.Parameters["ViewDistance"];
            this.MinLightParameter = this.Effect.Parameters["MinLight"];
        }

        public Texture2D Noise { set => this.NoiseParameter.SetValue(value); }

        public Texture2D VolumeBack { set => this.VolumeBackParameter.SetValue(value); }

        public Texture2D VolumeFront { set => this.VolumeFrontParameter.SetValue(value); }

        public Matrix InverseViewProjection { set => this.InverseViewProjectionParameter.SetValue(value); }

        public Texture2D Depth { set => this.DepthParameter.SetValue(value); }

        public Vector3 CameraPosition { set => this.CameraPositionParameter.SetValue(value); }


        public float Strength { set => this.StrengthParameter.SetValue(value); }

        public Texture2D ShadowMap { set => this.ShadowMapParameter.SetValue(value); }

        public Matrix ShadowMatrix { set => this.ShadowMatrixParameter.SetValue(value); }

        public float[] Splits { set => this.SplitsParameter.SetValue(value); }

        public Vector4[] Offsets { set => this.OffsetsParameter.SetValue(value); }

        public Vector4[] Scales { set => this.ScalesParameter.SetValue(value); }

        public float ViewDistance { set => this.ViewDistanceParameter.SetValue(value); }

        public float MinLight { set => this.MinLightParameter.SetValue(value); }
    }
}


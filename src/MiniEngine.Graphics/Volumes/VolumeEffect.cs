﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;


namespace MiniEngine.Graphics.Volumes
{
    public sealed class VolumeEffect : EffectWrapper
    {
        // TODO: same parameters as GeometryEffect

        private readonly EffectParameter CameraPositionParameter;
        private readonly EffectParameter WorldParameter;
        private readonly EffectParameter WorldViewProjectionParameter;
        private readonly EffectParameter AlbedoParameter;
        private readonly EffectParameter NormalParameter;
        private readonly EffectParameter MetalicnessParameter;
        private readonly EffectParameter RoughnessParameter;
        private readonly EffectParameter AmbientOcclusionParameter;

        public VolumeEffect(EffectFactory factory) : base(factory.Load<VolumeEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["VolumeTechnique"];

            this.CameraPositionParameter = this.Effect.Parameters["CameraPosition"];
            this.WorldParameter = this.Effect.Parameters["World"];
            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
            this.AlbedoParameter = this.Effect.Parameters["Albedo"];
            this.NormalParameter = this.Effect.Parameters["Normal"];
            this.MetalicnessParameter = this.Effect.Parameters["Metalicness"];
            this.RoughnessParameter = this.Effect.Parameters["Roughness"];
            this.AmbientOcclusionParameter = this.Effect.Parameters["AmbientOcclusion"];
        }

        public Vector3 CameraPosition { set => this.CameraPositionParameter.SetValue(value); }

        public Matrix World { set => this.WorldParameter.SetValue(value); }

        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }

        public Texture2D Albedo { set => this.AlbedoParameter.SetValue(value); }

        public Texture2D Normal { set => this.NormalParameter.SetValue(value); }

        public Texture2D Metalicness { set => this.MetalicnessParameter.SetValue(value); }

        public Texture2D Roughness { set => this.RoughnessParameter.SetValue(value); }

        public Texture2D AmbientOcclusion { set => this.AmbientOcclusionParameter.SetValue(value); }
    }
}

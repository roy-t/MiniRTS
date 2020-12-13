using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class GeometryEffect : EffectWrapper
    {
        private readonly EffectParameter CameraPositionParameter;
        private readonly EffectParameter WorldParameter;
        private readonly EffectParameter WorldViewProjectionParameter;
        private readonly EffectParameter DiffuseParameter;
        private readonly EffectParameter NormalParameter;
        private readonly EffectParameter MetalicnessParameter;
        private readonly EffectParameter RoughnessParameter;
        private readonly EffectParameter AmbientOcclusionParameter;

        public GeometryEffect(EffectFactory factory) : base(factory.Load<GeometryEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["GeometryTechnique"];

            this.CameraPositionParameter = this.Effect.Parameters["CameraPosition"];
            this.WorldParameter = this.Effect.Parameters["World"];
            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
            this.DiffuseParameter = this.Effect.Parameters["Diffuse"];
            this.NormalParameter = this.Effect.Parameters["Normal"];
            this.MetalicnessParameter = this.Effect.Parameters["Metalicness"];
            this.RoughnessParameter = this.Effect.Parameters["Roughness"];
            this.AmbientOcclusionParameter = this.Effect.Parameters["AmbientOcclusion"];
        }

        public Vector3 CameraPosition { set => this.CameraPositionParameter.SetValue(value); }

        public Matrix World { set => this.WorldParameter.SetValue(value); }

        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }

        public Texture2D Diffuse { set => this.DiffuseParameter.SetValue(value); }

        public Texture2D Normal { set => this.NormalParameter.SetValue(value); }

        public float Metalicness { set => this.MetalicnessParameter.SetValue(value); }

        public float Roughness { set => this.RoughnessParameter.SetValue(value); }

        public float AmbientOcclusion { set => this.AmbientOcclusionParameter.SetValue(value); }
    }
}

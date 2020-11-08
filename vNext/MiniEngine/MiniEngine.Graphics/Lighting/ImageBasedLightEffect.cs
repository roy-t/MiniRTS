using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Lighting
{
    public sealed class ImageBasedLightEffect : EffectWrapper
    {
        private readonly EffectParameter DiffuseParameter;
        private readonly EffectParameter NormalParameter;
        private readonly EffectParameter DepthParameter;
        private readonly EffectParameter MaterialParameter;
        private readonly EffectParameter InverseViewProjectionParameter;
        private readonly EffectParameter CameraPositionParameter;
        private readonly EffectParameter IrradianceParameter;
        private readonly EffectParameter EnvironmentParameter;
        private readonly EffectParameter BrdfLutParameter;
        private readonly EffectParameter MaxReflectionLodParameter;

        public ImageBasedLightEffect(EffectFactory factory) : base(factory.Load<ImageBasedLightEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ImageBasedLightTechnique"];

            this.DiffuseParameter = this.Effect.Parameters["Diffuse"];
            this.NormalParameter = this.Effect.Parameters["Normal"];
            this.DepthParameter = this.Effect.Parameters["Depth"];
            this.MaterialParameter = this.Effect.Parameters["Material"];
            this.InverseViewProjectionParameter = this.Effect.Parameters["InverseViewProjection"];
            this.CameraPositionParameter = this.Effect.Parameters["CameraPosition"];
            this.IrradianceParameter = this.Effect.Parameters["Irradiance"];
            this.EnvironmentParameter = this.Effect.Parameters["Environment"];
            this.BrdfLutParameter = this.Effect.Parameters["BrdfLut"];
            this.MaxReflectionLodParameter = this.Effect.Parameters["MaxReflectionLod"];
        }

        public Texture2D Diffuse { set => this.DiffuseParameter.SetValue(value); }

        public Texture2D Normal { set => this.NormalParameter.SetValue(value); }

        public Texture2D Depth { set => this.DepthParameter.SetValue(value); }

        public Texture2D Material { set => this.MaterialParameter.SetValue(value); }

        public TextureCube Irradiance { set => this.IrradianceParameter.SetValue(value); }

        public TextureCube Environment { set => this.EnvironmentParameter.SetValue(value); }

        public Texture2D BrdfLut { set => this.BrdfLutParameter.SetValue(value); }

        public Matrix InverseViewProjection { set => this.InverseViewProjectionParameter.SetValue(value); }

        public Vector3 CameraPosition { set => this.CameraPositionParameter.SetValue(value); }

        public int MaxReflectionLod { set => this.MaxReflectionLodParameter.SetValue(value); }
    }
}

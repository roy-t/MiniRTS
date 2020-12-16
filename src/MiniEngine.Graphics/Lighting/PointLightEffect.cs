using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Lighting
{
    public sealed class PointLightEffect : EffectWrapper
    {
        private readonly EffectParameter WorldViewProjectionParameter;
        private readonly EffectParameter AlbedoParameter;
        private readonly EffectParameter NormalParameter;
        private readonly EffectParameter DepthParameter;
        private readonly EffectParameter MaterialParameter;
        private readonly EffectParameter InverseViewProjectionParameter;
        private readonly EffectParameter CameraPositionParameter;
        private readonly EffectParameter PositionParameter;
        private readonly EffectParameter ColorParameter;
        private readonly EffectParameter StrengthParameter;

        public PointLightEffect(EffectFactory factory) : base(factory.Load<PointLightEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["PointLightTechnique"];

            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
            this.AlbedoParameter = this.Effect.Parameters["Albedo"];
            this.NormalParameter = this.Effect.Parameters["Normal"];
            this.DepthParameter = this.Effect.Parameters["Depth"];
            this.MaterialParameter = this.Effect.Parameters["Material"];
            this.InverseViewProjectionParameter = this.Effect.Parameters["InverseViewProjection"];
            this.CameraPositionParameter = this.Effect.Parameters["CameraPosition"];
            this.PositionParameter = this.Effect.Parameters["Position"];
            this.ColorParameter = this.Effect.Parameters["Color"];
            this.StrengthParameter = this.Effect.Parameters["Strength"];
        }

        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }

        public Texture2D Albedo { set => this.AlbedoParameter.SetValue(value); }

        public Texture2D Normal { set => this.NormalParameter.SetValue(value); }

        public Texture2D Depth { set => this.DepthParameter.SetValue(value); }

        public Texture2D Material { set => this.MaterialParameter.SetValue(value); }

        public Matrix InverseViewProjection { set => this.InverseViewProjectionParameter.SetValue(value); }

        public Vector3 CameraPosition { set => this.CameraPositionParameter.SetValue(value); }

        public Vector3 Position { set => this.PositionParameter.SetValue(value); }

        public Color Color { set => this.ColorParameter.SetValue(value.ToVector4()); }

        public float Strength { set => this.StrengthParameter.SetValue(value); }
    }
}

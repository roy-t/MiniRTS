using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Particles
{
    public sealed class SimulationEffect : EffectWrapper
    {
        private readonly EffectParameter DataParameter;

        private readonly EffectParameter LengthScaleParameter;
        private readonly EffectParameter FieldSpeedParameter;
        private readonly EffectParameter NoiseStrengthParameter;
        private readonly EffectParameter ElapsedParameter;
        private readonly EffectParameter TimeParameter;
        private readonly EffectParameter ProgressionRateParameter;
        private readonly EffectParameter FieldMainDirectionParameter;

        private readonly EffectParameter SpherePositionParameter;
        private readonly EffectParameter SphereRadiusParameter;


        public SimulationEffect(EffectFactory factory) : base(factory.Load<SimulationEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["SimulationTechnique"];
            this.ElapsedParameter = this.Effect.Parameters["Elapsed"];
            this.DataParameter = this.Effect.Parameters["Data"];


            this.LengthScaleParameter = this.Effect.Parameters["LengthScale"];
            this.FieldSpeedParameter = this.Effect.Parameters["FieldSpeed"];
            this.NoiseStrengthParameter = this.Effect.Parameters["NoiseStrength"];
            this.ElapsedParameter = this.Effect.Parameters["Elapsed"];
            this.TimeParameter = this.Effect.Parameters["Time"];
            this.ProgressionRateParameter = this.Effect.Parameters["ProgressionRate"];
            this.FieldMainDirectionParameter = this.Effect.Parameters["FieldMainDirection"];

            this.SpherePositionParameter = this.Effect.Parameters["SpherePosition"];
            this.SphereRadiusParameter = this.Effect.Parameters["SphereRadius"];
        }

        public Texture2D Data { set => this.DataParameter.SetValue(value); }

        public float LengthScale { set => this.LengthScaleParameter.SetValue(value); }
        public float FieldSpeed { set => this.FieldSpeedParameter.SetValue(value); }
        public float NoiseStrength { set => this.NoiseStrengthParameter.SetValue(value); }
        public float Elapsed { set => this.ElapsedParameter.SetValue(value); }
        public float Time { set => this.TimeParameter.SetValue(value); }
        public float ProgressionRate { set => this.ProgressionRateParameter.SetValue(value); }
        public Vector3 FieldMainDirection { set => this.FieldMainDirectionParameter.SetValue(value); }

        public Vector3 SpherePosition { set => this.SpherePositionParameter.SetValue(value); }
        public float SphereRadius { set => this.SphereRadiusParameter.SetValue(value); }
    }
}

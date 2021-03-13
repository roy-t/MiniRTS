using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Particles
{
    public sealed class ParticleSimulationEffect : EffectWrapper
    {
        private readonly EffectParameter VelocityParameter;
        private readonly EffectParameter AccelerationParameter;
        private readonly EffectParameter PositionParameter;

        private readonly EffectParameter LengthScaleParameter;
        private readonly EffectParameter FieldSpeedParameter;
        private readonly EffectParameter NoiseStrengthParameter;
        private readonly EffectParameter ElapsedParameter;
        private readonly EffectParameter TimeParameter;
        private readonly EffectParameter ProgressionRateParameter;
        private readonly EffectParameter FieldMainDirectionParameter;
        private readonly EffectParameter SpherePositionParameter;
        private readonly EffectParameter SphereRadiusParameter;


        public ParticleSimulationEffect(EffectFactory factory) : base(factory.Load<ParticleSimulationEffect>())
        {
            this.VelocityParameter = this.Effect.Parameters["Velocity"];
            this.AccelerationParameter = this.Effect.Parameters["Acceleration"];
            this.PositionParameter = this.Effect.Parameters["Position"];

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

        public Texture2D Velocity { set => this.VelocityParameter.SetValue(value); }
        public Texture2D Acceleration { set => this.AccelerationParameter.SetValue(value); }
        public Texture2D Position { set => this.PositionParameter.SetValue(value); }

        public float LengthScale { set => this.LengthScaleParameter.SetValue(value); }
        public float FieldSpeed { set => this.FieldSpeedParameter.SetValue(value); }
        public float NoiseStrength { set => this.NoiseStrengthParameter.SetValue(value); }
        public float Elapsed { set => this.ElapsedParameter.SetValue(value); }
        public float Time { set => this.TimeParameter.SetValue(value); }
        public float ProgressionRate { set => this.ProgressionRateParameter.SetValue(value); }
        public Vector3 FieldMainDirection { set => this.FieldMainDirectionParameter.SetValue(value); }

        public Vector3 SpherePosition { set => this.SpherePositionParameter.SetValue(value); }
        public float SphereRadius { set => this.SphereRadiusParameter.SetValue(value); }

        public void ApplyVelocity()
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ParticleVelocitySimulationTechnique"];
            this.Apply();
        }
    }
}

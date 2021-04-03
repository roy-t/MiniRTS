using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Particles
{
    public sealed class ParticleSimulationEffect : EffectWrapper
    {
        private readonly EffectParameter VelocityParameter;
        private readonly EffectParameter PositionParameter;
        private readonly EffectParameter ForcesParameter;

        private readonly EffectParameter ElapsedParameter;
        private readonly EffectParameter TimeParameter;
        private readonly EffectParameter MaxLifeTimeParameter;

        private readonly EffectParameter LengthScaleParameter;
        private readonly EffectParameter NoiseStrengthParameter;

        private readonly EffectParameter ProgressionRateParameter;
        private readonly EffectParameter FieldSpeedParameter;

        private readonly EffectParameter EmitterSizeParameter;

        private readonly EffectParameter SpherePositionParameter;
        private readonly EffectParameter SphereRadiusParameter;

        private readonly EffectParameter ForceParameter;
        private readonly EffectParameter ForceWorldParameter;

        public ParticleSimulationEffect(EffectFactory factory) : base(factory.Load<ParticleSimulationEffect>())
        {
            this.VelocityParameter = this.Effect.Parameters["Velocity"];
            this.PositionParameter = this.Effect.Parameters["Position"];
            this.ForcesParameter = this.Effect.Parameters["Forces"];

            this.ElapsedParameter = this.Effect.Parameters["Elapsed"];
            this.TimeParameter = this.Effect.Parameters["Time"];
            this.MaxLifeTimeParameter = this.Effect.Parameters["MaxLifeTime"];

            this.LengthScaleParameter = this.Effect.Parameters["LengthScale"];
            this.NoiseStrengthParameter = this.Effect.Parameters["NoiseStrength"];

            this.ProgressionRateParameter = this.Effect.Parameters["ProgressionRate"];
            this.FieldSpeedParameter = this.Effect.Parameters["FieldSpeed"];

            this.EmitterSizeParameter = this.Effect.Parameters["EmitterSize"];

            this.SpherePositionParameter = this.Effect.Parameters["SpherePosition"];
            this.SphereRadiusParameter = this.Effect.Parameters["SphereRadius"];

            this.ForceParameter = this.Effect.Parameters["Force"];
            this.ForceWorldParameter = this.Effect.Parameters["ForceWorld"];
        }

        public Texture2D Velocity { set => this.VelocityParameter.SetValue(value); }
        public Texture2D Position { set => this.PositionParameter.SetValue(value); }
        public Texture2D Forces { set => this.ForcesParameter.SetValue(value); }

        public float Elapsed { set => this.ElapsedParameter.SetValue(value); }
        public float Time { set => this.TimeParameter.SetValue(value); }
        public float MaxLifeTime { set => this.MaxLifeTimeParameter.SetValue(value); }

        public float LengthScale { set => this.LengthScaleParameter.SetValue(value); }
        public float NoiseStrength { set => this.NoiseStrengthParameter.SetValue(value); }

        public float ProgressionRate { set => this.ProgressionRateParameter.SetValue(value); }
        public float FieldSpeed { set => this.FieldSpeedParameter.SetValue(value); }

        public float EmitterSize { set => this.EmitterSizeParameter.SetValue(value); }

        public Vector3 SpherePosition { set => this.SpherePositionParameter.SetValue(value); }
        public float SphereRadius { set => this.SphereRadiusParameter.SetValue(value); }

        public Vector3 Force { set => this.ForceParameter.SetValue(value); }
        public Matrix ForceWorld { set => this.ForceWorldParameter.SetValue(value); }

        public void ApplyVelocity()
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ParticleVelocitySimulationTechnique"];
            this.Apply();
        }

        public void ApplyPosition()
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ParticlePositionSimulationTechnique"];
            this.Apply();
        }
    }
}

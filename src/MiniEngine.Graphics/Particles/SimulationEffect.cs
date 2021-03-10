using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Particles
{
    public sealed class SimulationEffect : EffectWrapper
    {
        private readonly EffectParameter ElapsedParameter;
        private readonly EffectParameter DataParameter;

        public SimulationEffect(EffectFactory factory) : base(factory.Load<SimulationEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["SimulationTechnique"];
            this.ElapsedParameter = this.Effect.Parameters["Elapsed"];
            this.DataParameter = this.Effect.Parameters["Data"];
        }

        public float Elapsed { set => this.ElapsedParameter.SetValue(value); }
        public Texture2D Data { set => this.DataParameter.SetValue(value); }
    }
}

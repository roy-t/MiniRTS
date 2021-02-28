using Microsoft.Xna.Framework;
using MiniEngine.Graphics.Camera;

namespace MiniEngine.Graphics.Particles.Functions
{
    public sealed class LinearUpdateFunction : IParticleUpdateFunction
    {
        // TODO: update fields

        public float VelocityDelta { get; set; }

        public float ScrewDelta { get; set; }

        public float AngleDelta { get; set; }

        public float AmplitudeDelta { get; set; }

        public float ScaleDelta { get; set; }

        public float AlphaDelta { get; set; }

        public void Update(float elapsed, ref Particle particle, ICamera camera)
        {
            particle.Scale += elapsed * this.ScaleDelta;
            particle.Position += Vector3.Up * elapsed;
        }
    }
}

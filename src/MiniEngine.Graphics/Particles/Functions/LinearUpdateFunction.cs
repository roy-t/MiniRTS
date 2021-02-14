using MiniEngine.Graphics.Camera;

namespace MiniEngine.Graphics.Particles.Functions
{
    public sealed class LinearUpdateFunction : IParticleUpdateFunction
    {
        public float VelocityDelta { get; set; }

        public float ScrewDelta { get; set; }

        public float AngleDelta { get; set; }

        public float AmplitudeDelta { get; set; }

        public float ScaleDelta { get; set; }

        public float AlphaDelta { get; set; }

        public void Update(float elapsed, ref Particle particle, ICamera camera)
        {
            var elapsedOfMaxAge = elapsed / particle.MaxAge;

            particle.Velocity += elapsedOfMaxAge * this.VelocityDelta;
            particle.Screw += elapsedOfMaxAge * this.ScrewDelta;
            particle.Angle += elapsedOfMaxAge * this.AngleDelta;
            particle.Amplitude += elapsedOfMaxAge * this.AmplitudeDelta;
            particle.Scale += elapsedOfMaxAge * this.ScaleDelta;
            particle.Alpha += elapsedOfMaxAge * this.AlphaDelta;

            particle.Position += particle.Velocity * elapsed * particle.Forward;
            particle.Update(camera);
        }
    }
}

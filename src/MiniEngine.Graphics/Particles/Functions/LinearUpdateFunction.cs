using Microsoft.Xna.Framework;
using MiniEngine.Graphics.Camera;

namespace MiniEngine.Graphics.Particles.Functions
{
    public sealed class LinearUpdateFunction : IParticleUpdateFunction
    {
        public void Update(ref Particle particle, ICamera camera)
        {
            var position = particle.StartPosition + (particle.Velocity * particle.Age);
            particle.Transform = Matrix.CreateScale(particle.Scale)
                * ParticleMath.CreateBillboard(position, camera.View);
        }
    }
}

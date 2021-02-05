using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Particles.Functions
{
    public sealed class LinearUpdateFunction : IParticleUpdateFunction
    {
        public void Update(float elapsed, ref Particle particle)
            => particle.Transform *= Matrix.CreateTranslation(particle.Velocity * elapsed);
    }
}

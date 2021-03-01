using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Particles
{
    public interface IParticleUpdateFunction
    {
        void Update(float elapsed, Matrix transform, ref Particle particle);
    }
}

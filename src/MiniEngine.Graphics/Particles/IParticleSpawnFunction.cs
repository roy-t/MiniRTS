using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Particles
{
    public interface IParticleSpawnFunction
    {
        Particle[] Spawn(float elapsed, Matrix transform);
    }
}

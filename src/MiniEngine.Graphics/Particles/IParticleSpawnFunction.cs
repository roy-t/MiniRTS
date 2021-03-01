using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Particles
{
    public interface IParticleSpawnFunction
    {
        void Spawn(float elapsed, Matrix transform, ParticleBuffer buffer);
    }
}

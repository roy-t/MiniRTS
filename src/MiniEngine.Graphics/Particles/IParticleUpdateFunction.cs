using MiniEngine.Graphics.Camera;

namespace MiniEngine.Graphics.Particles
{
    public interface IParticleUpdateFunction
    {
        void Update(ref Particle particle, ICamera camera);
    }
}

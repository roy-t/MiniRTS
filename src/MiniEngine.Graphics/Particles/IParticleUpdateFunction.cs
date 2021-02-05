namespace MiniEngine.Graphics.Particles
{
    public interface IParticleUpdateFunction
    {
        void Update(float elapsed, ref Particle particle);
    }
}

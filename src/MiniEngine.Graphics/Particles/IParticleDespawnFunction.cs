namespace MiniEngine.Graphics.Particles
{
    public interface IParticleDespawnFunction
    {
        void Update(int id, float elapsed, ref Particle particle);
    }
}

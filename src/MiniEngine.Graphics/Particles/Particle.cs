using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Particles
{
    public struct Particle
    {
        public Matrix Transform;
        public Vector3 Velocity;
        public float TimeToLive;
    }
}

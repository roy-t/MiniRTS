using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Particles
{
    public struct Particle
    {
        public Matrix Transform;
        public Vector3 Velocity;
        public Vector3 StartPosition;
        public float Age;
        public float MaxAge;
        public Vector3 Scale;
    }
}

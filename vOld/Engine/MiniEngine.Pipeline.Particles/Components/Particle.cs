using Microsoft.Xna.Framework;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Particles.Components
{
    public sealed class Particle
    {
        public Particle(Vector3 position, Vector3 scale, Vector3 linearVelocity, Seconds timePerFrame)
        {
            this.Position = position;
            this.Scale = scale;
            this.LinearVelocity = linearVelocity;
            this.TimePerFrame = timePerFrame;

            this.LifeTime = 0;
            this.Frame = 0;
            this.Tint = Color.Purple;
        }

        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 LinearVelocity { get; set; }
        public Seconds LifeTime { get; set; }
        public Seconds TimePerFrame { get; set; }
        public int Frame { get; set; }
        public Color Tint { get; set; }
    }
}
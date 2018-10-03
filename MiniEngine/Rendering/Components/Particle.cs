using Microsoft.Xna.Framework;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Components
{
    public struct Particle
    {
        public Particle(Vector3 position, Vector3 linearVelocity, Vector3 angularVelocity, Seconds timeToLive, Seconds timePerFrame)
        {
            this.Position = position;
            this.LinearVelocity = linearVelocity;
            this.AngularVelocity = angularVelocity;
            this.TimeToLive = timeToLive;
            this.TimePerFrame = timePerFrame;
            this.Frame = 0;
        }

        public Vector3 Position { get; set; }
        public Vector3 LinearVelocity { get; set; }
        public Vector3 AngularVelocity { get; set; }
        public Seconds TimeToLive { get; set; }
        public Seconds TimePerFrame { get; set; }
        public int Frame { get; set; }        
    }
}

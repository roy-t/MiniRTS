using Microsoft.Xna.Framework;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Components
{
    public struct Particle
    {
        public Vector3 Location { get; set; }
        public Vector3 LinearVelocity { get; set; }
        public Vector3 AngularVelocity { get; set; }
        public Seconds Lifetime { get; set; }
        public Seconds TimePerFrame { get; set; }
        public int Frame { get; set; }
    }
}

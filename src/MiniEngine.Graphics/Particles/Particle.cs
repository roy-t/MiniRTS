using Microsoft.Xna.Framework;
using MiniEngine.Graphics.Camera;

namespace MiniEngine.Graphics.Particles
{
    public struct Particle
    {
        public Matrix Transform;
        public Vector3 Forward;
        public Vector3 Up;
        public Vector3 Position;
        public float Velocity;
        public float Angle;
        public float Amplitude;
        public float Scale;
        public Vector4 Tint;
        public float Age;
        public float MaxAge;

        public void Update(ICamera camera)
        {
            var offset = Vector3.Transform(this.Up, Matrix.CreateFromAxisAngle(this.Forward, this.Angle)) * this.Amplitude;

            this.Transform =
                Matrix.CreateScale(this.Scale)
                * ParticleMath.CreateBillboard(this.Position + offset, camera.View);
        }
    }
}

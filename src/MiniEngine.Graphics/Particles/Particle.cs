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
        public float AngularVelocity;
        public float Angle;
        public float Screw;
        public float Amplitude;
        public float Scale;
        public Color Tint;
        public float Alpha;
        public float Age;
        public float MaxAge;

        public void Update(ICamera camera)
        {
            var offset = Vector3.Transform(this.Up, Matrix.CreateFromAxisAngle(this.Forward, this.Screw)) * this.Amplitude;

            this.Transform =
                Matrix.CreateScale(this.Scale)
                * Matrix.CreateFromAxisAngle(this.Up, this.Angle)
                * ParticleMath.CreateBillboard(this.Position + offset, camera.View);
        }
    }
}

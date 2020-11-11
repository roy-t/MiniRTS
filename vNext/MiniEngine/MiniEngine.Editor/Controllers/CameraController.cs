using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;

namespace MiniEngine.Editor.Controllers
{
    [Service]
    public sealed class CameraController
    {
        private const float MinVelocity = 1.0f;
        private const float MaxVelocity = 21.0f;

        private readonly KeyboardController KeyboardInput;
        private readonly MouseController MouseInput;

        public CameraController(KeyboardController keyboardInput, MouseController mouseInput)
        {
            this.KeyboardInput = keyboardInput;
            this.MouseInput = mouseInput;

            this.Velocity = 1.0f;
            this.RadiansPerPixel = MathHelper.Pi * 0.002f;
        }

        public float Velocity { get; set; }
        public float RadiansPerPixel { get; set; }

        internal void Update(ICamera camera, float elapsed)
        {
            var translation = Matrix.Identity;
            var position = camera.Position;
            var forward = camera.Forward;
            var left = Vector3.Cross(Vector3.Up, forward);

            if (this.KeyboardInput.Held(Keys.W))
            {
                translation *= Matrix.CreateTranslation(forward * elapsed * this.Velocity);
            }

            if (this.KeyboardInput.Held(Keys.S))
            {
                translation *= Matrix.CreateTranslation(-forward * elapsed * this.Velocity);
            }

            if (this.KeyboardInput.Held(Keys.A))
            {
                translation *= Matrix.CreateTranslation(left * elapsed * this.Velocity);
            }

            if (this.KeyboardInput.Held(Keys.D))
            {
                translation *= Matrix.CreateTranslation(-left * elapsed * this.Velocity);
            }

            if (this.KeyboardInput.Held(Keys.Space))
            {
                translation *= Matrix.CreateTranslation(Vector3.Up * elapsed * this.Velocity);
            }

            if (this.KeyboardInput.Held(Keys.C))
            {
                translation *= Matrix.CreateTranslation(Vector3.Down * elapsed * this.Velocity);
            }

            if (this.KeyboardInput.Held(Keys.R))
            {
                position = Vector3.Backward * 10;
                forward = Vector3.Forward;
            }

            if (this.MouseInput.Held(MouseButtons.Middle))
            {
                var mouseMovement = new Vector2(this.MouseInput.Movement.X, this.MouseInput.Movement.Y) * this.RadiansPerPixel;
                var rotation = Matrix.CreateFromAxisAngle(Vector3.Up, mouseMovement.X) * Matrix.CreateFromAxisAngle(left, -mouseMovement.Y);
                forward = Vector3.Normalize(Vector3.Transform(forward, rotation));
            }

            if (this.MouseInput.ScrolledUp)
            {
                this.Velocity = Math.Min(this.Velocity + 1, MaxVelocity);
            }

            if (this.MouseInput.ScrolledDown)
            {
                this.Velocity = Math.Max(this.Velocity - 1, MinVelocity);
            }

            position = Vector3.Transform(position, translation);

            camera.Move(position, forward);
        }
    }
}

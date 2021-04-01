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
        private const float MinVelocity = 0.5f;
        private const float MaxVelocity = 25.0f;

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

        internal void Update(PerspectiveCamera camera, float elapsed)
        {
            var step = elapsed * this.Velocity;

            var forward = camera.Forward;
            var backward = -forward;
            var up = camera.Up;
            var down = -up;
            var left = camera.Left;
            var right = -left;

            var movementVector = this.KeyboardInput.AsArray(InputState.Pressed, Keys.W, Keys.S, Keys.A, Keys.D, Keys.Space, Keys.C);
            var translation = Vector3.Zero;
            translation += movementVector[0] * forward;
            translation += movementVector[1] * backward;
            translation += movementVector[2] * left;
            translation += movementVector[3] * right;
            translation += movementVector[4] * up;
            translation += movementVector[5] * down;

            translation *= step;

            var rotation = Quaternion.Identity;
            if (this.MouseInput.Held(MouseButtons.Middle))
            {
                var mouseMovement = new Vector2(this.MouseInput.Movement.X, this.MouseInput.Movement.Y) * this.RadiansPerPixel;
                rotation *= Quaternion.CreateFromAxisAngle(up, mouseMovement.X);
                rotation *= Quaternion.CreateFromAxisAngle(right, mouseMovement.Y);
            }

            if (this.MouseInput.ScrolledUp)
            {
                this.Velocity = Math.Min(this.Velocity + 1, MaxVelocity);
            }

            if (this.MouseInput.ScrolledDown)
            {
                this.Velocity = Math.Max(this.Velocity - 1, MinVelocity);
            }

            if (translation.LengthSquared() != 0 || rotation != Quaternion.Identity)
            {
                camera.Transform.MoveTo(camera.Position + translation);
                var lookAt = camera.Position + Vector3.Transform(camera.Transform.Forward, rotation);
                camera.Transform.FaceTargetConstrained(lookAt, Vector3.Up);
                camera.Update();
            }

            if (this.KeyboardInput.Held(Keys.R))
            {
                camera.Transform.MoveTo(Vector3.Backward * 10);
                camera.Transform.FaceTargetConstrained(Vector3.Zero, Vector3.Up);
                camera.Update();
            }
        }
    }
}

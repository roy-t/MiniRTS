using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Input;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Controllers
{
    public sealed class CameraController
    {
        private static readonly MetersPerSecond TranslateSpeed = 50.0f;

        // percentage to rotate the camera per pixel the mouse moved
        private const float RotateFactor = MathHelper.TwoPi * 0.001f;

        private readonly KeyboardInput Keyboard;
        private readonly MouseInput Mouse;
        private readonly PerspectiveCamera PerspectiveCamera;

        private Vector3 forward;
        private Vector3 left;
        private Vector3 up;

        public CameraController(KeyboardInput keyboard, MouseInput mouse, PerspectiveCamera perspectiveCamera)
        {
            this.Keyboard = keyboard;
            this.PerspectiveCamera = perspectiveCamera;
            this.Mouse = mouse;

            this.forward = Vector3.Forward;
            this.left = Vector3.Left;
            this.up = Vector3.Up;
        }

        public void Update(Seconds elapsed)
        {
            var position = this.PerspectiveCamera.Position;            

            var translate = TranslateSpeed * elapsed;

            if (this.Mouse.Hold(MouseButtons.Right))
            {
                var rotation = new Vector2(this.Mouse.Movement.X, this.Mouse.Movement.Y) * RotateFactor;
                var matrix = Matrix.CreateFromAxisAngle(this.up, rotation.X) * Matrix.CreateFromAxisAngle(this.left, -rotation.Y);

                this.forward = Vector3.Transform(this.forward, matrix);
                this.left = Vector3.Cross(this.up, this.forward);
            }

            if (this.Keyboard.Hold(Keys.A))
            {
                position += this.left * translate;
            }
            else if (this.Keyboard.Hold(Keys.D))
            {
                position -= this.left * translate;
            }

            if (this.Keyboard.Hold(Keys.W))
            {
                position += this.forward * translate;
            }
            else if (this.Keyboard.Hold(Keys.S))
            {
                position -= this.forward * translate;
            }

            if (this.Keyboard.Hold(Keys.C))
            {
                position -= this.up * translate;
            }
            else if (this.Keyboard.Hold(Keys.Space))
            {
                position += this.up * translate;
            }

            if (this.Keyboard.Click(Keys.R))
            {
                position = Vector3.Backward * 10;

                this.forward = Vector3.Forward;
                this.left = Vector3.Left;
                this.up = Vector3.Up;
            }           

            this.PerspectiveCamera.Move(position, position + this.forward);
        }
    }
}

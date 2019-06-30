using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Input;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Units;
using KeyboardInput = MiniEngine.Input.KeyboardInput;

namespace MiniEngine.Controllers
{
    public sealed class CameraController
    {
        private static readonly MetersPerSecond TranslateSpeed = 10.0f;
        
        // percentage to rotate the camera per pixel the mouse moved
        private static readonly Radians RotateFactor = Radians.Pi* 0.002f;

        private readonly KeyboardInput Keyboard;
        private readonly MouseInput Mouse;        

        private Vector3 forward;
        private Vector3 left;
        private Vector3 up;

        public CameraController(KeyboardInput keyboard, MouseInput mouse)
        {
            this.Keyboard = keyboard;
            this.Mouse = mouse;

            this.forward = Vector3.Forward;
            this.left = Vector3.Left;
            this.up = Vector3.Up;
        }

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            var position = camera.Position;
            this.forward = Vector3.Normalize(camera.LookAt - camera.Position);

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
            
            camera.Move(position, position + this.forward);
        }
    }
}

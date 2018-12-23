using System;
using System.Globalization;
using System.IO;
using System.Text;
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

            if (File.Exists("last_camera_position.ini"))
            {
                var text = File.ReadAllText("last_camera_position.ini");
                var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                var x = float.Parse(lines[0]);
                var y = float.Parse(lines[1]);
                var z = float.Parse(lines[2]);

                var lx = float.Parse(lines[3]);
                var ly = float.Parse(lines[4]);
                var lz = float.Parse(lines[5]);

                this.PerspectiveCamera.Move(new Vector3(x, y, z), new Vector3(lx, ly, lz));
            }
        }

        public void Update(Seconds elapsed)
        {
            var position = this.PerspectiveCamera.Position;
            this.forward = Vector3.Normalize(this.PerspectiveCamera.LookAt - this.PerspectiveCamera.Position);

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


            if (this.Keyboard.Click(Keys.B))
            {
                var text = new StringBuilder();
                text.AppendLine(this.PerspectiveCamera.Position.X.ToString(CultureInfo.InvariantCulture));
                text.AppendLine(this.PerspectiveCamera.Position.Y.ToString(CultureInfo.InvariantCulture));
                text.AppendLine(this.PerspectiveCamera.Position.Z.ToString(CultureInfo.InvariantCulture));

                text.AppendLine(this.PerspectiveCamera.LookAt.X.ToString(CultureInfo.InvariantCulture));
                text.AppendLine(this.PerspectiveCamera.LookAt.Y.ToString(CultureInfo.InvariantCulture));
                text.AppendLine(this.PerspectiveCamera.LookAt.Z.ToString(CultureInfo.InvariantCulture));

                File.WriteAllText("last_camera_position.ini", text.ToString());
            }



            this.PerspectiveCamera.Move(position, position + this.forward);
        }
    }
}

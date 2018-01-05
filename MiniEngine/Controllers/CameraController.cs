using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Input;
using MiniEngine.Rendering;
using MiniEngine.Units;

namespace MiniEngine.Controllers
{
    public sealed class CameraController
    {
        private static readonly MetersPerSecond TranslateSpeed = 50.0f;

        private readonly Input.KeyboardInput Keyboard;
        private readonly Camera Camera;

        public CameraController(Input.KeyboardInput keyboard, Camera camera)
        {
            this.Keyboard = keyboard;
            this.Camera = camera;            
        }

        public void Update(Seconds elapsed)
        {
            var position = this.Camera.Position;
            var lookAt = this.Camera.LookAt;

            var translate = TranslateSpeed * elapsed;

            if (this.Keyboard.Hold(Keys.A))
            {
                position.X -= translate;
                lookAt.X -= translate;
            }
            else if (this.Keyboard.Hold(Keys.D))
            {
                position.X += translate;
                lookAt.X += translate;
            }

            if (this.Keyboard.Hold(Keys.W))
            {
                position.Z -= translate;
                lookAt.Z -= translate;
            }
            else if (this.Keyboard.Hold(Keys.S))
            {
                position.Z += translate;
                lookAt.Z += translate;
            }

            if (this.Keyboard.Hold(Keys.C))
            {
                position.Y -= translate;
                lookAt.Y -= translate;
            }
            else if (this.Keyboard.Hold(Keys.Space))
            {
                position.Y += translate;
                lookAt.Y += translate;
            }

            if (this.Keyboard.Click(Keys.R))
            {
                position = Vector3.Backward * 10;
                lookAt = Vector3.Zero;
            }

            this.Camera.Move(position, lookAt);
        }
    }
}

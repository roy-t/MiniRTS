using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Editor.Configuration;
using MiniEngine.Editor.Controllers;
using MiniEngine.Graphics.Utilities;

namespace MiniEngine.Editor
{
    [Service]
    internal sealed class SingleFrameLoop : IGameLoop
    {
        private readonly RenderDoc RenderDoc;
        private readonly ContentManager Content;
        private readonly CubeMapGenerator CubeMapGenerator;
        private readonly GraphicsDeviceManager Graphics;
        private readonly GraphicsDevice Device;
        private readonly GameWindow Window;
        private readonly GameTimer GameTimer;
        private readonly KeyboardController Keyboard;
        private readonly MouseController Mouse;
        private readonly CameraController CameraController;

        private int drawCalls;

        public SingleFrameLoop(RenderDoc renderDoc, ContentManager content, CubeMapGenerator cubeMapGenerator, GraphicsDeviceManager graphics, GraphicsDevice device, GameWindow window, GameTimer gameTimer, KeyboardController keyboard, MouseController mouse, CameraController cameraController)
        {
            this.RenderDoc = renderDoc;
            this.Content = content;
            this.CubeMapGenerator = cubeMapGenerator;
            Graphics = graphics;
            Device = device;
            Window = window;
            GameTimer = gameTimer;
            Keyboard = keyboard;
            Mouse = mouse;
            CameraController = cameraController;
        }

        private void DrawExperiment()
        {
            var name = "Skyboxes/Grid/testgrid";
            var equiRect = this.Content.Load<Texture2D>(name);
            var albedo = this.CubeMapGenerator.Generate(equiRect);
        }


        public bool Update(GameTime gameTime)
        {
            return this.drawCalls < 2;
        }

        public void Draw(GameTime gameTime)
        {
            if (this.drawCalls == 0 && this.RenderDoc != null)
            {
                this.RenderDoc.TriggerCapture();
            }

            if (this.drawCalls == 1)
            {
                this.DrawExperiment();
            }

            this.drawCalls++;
        }

        private void RenderToViewport(RenderTarget2D renderTarget)
        {

        }

        public void Stop()
        {
            var path = this.RenderDoc.GetCapture(this.RenderDoc.GetNumCaptures() - 1) ?? string.Empty;
            this.RenderDoc.LaunchReplayUI(path);
        }
    }
}

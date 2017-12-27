using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Lighting;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering
{
    public sealed class RenderSystem
    {
        private readonly GraphicsDevice Device;
        private readonly Effect ClearEffect;
        private readonly Quad Quad;
        private readonly Vector2 HalfPixel;
        private readonly RenderTarget2D ColorTarget;
        private readonly RenderTarget2D NormalTarget;
        private readonly RenderTarget2D DepthTarget;
        private readonly RenderTarget2D LightTarget;

        private readonly DirectionalLightSystem DirectionalLightSystem;

        public RenderSystem(GraphicsDevice device, Effect clearEffect, Effect directionalLightEffect, Scene scene)
        {
            this.Device = device;
            this.ClearEffect = clearEffect;
            this.Scene = scene;

            this.Quad = new Quad();

            var width = device.PresentationParameters.BackBufferWidth;
            var height = device.PresentationParameters.BackBufferHeight;

            this.HalfPixel = new Vector2(0.5f / width, 0.5f / height);

            this.ColorTarget  = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            this.NormalTarget = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            this.DepthTarget  = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.None);
            this.LightTarget  = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.None);

            this.DirectionalLightSystem = new DirectionalLightSystem(device, directionalLightEffect);
        }       

        public Scene Scene { get; set; }        

        public RenderTarget2D[] GetIntermediateRenderTargets() => new[]
        {
            this.ColorTarget,
            this.NormalTarget,
            this.DepthTarget,
            this.LightTarget
        };

        public void Render()
        {
            // Set and clear the G-Buffer
            this.Device.SetRenderTargets(this.ColorTarget, this.NormalTarget, this.DepthTarget);
            foreach (var pass in this.ClearEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.Quad.Render(this.Device);
            }

            // Draw scene
            this.Scene.Draw();

            // Resolve the G-Buffer
            this.Device.SetRenderTargets(null);

            // Set and clear the light buffer
            this.Device.SetRenderTarget(this.LightTarget);
            this.Device.Clear(Color.Transparent);

            // Draw the lights
            this.DirectionalLightSystem.Render(this.Scene.DirectionalLights, this.Scene.Camera, this.ColorTarget, this.NormalTarget, this.DepthTarget, this.HalfPixel);

            // Resolve the light buffer
            this.Device.SetRenderTarget(null);

            // Combine everything
        }      
    }
}

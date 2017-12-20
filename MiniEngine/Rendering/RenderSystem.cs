using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering
{
    public sealed class RenderSystem
    {
        private readonly GraphicsDevice Device;
        private readonly Effect ClearEffect;
        private readonly Effect RenderEffect;
        private readonly Quad Quad;
        private readonly RenderTarget2D Color;
        private readonly RenderTarget2D Normal;
        private readonly RenderTarget2D Depth;

        public RenderSystem(GraphicsDevice device, Effect clearEffect, Effect renderEffect, Scene scene)
        {
            this.Device = device;
            this.ClearEffect = clearEffect;
            this.RenderEffect = renderEffect;
            this.Scene = scene;

            this.Quad = new Quad();

            var width = device.PresentationParameters.BackBufferWidth;
            var height = device.PresentationParameters.BackBufferHeight;

            this.Color  = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            this.Normal = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            this.Depth  = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.None);           
        }       

        public Scene Scene { get; set; }        

        public RenderTarget2D[] GetGBuffer() => new[]
        {
            this.Color,
            this.Normal,
            this.Depth
        };

        public void Render()
        {
            SetGBuffer();
            ClearGBuffer();

            // Draw scene
            this.Scene.Draw();

            ResolveGBuffer();

            // Draw lights
            // Combine everything
        }

        private void SetGBuffer()
        {            
            this.Device.SetRenderTargets(this.Color, this.Normal, this.Depth);
        }

        private void ResolveGBuffer()
        {
            this.Device.SetRenderTargets(null);
        }

        private void ClearGBuffer()
        {
            foreach (var pass in this.ClearEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.Quad.Render(this.Device);
            }
        }
    }
}

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
        private readonly Effect CombineEffect;
        private readonly Quad Quad;
        private readonly RenderTarget2D ColorTarget;
        private readonly RenderTarget2D NormalTarget;
        private readonly RenderTarget2D DepthTarget;
        private readonly RenderTarget2D LightTarget;

        private readonly DirectionalLightSystem DirectionalLightSystem;
        private readonly PointLightSystem PointLightSystem;

        public RenderSystem(GraphicsDevice device, Effect clearEffect, Effect directionalLightEffect, Effect pointLightEffect, Model sphere, Effect combineEffect, Scene scene)
        {
            this.Device = device;
            this.ClearEffect = clearEffect;
            this.CombineEffect = combineEffect;

            this.Scene = scene;

            this.Quad = new Quad();

            var width = device.PresentationParameters.BackBufferWidth;
            var height = device.PresentationParameters.BackBufferHeight;

            this.ColorTarget  = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            this.NormalTarget = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            this.DepthTarget  = new RenderTarget2D(device, width, height, false, SurfaceFormat.Single, DepthFormat.None);
            this.LightTarget  = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.None);

            this.DirectionalLightSystem = new DirectionalLightSystem(device, directionalLightEffect);
            this.PointLightSystem = new PointLightSystem(device, pointLightEffect, sphere);
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
            foreach (var pass in this.ClearEffect.Techniques[0].Passes)
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
            this.PointLightSystem.Render(this.Scene.PointLights, this.Scene.Camera, this.ColorTarget, this.NormalTarget, this.DepthTarget);
            this.DirectionalLightSystem.Render(this.Scene.DirectionalLights, this.Scene.Camera, this.ColorTarget, this.NormalTarget, this.DepthTarget);
            
            // Resolve the light buffer
            this.Device.SetRenderTarget(null);

            // Combine everything
            using (this.Device.PostProcessState())
            {
                foreach (var pass in this.CombineEffect.Techniques[0].Passes)
                {
                    this.CombineEffect.Parameters["ColorMap"].SetValue(this.ColorTarget);
                    this.CombineEffect.Parameters["LightMap"].SetValue(this.LightTarget);                    

                    pass.Apply();
                    this.Quad.Render(this.Device);
                }
            }
        }      
    }
}

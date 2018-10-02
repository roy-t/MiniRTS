using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Effects;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering
{
    public sealed class DeferredRenderer
    {
        private readonly AmbientLightSystem AmbientLightSystem;
        private readonly ModelSystem ModelSystem;
        private readonly DirectionalLightSystem DirectionalLightSystem;
        private readonly PointLightSystem PointLightSystem;
        private readonly ShadowMapSystem ShadowMapSystem;
        private readonly ShadowCastingLightSystem ShadowCastingLightSystem;
        private readonly SunlightSystem SunlightSystem;
        private readonly DebugRenderSystem DebugRenderSystem;

        private readonly GraphicsDevice Device;
        private readonly Effect ClearEffect;                
        private readonly CombineEffect CombineEffect;
        private readonly PostProcessEffect PostProcessEffect;
        private readonly FullScreenTriangle FullScreenTriangle;
        private readonly GBuffer GBuffer;
        public RenderTarget2D CombineTarget { get; }
        public RenderTarget2D PostProcessTarget { get; }

        public DeferredRenderer(
            GraphicsDevice device,
            Effect clearEffect,
            CombineEffect combineEffect,
            PostProcessEffect postProcessEffect,
            AmbientLightSystem ambientLightSystem,
            ModelSystem modelSystem,
            DirectionalLightSystem directionalLightSystem,
            PointLightSystem pointLightSystem,
            ShadowMapSystem shadowMapSystem,
            ShadowCastingLightSystem shadowCastingLightSystem,
            SunlightSystem sunlightSystem,
            DebugRenderSystem debugRenderSystem)
        {
            this.Device = device;

            this.ClearEffect       = clearEffect;
            this.CombineEffect     = combineEffect;
            this.PostProcessEffect = postProcessEffect;

            this.AmbientLightSystem       = ambientLightSystem;
            this.ModelSystem              = modelSystem;
            this.DirectionalLightSystem   = directionalLightSystem;
            this.PointLightSystem         = pointLightSystem;
            this.ShadowMapSystem          = shadowMapSystem;
            this.ShadowCastingLightSystem = shadowCastingLightSystem;
            this.SunlightSystem           = sunlightSystem;
            this.DebugRenderSystem        = debugRenderSystem;

            this.FullScreenTriangle = new FullScreenTriangle();

            var width  = device.PresentationParameters.BackBufferWidth;
            var height = device.PresentationParameters.BackBufferHeight;

            this.GBuffer = new GBuffer(device, width, height);

            this.CombineTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);

            this.PostProcessTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);
        }

        public bool EnableFXAA { get; set; } = true;

        public RenderTarget2D[] GetIntermediateRenderTargets() => new[]
        {            
            this.GBuffer.DiffuseTarget,
            this.GBuffer.NormalTarget,
            this.GBuffer.DepthTarget,
            this.GBuffer.LightTarget,                  
        };

        public RenderTarget2D Render(PerspectiveCamera perspectiveCamera)
        {
            this.Device.SetRenderTarget(this.GBuffer.DiffuseTarget);
            this.Device.Clear(Color.TransparentBlack);

            this.Device.SetRenderTarget(this.PostProcessTarget);
            this.Device.Clear(Color.Black);

            this.SunlightSystem.Update(perspectiveCamera);
            this.ShadowMapSystem.RenderShadowMaps();

            

            var batches = this.ModelSystem.ComputeBatches(perspectiveCamera);



            RenderBatch(perspectiveCamera, batches.OpaqueBatch);            
            foreach (var batch in batches.TransparentBatches)
            {
                RenderBatch(perspectiveCamera, batch);
            }

            this.DebugRenderSystem.Render2DOverlay(perspectiveCamera);
            return this.PostProcessTarget;
        }

        private void RenderBatch(PerspectiveCamera camera, ModelRenderBatch batch)
        {
            RenderGBuffer(batch);
            this.DebugRenderSystem.Render3DOverlay(camera);
            RenderLights(camera);
            Combine();
            PostProcess();
            

        }

        private void PostProcess()
        {           
            this.Device.SetRenderTarget(this.PostProcessTarget);            
            using (this.Device.PostProcessState())
            {
                this.PostProcessEffect.ScaleX = 1.0f / this.CombineTarget.Width;
                this.PostProcessEffect.ScaleY = 1.0f / this.CombineTarget.Height;
                this.PostProcessEffect.DiffuseMap = this.CombineTarget;
                this.PostProcessEffect.NormalMap = this.GBuffer.NormalTarget;
                this.PostProcessEffect.Strength = this.EnableFXAA ? 2.0f : 0.0f;

                this.PostProcessEffect.Apply();

                this.FullScreenTriangle.Render(this.Device);
            }            
        }        

        private void Combine()
        {                        
            this.Device.SetRenderTarget(this.CombineTarget);
            this.Device.Clear(Color.TransparentBlack);

            using (this.Device.PostProcessState())
            {                
                this.CombineEffect.DiffuseMap = this.GBuffer.DiffuseTarget;
                this.CombineEffect.LightMap = this.GBuffer.LightTarget;

                this.CombineEffect.Apply();

                this.FullScreenTriangle.Render(this.Device);
            }

            this.Device.SetRenderTarget(null);
        }

        private void RenderLights(PerspectiveCamera perspectiveCamera)
        {                        
            this.Device.SetRenderTarget(this.GBuffer.LightTarget);
            
            this.Device.Clear(this.AmbientLightSystem.ComputeAmbientLightZeroAlpha());            
                                   
            this.DirectionalLightSystem.Render(perspectiveCamera, this.GBuffer);
            this.PointLightSystem.Render(perspectiveCamera, this.GBuffer);


            this.ShadowCastingLightSystem.RenderLights(perspectiveCamera, this.GBuffer);
            this.SunlightSystem.RenderLights(perspectiveCamera, this.GBuffer);            
            

            this.Device.SetRenderTarget(null);
        }

        private void RenderGBuffer(ModelRenderBatch batch)
        {
            this.Device.SetRenderTarget(this.GBuffer.DiffuseTarget);
            this.Device.Clear(ClearOptions.Target, Color.TransparentBlack, 1.0f, 0);

            this.Device.SetRenderTarget(this.GBuffer.NormalTarget);
            this.Device.Clear(new Color(0.5f, 0.5f, 0.5f, 0.0f));

            this.Device.SetRenderTarget(this.GBuffer.DepthTarget);
            this.Device.Clear(Color.TransparentBlack);

            // TODO: why doesn't clearing using the effect work for the alpha channel of the DiffuseTarget? WTF
            //this.Device.SetRenderTargets(this.GBuffer.DiffuseTarget, this.GBuffer.NormalTarget, this.GBuffer.DepthTarget);
            //using (this.Device.GeometryState())
            //{
            //    foreach (var pass in this.ClearEffect.Techniques[0].Passes)
            //    {
            //        pass.Apply();
            //        this.Quad.Render(this.Device);
            //    }
            //}

            this.Device.SetRenderTargets(this.GBuffer.DiffuseTarget, this.GBuffer.NormalTarget, this.GBuffer.DepthTarget);
            using (this.Device.GeometryState())
            {
                batch.Draw(Techniques.MRT);
            }            
        }        
    }
}

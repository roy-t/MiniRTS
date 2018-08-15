using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering
{
    public sealed class DeferredRenderer
    {
        public readonly AmbientLightSystem AmbientLightSystem;
        public readonly ModelSystem ModelSystem;
        public readonly DirectionalLightSystem DirectionalLightSystem;
        public readonly PointLightSystem PointLightSystem;
        private readonly ShadowMapSystem ShadowMapSystem;
        public readonly ShadowCastingLightSystem ShadowCastingLightSystem;
        public readonly SunlightSystem SunlightSystem;

        private readonly GraphicsDevice Device;
        private readonly Effect ClearEffect;                
        private readonly Effect CombineEffect;
        private readonly Effect PostProcessEffect;
        private readonly Quad Quad;
        private readonly GBuffer GBuffer;
        public RenderTarget2D CombineTarget { get; }

        public DeferredRenderer(
            GraphicsDevice device,
            Effect clearEffect,
            Effect combineEffect,
            Effect postProcessEffect,
            AmbientLightSystem ambientLightSystem,
            ModelSystem modelSystem,
            DirectionalLightSystem directionalLightSystem,
            PointLightSystem pointLightSystem,
            ShadowMapSystem shadowMapSystem,
            ShadowCastingLightSystem shadowCastingLightSystem,
            SunlightSystem sunlightSystem)
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

            this.Quad = new Quad();         

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
                RenderTargetUsage.DiscardContents);
        }

        public bool EnableFXAA { get; set; } = true;

        public RenderTarget2D[] GetIntermediateRenderTargets() => new[]
        {            

            //this.GBuffer.DiffuseTarget,
            //this.GBuffer.NormalTarget,
            //this.GBuffer.DepthTarget,
            //this.GBuffer.LightTarget,
            //this.CombineTarget,
            this.ShadowMapSystem.DebugFoo().DepthMap,
            this.ShadowMapSystem.DebugFoo().ColorMap,
        };

        public void Render(PerspectiveCamera perspectiveCamera)
        {            
            RenderGBuffer(perspectiveCamera);

            RenderLights(perspectiveCamera);            

            Combine();

            PostProcess();
        }   

        private void PostProcess()
        {            
            using (this.Device.PostProcessState())
            {
                // Post process the image
                foreach (var pass in this.PostProcessEffect.Techniques[0].Passes)
                {
                    this.PostProcessEffect.Parameters["ScaleX"].SetValue(1.0f / this.CombineTarget.Width);
                    this.PostProcessEffect.Parameters["ScaleY"].SetValue(1.0f / this.CombineTarget.Height);
                    this.PostProcessEffect.Parameters["DiffuseMap"].SetValue(this.CombineTarget);
                    this.PostProcessEffect.Parameters["NormalMap"].SetValue(this.GBuffer.NormalTarget);                                        
                    this.PostProcessEffect.Parameters["Strength"].SetValue(this.EnableFXAA? 2.0f : 0.0f);                    
                    pass.Apply();

                    this.Quad.Render(this.Device);
                }
            }            
        }        

        private void Combine()
        {
            this.Device.SetRenderTarget(this.CombineTarget);

            using (this.Device.PostProcessState())
            {
                // Combine everything
                foreach (var pass in this.CombineEffect.Techniques[0].Passes)
                {
                    this.CombineEffect.Parameters["DiffuseMap"].SetValue(this.GBuffer.DiffuseTarget);
                    this.CombineEffect.Parameters["LightMap"].SetValue(this.GBuffer.LightTarget);                    

                    pass.Apply();
                    this.Quad.Render(this.Device);
                }
            }

            this.Device.SetRenderTarget(null);
        }

        private void RenderLights(PerspectiveCamera perspectiveCamera)
        {            
            this.SunlightSystem.Update(perspectiveCamera);

            this.ShadowMapSystem.RenderShadowMaps();            
            

            this.Device.SetRenderTarget(this.GBuffer.LightTarget);
            
            this.Device.Clear(this.AmbientLightSystem.ComputeAmbientLightZeroAlpha());            
                                   
            this.DirectionalLightSystem.Render(perspectiveCamera, this.GBuffer);
            this.PointLightSystem.Render(perspectiveCamera, this.GBuffer);


            this.ShadowCastingLightSystem.RenderLights(perspectiveCamera, this.GBuffer);
            this.SunlightSystem.RenderLights(perspectiveCamera, this.GBuffer);            
            

            this.Device.SetRenderTarget(null);
        }

        private void RenderGBuffer(IViewPoint viewPoint)
        {
            this.Device.SetRenderTargets(this.GBuffer.DiffuseTarget, this.GBuffer.NormalTarget, this.GBuffer.DepthTarget);

            using (this.Device.PostProcessState())
            {
                foreach (var pass in this.ClearEffect.Techniques[0].Passes)
                {
                    pass.Apply();
                    this.Quad.Render(this.Device);
                }
            }
            
            using (this.Device.GeometryState())
            {
                this.ModelSystem.DrawOpaqueModels(viewPoint);                
            }

            this.Device.SetRenderTargets(null);
        }
    }
}

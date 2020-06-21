using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Primitives
{
    public sealed class GBuffer : IDisposable
    {
        private static readonly Color NormalClearColor = new Color(0.5f, 0.5f, 0.5f, 0.0f);
        private static readonly Color ParticleClearColor = new Color(1.0f, 0, 0, 0);

        public GBuffer(GraphicsDevice device, int width, int height)
        {
            this.Device = device;

            // Do not enable AA as we use FXAA during post processing
            const int aaSamples = 0;

            this.DiffuseTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.HalfVector4,
                DepthFormat.Depth24,
                aaSamples,
                RenderTargetUsage.PreserveContents);

            this.NormalTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                aaSamples,
                RenderTargetUsage.PreserveContents);

            this.DepthTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.Single,
                DepthFormat.None,
                aaSamples,
                RenderTargetUsage.PreserveContents);

            this.ParticleTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.HalfSingle,
                DepthFormat.None,
                aaSamples,
                RenderTargetUsage.PreserveContents);

            this.LightTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);

            this.CombineTarget = new RenderTarget2D(
              device,
              width,
              height,
              false,
              SurfaceFormat.Color,
              DepthFormat.None,
              0,
              RenderTargetUsage.PreserveContents);

            this.BlurTarget = new RenderTarget2D(
               device,
               width,
               height,
               false,
               SurfaceFormat.Color,
               DepthFormat.None,
               0,
               RenderTargetUsage.PreserveContents);

            this.FinalTarget = new RenderTarget2D(
              device,
              width,
              height,
              false,
              SurfaceFormat.Color,
              DepthFormat.None,
              0,
              RenderTargetUsage.PreserveContents);


        }

        public GraphicsDevice Device { get; }

        public RenderTarget2D DiffuseTarget { get; }
        public RenderTarget2D NormalTarget { get; }
        public RenderTarget2D ParticleTarget { get; }
        public RenderTarget2D DepthTarget { get; }
        public RenderTarget2D LightTarget { get; }
        public RenderTarget2D BlurTarget { get; }
        public RenderTarget2D CombineTarget { get; }
        public RenderTarget2D FinalTarget { get; }
        public float AspectRatio => this.DiffuseTarget.Width / (float)this.DiffuseTarget.Height;

        public void ClearAllTargets()
        {
            this.SetClearDiffuseTarget();
            this.SetClearNormalTarget();
            this.SetClearParticleTarget();
            this.SetClearDepthTarget();
            this.SetClearLightTarget();
            this.SetClearBlurTarget();
            this.SetClearCombineTarget();
            this.SetClearFinalTarget();
        }

        public void SetClearDiffuseTarget()
        {
            this.Device.SetRenderTarget(this.DiffuseTarget);
            this.Device.Clear(Color.TransparentBlack);
        }

        public void SetClearDiffuseTargetColorOnly()
        {
            this.Device.SetRenderTarget(this.DiffuseTarget);
            this.Device.Clear(ClearOptions.Target, Color.TransparentBlack, 1.0f, 0);
        }

        public void SetClearNormalTarget()
        {
            this.Device.SetRenderTarget(this.NormalTarget);
            this.Device.Clear(NormalClearColor);
        }

        public void SetClearParticleTarget()
        {
            this.Device.SetRenderTarget(this.ParticleTarget);
            this.Device.Clear(ParticleClearColor);
        }

        public void SetClearDepthTarget()
        {
            this.Device.SetRenderTarget(this.DepthTarget);
            this.Device.Clear(Color.White); // clear to max distance
        }

        public void SetClearLightTarget()
        {
            this.Device.SetRenderTarget(this.LightTarget);
            this.Device.Clear(Color.TransparentBlack);
        }

        public void SetClearBlurTarget()
        {
            this.Device.SetRenderTarget(this.BlurTarget);
            this.Device.Clear(Color.TransparentBlack);
        }

        public void SetClearCombineTarget()
        {
            this.Device.SetRenderTarget(this.CombineTarget);
            this.Device.Clear(Color.TransparentBlack);
        }

        public void SetClearFinalTarget()
        {
            this.Device.SetRenderTarget(this.FinalTarget);
            this.Device.Clear(Color.TransparentBlack);
        }

        public void Dispose()
        {
            this.DiffuseTarget.Dispose();
            this.NormalTarget.Dispose();
            this.ParticleTarget.Dispose();
            this.DepthTarget.Dispose();
            this.LightTarget.Dispose();
            this.BlurTarget.Dispose();
            this.CombineTarget.Dispose();
            this.FinalTarget.Dispose();
        }
    }
}
using System;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Telemetry;
using MiniEngine.Units;

namespace MiniEngine.Rendering
{
    public sealed class DeferredRenderPipeline : IDisposable
    {
        private readonly GBuffer GBuffer;
        private readonly RenderPipelineInput Input;
        private readonly RenderPipelineBuilder RenderPipelineBuilder;
        private readonly AnimationPipelineBuilder AnimationPipelineBuilder;
        private readonly IMeterRegistry MeterRegistry;
        private readonly Pass RootPass;

        private RenderPipeline renderPipeline;
        private RenderPipeline animationPipeline;

        public DeferredRenderPipeline(
            RenderPipelineBuilder renderPipelineBuilder,
            AnimationPipelineBuilder animationPipelineBuilder,
            IMeterRegistry meterRegistry,
            GraphicsDevice device)
        {
            this.RenderPipelineBuilder = renderPipelineBuilder;
            this.AnimationPipelineBuilder = animationPipelineBuilder;
            this.MeterRegistry = meterRegistry;

            var width = device.PresentationParameters.BackBufferWidth;
            var height = device.PresentationParameters.BackBufferHeight;
            this.GBuffer = new GBuffer(device, width, height);

            this.Input = new RenderPipelineInput();

            this.Settings = new RenderPipelineSettings();
            this.RootPass = new Pass(PassType.Opaque, 0);

            this.Recreate();
        }

        public RenderPipelineSettings Settings { get; }

        public void Recreate()
        {
            this.animationPipeline = this.AnimationPipelineBuilder.Build(this.GBuffer.Device, this.MeterRegistry);
            this.renderPipeline = this.RenderPipelineBuilder.Build(this.GBuffer.Device, this.MeterRegistry, this.Settings);
        }


        public RenderTarget2D Render(PerspectiveCamera camera, Seconds elapsed, TextureCube skybox)
        {
            this.Input.Update(camera, elapsed, this.GBuffer, this.RootPass, skybox);
            this.animationPipeline.Execute(this.Input, "animation");
            this.renderPipeline.Execute(this.Input, "render");

            return this.GBuffer.FinalTarget;
        }

        public GBuffer GetGBuffer() => this.GBuffer;

        public void Dispose() => this.GBuffer.Dispose();
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class ClearStage : IPipelineStage, IModelPipelineStage, ILightingPipelineStage, IParticlePipelineStage
    {
        private readonly GraphicsDevice Device;
        private readonly RenderTarget2D RenderTarget;

        public ClearStage(
            GraphicsDevice device,
            RenderTarget2D renderTarget,
            ClearOptions options,
            Color color,
            float depth,
            int stencil)
        {
            this.Device = device;
            this.RenderTarget = renderTarget;
            this.Options = options;
            this.Color = color;
            this.Depth = depth;
            this.Stencil = stencil;
        }

        public ClearOptions Options { get; }
        public Color Color { get; }
        public float Depth { get; }
        public int Stencil { get; }

        public void Execute(PerspectiveCamera _, GBuffer __) => this.Execute();

        public void Execute(PerspectiveCamera _, ModelRenderBatch __) => this.Execute();

        public void Execute(PerspectiveCamera _, ParticleRenderBatch __) => this.Execute();

        public void Execute(PerspectiveCamera _, Seconds seconds) => this.Execute();

        private void Execute()
        {
            this.Device.SetRenderTarget(this.RenderTarget);
            this.Device.Clear(this.Options, this.Color, this.Depth, this.Stencil);
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering.Pipelines
{
    public sealed class ClearStage : IPipelineStage, IModelPipelineStage, ILightingPipelineStage
    {
        private readonly GraphicsDevice Device;
        private readonly RenderTarget2D RenderTarget;

        public ClearStage(GraphicsDevice device, RenderTarget2D renderTarget, ClearOptions options, Color color, float depth, int stencil)
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

        public void Execute(PerspectiveCamera _)
        {
            this.Device.SetRenderTarget(this.RenderTarget);
            this.Device.Clear(this.Options, this.Color, this.Depth, this.Stencil);
        }

        public void Execute(PerspectiveCamera _, ModelRenderBatch __)
            => Execute(_);

        public void Execute(PerspectiveCamera _, GBuffer __)
            => Execute(_);
    }
}

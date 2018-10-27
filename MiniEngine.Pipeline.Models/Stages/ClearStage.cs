using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Batches;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Rendering.Pipelines;

namespace MiniEngine.Pipeline.Models.Stages
{
    public sealed class ClearStage : IModelPipelineStage
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

        public void Execute(PerspectiveCamera _, ModelRenderBatch __) => this.Execute();

        private void Execute()
        {
            this.Device.SetRenderTarget(this.RenderTarget);
            this.Device.Clear(this.Options, this.Color, this.Depth, this.Stencil);
        }
    }
}

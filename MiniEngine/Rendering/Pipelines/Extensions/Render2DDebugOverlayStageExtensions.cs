using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class Render2DDebugOverlayStageExtensions
    {
        public static Pipeline Render2DDebugOverlay(
            this Pipeline pipeline,
            DebugRenderSystem debugRenderSystem,
            RenderTarget2D destinationTarget)
        {
            var stage = new Render2DDebugOverlayStage(pipeline.Device, debugRenderSystem, destinationTarget);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}

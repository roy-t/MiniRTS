using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Stages;
using MiniEngine.Pipeline.Systems;

namespace MiniEngine.Pipeline.Extensions
{
    public static class Render2DDebugOverlayStageExtensions
    {
        public static RenderPipeline Render2DDebugOverlay(
            this RenderPipeline pipeline,
            DebugRenderSystem debugRenderSystem,
            RenderTarget2D destinationTarget)
        {
            var stage = new Render2DDebugOverlayStage(pipeline.Device, debugRenderSystem, destinationTarget);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
using MiniEngine.Pipeline.Stages;
using MiniEngine.Pipeline.Systems;

namespace MiniEngine.Pipeline.Extensions
{
    public static class Render2DDebugOverlayStageExtensions
    {
        public static RenderPipeline Render2DDebugOverlay(
            this RenderPipeline pipeline,
            DebugRenderSystem debugRenderSystem)
        {
            var stage = new Render2DDebugOverlayStage(pipeline.Device, debugRenderSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
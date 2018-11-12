using MiniEngine.Pipeline.Stages;
using MiniEngine.Pipeline.Systems;

namespace MiniEngine.Pipeline.Extensions
{
    public static class Render3DDebugOverlayExtensions
    {
        public static RenderPipeline Render3DDebugOverlay(
            this RenderPipeline pipeline,
            DebugRenderSystem debugRenderSystem)
        {
            var stage = new Render3DDebugOverlayStage(pipeline.Device, debugRenderSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
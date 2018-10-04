using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class Render3DDebugOverlayExtensions
    {
        public static ModelPipeline Render3DDebugOverlay(
            this ModelPipeline pipeline,
            DebugRenderSystem debugRenderSystem,
            GBuffer gBuffer)
        {
            var stage = new Render3DDebugOverlayStage(pipeline.Device, debugRenderSystem, gBuffer);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
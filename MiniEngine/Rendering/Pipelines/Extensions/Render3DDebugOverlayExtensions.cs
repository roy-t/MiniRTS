using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline;
using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class Render3DDebugOverlayExtensions
    {
        public static RenderPipeline Render3DDebugOverlay(
            this RenderPipeline pipeline,
            DebugRenderSystem debugRenderSystem,
            RenderTarget2D target)
        {
            var stage = new Render3DDebugOverlayStage(pipeline.Device, debugRenderSystem, target);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
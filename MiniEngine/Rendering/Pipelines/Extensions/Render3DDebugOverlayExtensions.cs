using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class Render3DDebugOverlayExtensions
    {
        public static Pipeline Render3DDebugOverlay(
            this Pipeline pipeline,
            DebugRenderSystem debugRenderSystem,
            RenderTarget2D target)
        {
            var stage = new Render3DDebugOverlayStage(pipeline.Device, debugRenderSystem, target);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
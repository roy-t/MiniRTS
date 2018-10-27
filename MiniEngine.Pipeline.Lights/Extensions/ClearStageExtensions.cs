using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Stages;
using MiniEngine.Rendering.Pipelines;

namespace MiniEngine.Pipeline.Lights.Extensions
{
    public static class ClearStageExtensions
    {
        public static LightingPipeline Clear(
           this LightingPipeline pipeline,
           RenderTarget2D renderTarget,
           ClearOptions options,
           Color color,
           float depth,
           int stencil)
        {
            var stage = new ClearStage(pipeline.Device, renderTarget, options, color, depth, stencil);
            pipeline.Add(stage);
            return pipeline;
        }

        public static LightingPipeline Clear(this LightingPipeline pipeline, RenderTarget2D renderTarget, Color color)
        {
            return Clear(
                pipeline,
                renderTarget,
                ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil,
                color,
                1,
                0);
        }
    }
}

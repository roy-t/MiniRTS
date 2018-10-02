using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class ClearStageExtensions
    {
        public static Pipeline Clear(
            this Pipeline pipeline,
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

        public static Pipeline Clear(this Pipeline pipeline, RenderTarget2D renderTarget, Color color)
            => Clear(pipeline, renderTarget, ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, color, 1, 0);

        public static ModelPipeline Clear(
            this ModelPipeline pipeline,
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

        public static ModelPipeline Clear(this ModelPipeline pipeline, RenderTarget2D renderTarget, Color color)
            => Clear(pipeline, renderTarget, ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, color, 1, 0);

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
            => Clear(pipeline, renderTarget, ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, color, 1, 0);
    }
}

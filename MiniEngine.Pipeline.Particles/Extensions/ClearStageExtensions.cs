using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Particles.Stages;

namespace MiniEngine.Pipeline.Particles.Extensions
{
    public static class ClearStageExtensions
    {
          public static ParticlePipeline Clear(
            this ParticlePipeline pipeline,
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

        public static ParticlePipeline Clear(this ParticlePipeline pipeline, RenderTarget2D renderTarget, Color color)
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

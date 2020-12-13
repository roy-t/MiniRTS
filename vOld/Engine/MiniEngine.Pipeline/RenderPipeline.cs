using System;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Telemetry;

namespace MiniEngine.Pipeline
{
    public sealed class RenderPipeline : APipeline<RenderPipelineInput>
    {
        public RenderPipeline(GraphicsDevice device, IMeterRegistry meterRegistry)
            : base(device, meterRegistry, "render_pipeline") { }

        public static RenderPipeline Create(GraphicsDevice device, IMeterRegistry meterRegistry) => new RenderPipeline(device, meterRegistry);

        public RenderPipeline EnableIf(bool enabled, Func<RenderPipeline, RenderPipeline> factory)
        {
            if (enabled)
            {
                return factory(this);
            }
            return this;
        }
    }
}
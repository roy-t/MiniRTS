using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Telemetry;
using System;

namespace MiniEngine.Pipeline.Lights
{
    public sealed class LightingPipeline : APipeline<LightingPipelineInput>
    {
        public LightingPipeline(GraphicsDevice device, IMeterRegistry meterRegistry)
            : base(device, meterRegistry, "lighting_pipeline")
        {
        }

        public static LightingPipeline Create(GraphicsDevice device, IMeterRegistry meterRegistry) => new LightingPipeline(device, meterRegistry);

        public LightingPipeline EnableIf(bool enabled, Func<LightingPipeline, LightingPipeline> factory)
        {
            if(enabled)
            {
                return factory(this);
            }
            return this;
        }
    }
}
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Telemetry;

namespace MiniEngine.Pipeline.Shadows
{
    public sealed class ShadowPipeline : APipeline<ShadowPipelineInput>
    {
        private ShadowPipeline(GraphicsDevice device, IMeterRegistry meterRegistry)
            : base(device, meterRegistry, "shadow_pipeline")
        {
        }

        public static ShadowPipeline Create(GraphicsDevice device, IMeterRegistry meterRegistry) => new ShadowPipeline(device, meterRegistry);
    }
}

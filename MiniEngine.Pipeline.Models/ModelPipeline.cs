using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Telemetry;

namespace MiniEngine.Pipeline.Models
{
    public sealed class ModelPipeline : APipeline<ModelPipelineInput>
    {
        private ModelPipeline(GraphicsDevice device, IMeterRegistry meterRegistry)
            : base(device, meterRegistry, "model_pipeline")
        {
        }

        public static ModelPipeline Create(GraphicsDevice device, IMeterRegistry meterRegistry) => new ModelPipeline(device, meterRegistry);
    }
}
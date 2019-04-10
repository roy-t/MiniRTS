using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Telemetry;

namespace MiniEngine.Pipeline.Projectors
{
    public sealed class ProjectorPipeline : APipeline<ProjectorPipelineInput>
    {
        private ProjectorPipeline(GraphicsDevice device, IMeterRegistry meterRegistry) 
            : base(device, meterRegistry, "projector_pipeline")
        {
        }

        public static ProjectorPipeline Create(GraphicsDevice device, IMeterRegistry meterRegistry) => new ProjectorPipeline(device, meterRegistry);
    }
}

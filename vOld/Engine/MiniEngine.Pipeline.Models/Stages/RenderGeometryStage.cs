using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Pipeline.Models.Systems;

namespace MiniEngine.Pipeline.Models.Stages
{
    public sealed class RenderGeometryStage : IPipelineStage<ModelPipelineInput>
    {
        private readonly GraphicsDevice Device;
        private readonly GeometrySystem GeometrySystem;

        public RenderGeometryStage(GraphicsDevice device, GeometrySystem geometrySystem)
        {
            this.Device = device;
            this.GeometrySystem = geometrySystem;
        }

        public void Execute(ModelPipelineInput input)
        {
            this.Device.SetRenderTargets(
                input.GBuffer.DiffuseTarget,
                input.GBuffer.NormalTarget,
                input.GBuffer.DepthTarget);

            this.Device.GeometryState();
            this.GeometrySystem.Render(input.Camera);
        }
    }
}
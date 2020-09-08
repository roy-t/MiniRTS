using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Next;
using MiniEngine.Primitives;
using MiniEngine.Systems.Next;

namespace MiniEngine.Next.Rendering
{
    public sealed class ParallelRenderPipelineBuilder
    {
        private readonly Resolver<ISystem> Systems;

        public ParallelRenderPipelineBuilder(Resolver<ISystem> systems)
        {
            this.Systems = systems;
        }

        public ParallelPipeline Build(GraphicsDevice device)
        {
            var builder = new PipelineBuilder();


        }
    }




    public class OpaqueModelClearSystem : ISystemWithoutComponent
    {
        private readonly GBuffer GBuffer;

        public OpaqueModelClearSystem(GBuffer gBuffer)
        {
            this.GBuffer = gBuffer;
        }

        public void Process()
        {
            this.GBuffer.SetClearDiffuseTargetColorOnly();
            this.GBuffer.SetClearNormalTarget();
            this.GBuffer.SetClearDepthTarget();
            this.GBuffer.SetClearCombineTarget();
        }
    }

    public class TransparentModelClearSystem : ISystemWithoutComponent
    {
        private readonly GBuffer GBuffer;

        public TransparentModelClearSystem(GBuffer gBuffer)
        {
            this.GBuffer = gBuffer;
        }

        public void Process()
        {
            this.GBuffer.SetClearDiffuseTargetColorOnly();
            this.GBuffer.SetClearNormalTarget();
            this.GBuffer.SetClearCombineTarget();
        }
    }
}

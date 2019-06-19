using Microsoft.Xna.Framework;

namespace MiniEngine.Pipeline.Stages
{
    public sealed class ClearStage : IPipelineStage<RenderPipelineInput>
    {
        private readonly Color NormalClearColor;
        
        public ClearStage()
        {
            this.NormalClearColor = new Color(0.5f, 0.5f, 0.5f, 0.0f);
        }

        public void Execute(RenderPipelineInput input) 
            => input.GBuffer.ClearAllTargets();
    }
}

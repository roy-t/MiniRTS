using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Pipeline.Stages
{
    public sealed class ClearStage : IPipelineStage<RenderPipelineStageInput>
    {
        private readonly GraphicsDevice Device;
        private readonly Color NormalClearColor;
        
        public ClearStage(GraphicsDevice device)
        {
            this.Device = device;
            this.NormalClearColor = new Color(0.5f, 0.5f, 0.5f, 0.0f);
        }

        public void Execute(RenderPipelineStageInput input)
        {            
            this.Device.SetRenderTarget(input.GBuffer.DiffuseTarget);
            this.Device.Clear(Color.TransparentBlack);

            this.Device.SetRenderTarget(input.GBuffer.NormalTarget);
            this.Device.Clear(this.NormalClearColor);
            
            this.Device.SetRenderTarget(input.GBuffer.DepthTarget);
            this.Device.Clear(Color.TransparentBlack);

            this.Device.SetRenderTarget(input.GBuffer.LightTarget);
            this.Device.Clear(Color.TransparentBlack);

            this.Device.SetRenderTarget(input.GBuffer.CombineTarget);
            this.Device.Clear(Color.TransparentBlack);

            this.Device.SetRenderTarget(input.GBuffer.FinalTarget);
            this.Device.Clear(Color.Black);            
        }
    }
}

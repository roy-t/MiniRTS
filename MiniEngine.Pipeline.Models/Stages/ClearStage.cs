﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Pipeline.Models.Stages
{
    public sealed class ClearStage : IPipelineStage<ModelPipelineInput>
    {
        private readonly GraphicsDevice Device;
        private readonly Color NormalClearColor;

        public ClearStage(GraphicsDevice device)
        {
            this.Device = device;
            this.NormalClearColor = new Color(0.5f, 0.5f, 0.5f, 0.0f);
        }

        public void Execute(ModelPipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.DiffuseTarget);
            this.Device.Clear(ClearOptions.Target, Color.TransparentBlack, 1.0f, 0);
            
            this.Device.SetRenderTarget(input.GBuffer.NormalTarget);
            this.Device.Clear(this.NormalClearColor);

            // TODO: do not compare to string!
            // only clear the depth buffer in the opaque stage
            if (input.Pass == "opaque")
            {
                this.Device.SetRenderTarget(input.GBuffer.DepthTarget);
                this.Device.Clear(Color.TransparentBlack);
            }

            this.Device.SetRenderTarget(input.GBuffer.CombineTarget);
            this.Device.Clear(Color.TransparentBlack);
        }
    }
}

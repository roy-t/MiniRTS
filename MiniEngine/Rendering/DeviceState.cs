using System;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering
{
    internal sealed class DeviceState : IDisposable
    {
        private readonly GraphicsDevice Device;
        private readonly BlendState PreviousBlendState;
        private readonly DepthStencilState PreviousDepthStencilState;
        private readonly RasterizerState PreviousRasterizerState;

        public DeviceState(GraphicsDevice device, BlendState blendState, DepthStencilState depthStencilState, RasterizerState rasterizerState)
        {
            this.Device = device;
            this.PreviousBlendState = device.BlendState;
            this.PreviousDepthStencilState = device.DepthStencilState;
            this.PreviousRasterizerState = device.RasterizerState;

            device.BlendState = blendState;
            device.DepthStencilState = depthStencilState;
            device.RasterizerState = rasterizerState;
        }

        public void Dispose()
        {
            this.Device.BlendState = this.PreviousBlendState;
            this.Device.DepthStencilState = this.PreviousDepthStencilState;
            this.Device.RasterizerState = this.PreviousRasterizerState;
        }
    }
}

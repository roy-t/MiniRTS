using Microsoft.Xna.Framework.Graphics;
using System;

namespace MiniEngine.Rendering
{
    internal sealed class DeviceState : IDisposable
    {
        private readonly GraphicsDevice Device;
        private readonly BlendState PreviousBlendState;
        private readonly DepthStencilState PreviousDepthStencilState;
        private readonly RasterizerState PreviousRasterizerState;
        private readonly SamplerState PreviousSamplerState;

        public DeviceState(
            GraphicsDevice device,
            BlendState blendState,
            DepthStencilState depthStencilState,
            RasterizerState rasterizerState)
            : this(device, blendState, depthStencilState, rasterizerState, device.SamplerStates[0])
        {

        }

        public DeviceState(GraphicsDevice device, BlendState blendState, DepthStencilState depthStencilState, RasterizerState rasterizerState, SamplerState samplerState)
        {
            this.Device = device;
            this.PreviousBlendState = device.BlendState;
            this.PreviousDepthStencilState = device.DepthStencilState;
            this.PreviousRasterizerState = device.RasterizerState;
            this.PreviousSamplerState = device.SamplerStates[0];

            device.BlendState = blendState;
            device.DepthStencilState = depthStencilState;
            device.RasterizerState = rasterizerState;
            device.SamplerStates[0] = samplerState;
        }

        public void Dispose()
        {
            this.Device.BlendState = this.PreviousBlendState;
            this.Device.DepthStencilState = this.PreviousDepthStencilState;
            this.Device.RasterizerState = this.PreviousRasterizerState;
            this.Device.SamplerStates[0] = this.PreviousSamplerState;
        }
    }
}

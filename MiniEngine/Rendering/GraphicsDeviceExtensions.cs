using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering
{
    internal static class GraphicsDeviceExtensions
    {
        public static DeviceState DefaultState(this GraphicsDevice device)
        {
            return new DeviceState(
                device,
                BlendState.AlphaBlend,
                DepthStencilState.Default,
                RasterizerState.CullCounterClockwise,
                SamplerState.LinearClamp);
        }

        /// <summary>
        /// Graphics device state for drawing geometry to the G-Buffer
        /// </summary>
        public static DeviceState GeometryState(this GraphicsDevice device)
        {
            return new DeviceState(
                device,
                BlendState.Opaque,
                DepthStencilState.Default,
                RasterizerState.CullCounterClockwise,
                SamplerState.LinearClamp);
        }

        /// <summary>
        /// Graphics device state for drawing lights to the Light Target
        /// </summary>
        public static DeviceState LightState(this GraphicsDevice device)
        {
            return new DeviceState(
                device,
                BlendState.AlphaBlend,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                SamplerState.LinearClamp);
        }

        public static DeviceState WireFrameState(this GraphicsDevice device)
        {
            var rasterState = new RasterizerState()
            {
                CullMode = CullMode.CullCounterClockwiseFace,
                FillMode = FillMode.WireFrame
            };

            return new DeviceState(
                device,
                BlendState.AlphaBlend,
                DepthStencilState.Default,
                rasterState,
                SamplerState.LinearClamp);
        }
    }
}

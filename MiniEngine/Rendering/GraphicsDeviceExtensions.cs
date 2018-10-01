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
                RasterizerState.CullCounterClockwise);
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
                RasterizerState.CullCounterClockwise);
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
                RasterizerState.CullCounterClockwise);
        }

        /// <summary>
        /// Graphics device state for drawing sunlights to the Light Target
        /// </summary>        
        public static DeviceState SunlightState(this GraphicsDevice device)
        {
            var samplerState = new SamplerState
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                Filter = TextureFilter.Anisotropic,
                ComparisonFunction = CompareFunction.LessEqual,
                FilterMode = TextureFilterMode.Comparison
            };

            return new DeviceState(
                device,
                BlendState.AlphaBlend,
                DepthStencilState.Default,
                RasterizerState.CullCounterClockwise,
                samplerState);
        }

        public static DeviceState ShadowMapState(this GraphicsDevice device)
        {
            var rasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                DepthClipEnable = false
            };

            return new DeviceState(
                device,
                BlendState.Opaque,
                DepthStencilState.Default,
                rasterizerState);
        }

        public static DeviceState ColorMapState(this GraphicsDevice device)
        {
            // Similar to the alpha blending state but first divides
            // the source components by the inverse alpha, thus making
            // objects that are very opaque darker (as if they let less light through)
            var blendState = new BlendState
            {
                ColorSourceBlend = Blend.InverseSourceAlpha,
                AlphaSourceBlend = Blend.InverseSourceAlpha,

                ColorDestinationBlend = Blend.InverseSourceAlpha,
                AlphaDestinationBlend = Blend.InverseSourceAlpha
            };

            var rasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                DepthClipEnable = false
            };

            return new DeviceState(
                device,
                blendState, 
                DepthStencilState.DepthRead,
                rasterizerState);
        }

        public static DeviceState PostProcessState(this GraphicsDevice device)
        {
            return new DeviceState(
                device,
                BlendState.AlphaBlend,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);
        }

        public static DeviceState WireFrameState(this GraphicsDevice device)
        {            
            var rasterState = new RasterizerState
            {
                CullMode = CullMode.None,                
                FillMode = FillMode.WireFrame
            };

            return new DeviceState(
                device,
                BlendState.AlphaBlend,
                DepthStencilState.Default,
                rasterState);
        }
    }
}

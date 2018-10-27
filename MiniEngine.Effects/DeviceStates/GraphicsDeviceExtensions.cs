using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects.DeviceStates
{
    public static class GraphicsDeviceExtensions
    {
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
        /// Graphics device state for drawing particles to the G-Buffer
        /// </summary>
        public static DeviceState ParticleState(this GraphicsDevice device)
        {
            return new DeviceState(
                device,
                BlendState.Additive,
                DepthStencilState.Default,
                RasterizerState.CullNone);
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
        /// Graphics device state for drawing lights that cast shadows to the Light Target
        /// </summary>
        public static DeviceState ShadowCastingLightState(this GraphicsDevice device)
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
                DepthStencilState.None,
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

        public static DeviceState AdditiveBlendOccluderState(this GraphicsDevice device)
        {
            // Similar to the additive blending but substracts the colors 
            // thus making objects that are very opaque darker (as if they let less light through)
            var blendState = new BlendState
            {
                AlphaBlendFunction = BlendFunction.ReverseSubtract,
                ColorBlendFunction = BlendFunction.ReverseSubtract,

                AlphaSourceBlend = Blend.InverseSourceAlpha,
                ColorSourceBlend = Blend.InverseSourceAlpha,

                AlphaDestinationBlend = Blend.InverseSourceAlpha,
                ColorDestinationBlend = Blend.InverseSourceAlpha
            };

            var rasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                DepthClipEnable = true
            };

            return new DeviceState(
                device,
                blendState,
                DepthStencilState.DepthRead,
                rasterizerState);
        }

        public static DeviceState AlphaBlendOccluderState(this GraphicsDevice device)
        {
            // Similar to the alpha blending state but first divides
            // the source components by the inverse alpha, thus making
            // objects that are very opaque darker (as if they let less light through)
            var blendState = new BlendState
            {
                AlphaBlendFunction = BlendFunction.Add,
                ColorBlendFunction = BlendFunction.Add,

                AlphaSourceBlend = Blend.InverseSourceAlpha,
                ColorSourceBlend = Blend.InverseSourceAlpha,

                AlphaDestinationBlend = Blend.InverseSourceAlpha,
                ColorDestinationBlend = Blend.InverseSourceAlpha
            };

            var rasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                DepthClipEnable = true
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
                BlendState.Opaque,
                DepthStencilState.None,
                rasterState);
        }
    }
}
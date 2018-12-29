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
            var blendStates = new BlendState
            {
                IndependentBlendEnable = true
            };

            blendStates[0].AlphaSourceBlend = Blend.One;
            blendStates[0].ColorSourceBlend = Blend.One;
            blendStates[0].AlphaDestinationBlend = Blend.One;
            blendStates[0].ColorDestinationBlend = Blend.One;

            blendStates[1].AlphaSourceBlend = Blend.Zero;
            blendStates[1].ColorSourceBlend = Blend.Zero;
            blendStates[1].AlphaDestinationBlend = Blend.InverseSourceColor;
            blendStates[1].ColorDestinationBlend = Blend.InverseSourceColor;



            return new DeviceState(
                device,
                blendStates,
                DepthStencilState.DepthRead,
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
        /// or perform or other comparisons on a single texture that contains depth information.        
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

        /// <summary>
        /// Graphics device state for rendering shadow maps
        /// </summary>
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

        /// <summary>
        /// Similar to the additive blending but substracts the colors 
        /// thus making objects that are very opaque darker (as if they let less light through)
        /// </summary>
        public static DeviceState AdditiveBlendOccluderState(this GraphicsDevice device)
        {
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

        /// <summary>
        /// Similar to the alpha blending state but first divides
        /// the source components by the inverse alpha, thus making
        /// objects that are very opaque darker (as if they let less light through)
        /// </summary>
        public static DeviceState AlphaBlendOccluderState(this GraphicsDevice device)
        {
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

        /// <summary>
        /// Graphics device state for 2D post processing
        /// </summary>
        public static DeviceState PostProcessState(this GraphicsDevice device)
        {
            return new DeviceState(
                device,
                BlendState.AlphaBlend,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);
        }

        /// <summary>
        /// Graphics device state for 2D post processing
        /// </summary>
        public static DeviceState FooState(this GraphicsDevice device)
        {
            var blendState = new BlendState
            {
                AlphaSourceBlend = Blend.SourceAlpha,
                ColorSourceBlend = Blend.SourceAlpha,
                AlphaDestinationBlend = Blend.InverseSourceAlpha,
                ColorDestinationBlend = Blend.InverseSourceAlpha
            };

            return new DeviceState(
                device,
                BlendState.Additive,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);
        }

        /// <summary>
        /// Graphics device state for rendering everything as wireframes
        /// </summary>
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
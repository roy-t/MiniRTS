using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects.DeviceStates
{
    public static class GraphicsDeviceExtensions
    {
        private static readonly BlendState WeighedParticleBlendState = CreateWeightedParticleBlendState();
        private static readonly BlendState AlphaBlendOccluderBlendState = CreateAlphaBlendOccluderBlendState();

        private static readonly SamplerState ShadowCastingLightSamplerState = CreateShadowCastingLightSamplerState();

        private static readonly RasterizerState ShadowMapRasterizerState = CreateShadowMapRasterizerState();
        private static readonly RasterizerState WireFrameRasterizerState = CreateWireFrameRasterizerState();


        public static bool Override = false;

        /// <summary>
        /// Graphics device state for drawing geometry to the G-Buffer
        /// </summary>
        public static void GeometryState(this GraphicsDevice device)
        {
            if (!Override)
            {

                SetDeviceState(
                    device,
                    BlendState.Opaque,
                    DepthStencilState.Default,
                    RasterizerState.CullCounterClockwise);
            }
            else
            {
                WireFrameState(device);
            }
        }

        /// <summary>
        /// Graphics device state for drawing particles to the G-Buffer
        /// </summary>
        public static void WeightedParticlesState(this GraphicsDevice device)
        {
            SetDeviceState(
                device,
                WeighedParticleBlendState,
                DepthStencilState.DepthRead,
                RasterizerState.CullNone);
        }

        /// <summary>
        /// Graphics device state for drawing lights to the Light Target
        /// </summary>
        public static void LightState(this GraphicsDevice device)
        {
            SetDeviceState(
                device,
                BlendState.AlphaBlend,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);
        }

        /// <summary>
        /// Graphics device state for drawing lights that cast shadows to the Light Target
        /// or perform or other comparisons on a single texture that contains depth information.        
        /// </summary>
        public static void ShadowCastingLightState(this GraphicsDevice device)
        {            
            SetDeviceState(
                device,
                BlendState.AlphaBlend,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                ShadowCastingLightSamplerState);
        }

        /// <summary>
        /// Graphics device state for rendering shadow maps
        /// </summary>
        public static void ShadowMapState(this GraphicsDevice device)
        {
            SetDeviceState(
                device,
                BlendState.Opaque,
                DepthStencilState.Default,
                ShadowMapRasterizerState);
        }

        /// <summary>
        /// Similar to the alpha blending state but first divides
        /// the source components by the inverse alpha, thus making
        /// objects that are very opaque darker (as if they let less light through)
        /// </summary>
        public static void AlphaBlendOccluderState(this GraphicsDevice device)
        {
            SetDeviceState(
                device,
                AlphaBlendOccluderBlendState,
                DepthStencilState.DepthRead,
                ShadowMapRasterizerState);
        }

        /// <summary>
        /// Graphics device state for 2D post processing
        /// </summary>
        public static void PostProcessState(this GraphicsDevice device)
        {
            SetDeviceState(
                device,
                BlendState.AlphaBlend,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);
        }

        /// <summary>
        /// Graphics device state for additive blending of particles
        /// </summary>
        public static void AdditiveParticleState(this GraphicsDevice device)
        {
            SetDeviceState(
                device,
                BlendState.Additive,
                DepthStencilState.None,
                RasterizerState.CullNone);
        }

        /// <summary>
        /// Graphics device state for rendering everything as wireframes
        /// </summary>
        public static void WireFrameState(this GraphicsDevice device)
        {

            SetDeviceState(
                device,
                BlendState.Opaque,
                DepthStencilState.None,
                WireFrameRasterizerState);
        }

        private static void SetDeviceState(GraphicsDevice device, BlendState blendState, DepthStencilState depthStencilState, RasterizerState rasterizerState)
            => SetDeviceState(device, blendState, depthStencilState, rasterizerState, SamplerState.LinearWrap);

        private static void SetDeviceState(
           GraphicsDevice device,
           BlendState blendState,
           DepthStencilState depthStencilState,
           RasterizerState rasterizerState,
           SamplerState samplerState)
        {
            device.BlendState = blendState;
            device.DepthStencilState = depthStencilState;
            device.RasterizerState = rasterizerState;
            device.SamplerStates[0] = samplerState;
        }

        private static BlendState CreateWeightedParticleBlendState()
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

            return blendStates;
        }

        private static SamplerState CreateShadowCastingLightSamplerState()
        {
            return new SamplerState
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                Filter = TextureFilter.Anisotropic,
                ComparisonFunction = CompareFunction.LessEqual,
                FilterMode = TextureFilterMode.Comparison
            };
        }

        private static RasterizerState CreateShadowMapRasterizerState()
        {
            return new RasterizerState
            {
                CullMode = CullMode.None,
                DepthClipEnable = false
            };
        }

        private static BlendState CreateAlphaBlendOccluderBlendState()
        {
            return new BlendState
            {
                AlphaBlendFunction = BlendFunction.Add,
                ColorBlendFunction = BlendFunction.Add,

                AlphaSourceBlend = Blend.InverseSourceAlpha,
                ColorSourceBlend = Blend.InverseSourceAlpha,

                AlphaDestinationBlend = Blend.InverseSourceAlpha,
                ColorDestinationBlend = Blend.InverseSourceAlpha
            };
        }

        private static RasterizerState CreateWireFrameRasterizerState()
        {
            return new RasterizerState
            {
                CullMode = CullMode.None,
                FillMode = FillMode.WireFrame
            };
        }
    }
}
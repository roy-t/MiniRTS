using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Particles;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Transparency
{
    [System]
    public partial class TransparencyPreprocessSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly Quad Quad;
        private readonly WeightedTransparencyEffect Effect;
        private readonly BlendState WeightedParticleBlendState;

        public TransparencyPreprocessSystem(GraphicsDevice device, FrameService frameService, Quad quad, WeightedTransparencyEffect effect)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.Quad = quad;
            this.Effect = effect;
            this.WeightedParticleBlendState = CreateWeightedParticleBlendState();
        }

        public void OnSet()
        {
            this.Device.BlendState = this.WeightedParticleBlendState;
            this.Device.DepthStencilState = DepthStencilState.DepthRead;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;

            this.Device.SamplerStates[0] = SamplerState.LinearClamp;

            // Set the albedo target only for the depth buffer
            this.Device.SetRenderTargets(this.FrameService.GBuffer.Albedo, this.FrameService.TBuffer.Albedo, this.FrameService.TBuffer.Weights);
        }


        [ProcessAll]
        public void Process(TransparentParticleFountainComponent fountain, TransformComponent transform)
        {
            var camera = this.FrameService.CameraComponent.Camera;
            fountain.Update(this.FrameService.Elapsed, transform.Matrix, camera);

            for(var i = 0; i < fountain.Emitters.Count; i++)
            {
                var emitter = fountain.Emitters[i];
                this.Effect.Texture = emitter.Texture;
                this.Effect.WorldViewProjection = camera.ViewProjection;

                this.Effect.Apply();
                this.Quad.RenderInstanced(this.Device, emitter.Particles);
            }            
        }

        private static BlendState CreateWeightedParticleBlendState()
        {
            var blendStates = new BlendState
            {
                IndependentBlendEnable = true
            };

            blendStates[1].AlphaSourceBlend = Blend.One;
            blendStates[1].ColorSourceBlend = Blend.One;
            blendStates[1].AlphaDestinationBlend = Blend.One;
            blendStates[1].ColorDestinationBlend = Blend.One;

            blendStates[2].AlphaSourceBlend = Blend.Zero;
            blendStates[2].ColorSourceBlend = Blend.Zero;
            blendStates[2].AlphaDestinationBlend = Blend.InverseSourceColor;
            blendStates[2].ColorDestinationBlend = Blend.InverseSourceColor;

            return blendStates;
        }
    }
}

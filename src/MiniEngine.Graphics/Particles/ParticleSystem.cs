using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Particles
{
    [System]
    public partial class ParticleSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly Quad Quad;
        private readonly ParticleEffect Effect;

        public ParticleSystem(GraphicsDevice device, FrameService frameService, Quad quad, ParticleEffect effect)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.Quad = quad;
            this.Effect = effect;
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.Default;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
            this.Device.SamplerStates[0] = SamplerState.AnisotropicWrap;
            this.Device.SamplerStates[1] = SamplerState.AnisotropicWrap;
            this.Device.SamplerStates[2] = SamplerState.AnisotropicWrap;
            this.Device.SamplerStates[3] = SamplerState.AnisotropicWrap;
            this.Device.SamplerStates[4] = SamplerState.AnisotropicWrap;

            this.Device.SetRenderTargets(
                this.FrameService.GBuffer.Albedo,
                this.FrameService.GBuffer.Material,
                this.FrameService.GBuffer.Depth,
                this.FrameService.GBuffer.Normal);
        }

        [ProcessAll]
        public void Process(ParticleFountainComponent fountain, TransformComponent transform)
        {
            fountain.Update(this.FrameService.Elapsed, transform.Matrix);

            var camera = this.FrameService.CameraComponent.Camera;
            for (var i = 0; i < fountain.Emitters.Count; i++)
            {
                var emitter = fountain.Emitters[i];

                this.Effect.WorldViewProjection = camera.ViewProjection;
                this.Effect.View = camera.View;
                this.Effect.Metalicness = emitter.Metalicness;
                this.Effect.Roughness = emitter.Roughness;

                this.Effect.Apply();
                //this.Quad.RenderInstanced(this.Device, emitter.Particles);

                DrawPoints(emitter);
            }
        }

        private void DrawPoints(ParticleEmitter emitter)
        {
            var vertex = new ParticleVertex(Vector3.Zero);

            var vertices = new VertexBuffer(this.Device, ParticleVertex.Declaration, 1, BufferUsage.WriteOnly);
            vertices.SetData(new[] { vertex });

            var indices = new IndexBuffer(this.Device, IndexElementSize.SixteenBits, 1, BufferUsage.WriteOnly);
            indices.SetData(new short[] { 0 });

            this.Device.SetVertexBuffers(new VertexBufferBinding(vertices), new VertexBufferBinding(emitter.Particles.Commit(), 0, 1));
            this.Device.Indices = indices;

            var device = (SharpDX.Direct3D11.Device)this.Device.Handle;
            //var context = device.ImmediateContext.QueryInterface<SharpDX.Direct3D11.DeviceContext>(); ;

            var field = this.Device.GetType().GetField("_d3dContext", BindingFlags.Instance | BindingFlags.NonPublic)!;
            var context = (SharpDX.Direct3D11.DeviceContext)field.GetValue(this.Device)!;

            lock (context)
            {
                var method = this.Device.GetType().GetMethod("ApplyState", BindingFlags.Instance | BindingFlags.NonPublic)!;
                method.Invoke(this.Device, new object[] { true });

                var count = emitter.Particles.Count;
                context.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.PointList;
                var indexCount = count;
                context.DrawIndexedInstanced(indexCount, emitter.Particles.Count, 0, 0, 0);
            }
        }
    }
}

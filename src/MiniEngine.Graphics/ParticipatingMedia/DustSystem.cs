using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Graphics.Shadows;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.ParticipatingMedia
{
    [System]
    public partial class DustSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly MediaEffect Effect;
        private readonly PostProcessTriangle PostProcessTriangle;
        private readonly RasterizerState RasterizerState;

        public DustSystem(GraphicsDevice device, MediaEffect effect, PostProcessTriangle postProcessTriangle)
        {
            this.Device = device;
            this.Effect = effect;
            this.PostProcessTriangle = postProcessTriangle;
            this.RasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                DepthClipEnable = false
            };
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Additive;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = this.RasterizerState;
            this.Device.SamplerStates[0] = SamplerState.LinearClamp;
        }

        [ProcessAll]
        public void Process(DustComponent media, TransformComponent transform, CameraComponent camera)
        {
            this.Device.SetRenderTarget(media.VolumeBuffer);
            this.Device.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

            this.Device.SetVertexBuffer(media.Geometry.VertexBuffer);
            this.Device.Indices = media.Geometry.IndexBuffer;

            this.Effect.WorldViewProjection = transform.Matrix * camera.Camera.ViewProjection;

            // Back sides
            this.Device.RasterizerState = this.RasterizerState;
            this.Effect.Channel = new Vector2(1, 0);
            this.Effect.Apply(MediaTechnique.Volume);
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, media.Geometry.Primitives);

            // Front sides
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
            this.Effect.Channel = new Vector2(0, 1);
            this.Effect.Apply();
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, media.Geometry.Primitives);

            // Combine
            this.Device.SetRenderTarget(media.DensityBuffer);
            this.Device.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);

            this.Effect.WorldViewProjection = Matrix.Identity;
            this.Effect.VolumeTexture = media.VolumeBuffer;
            this.Effect.Apply(MediaTechnique.Density);
            this.PostProcessTriangle.Render(this.Device);

            this.Device.SetRenderTarget(null);

        }
    }
}

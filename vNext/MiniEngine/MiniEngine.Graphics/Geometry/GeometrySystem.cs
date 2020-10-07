using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Effects;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Geometry
{
    [System]
    public sealed class GeometrySystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly GeometryEffect Effect;

        public GeometrySystem(GraphicsDevice device, EffectFactory effectFactory, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.Effect = effectFactory.Construct<GeometryEffect>();
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.Default;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;
            this.Device.SamplerStates[0] = SamplerState.AnisotropicClamp;
            this.Device.SamplerStates[1] = SamplerState.AnisotropicClamp;

            this.Device.SetRenderTargets(this.FrameService.RenderTargetSet.Diffuse, this.FrameService.RenderTargetSet.Depth, this.FrameService.RenderTargetSet.Normal);
        }

        public void Process(GeometryComponent geometry, TransformComponent transform)
        {
            this.Effect.World = transform.Matrix;
            this.Effect.WorldViewProjection = transform.Matrix * this.FrameService.Camera.ViewProjection;
            this.Effect.Diffuse = geometry.Diffuse;
            this.Effect.Normal = geometry.Normal;
            this.Effect.Apply();

            this.Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, geometry.Vertices, 0, geometry.Vertices.Length, geometry.Indices, 0, geometry.Primitives);
        }
    }
}

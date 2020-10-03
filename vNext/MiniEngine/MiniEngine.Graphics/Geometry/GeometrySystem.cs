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
        private readonly GeometryEffect Effect;

        public GeometrySystem(GraphicsDevice device, EffectFactory effectFactory)
        {
            this.Device = device;
            this.Effect = effectFactory.Construct<GeometryEffect>();
        }

        public void Process(GeometryComponent geometry, TransformComponent transform, FrameService frameService)
        {
            // TODO: this is overriden by ImGuiRenderer, every system should have a setup step so it can fix things like these once
            this.Device.DepthStencilState = DepthStencilState.Default;

            this.Effect.WorldViewProjection = transform.Matrix * frameService.Camera.ViewProjection;
            this.Effect.Diffuse = geometry.Diffuse;
            this.Effect.Apply();

            this.Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, geometry.Vertices, 0, geometry.Vertices.Length, geometry.Indices, 0, geometry.Indices.Length / 3);
        }
    }
}

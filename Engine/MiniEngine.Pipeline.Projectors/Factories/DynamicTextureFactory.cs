using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Projectors.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;
using MiniEngine.Telemetry;

namespace MiniEngine.Pipeline.Projectors.Factories
{
    public sealed class DynamicTextureFactory : AComponentFactory<DynamicTexture>
    {
        public DynamicTextureFactory(GraphicsDevice device, EntityLinker linker)
            : base(device, linker)
        {
        }

        public DynamicTexture Construct(Entity entity, Vector3 position, Vector3 lookAt, int width, int height, string label, PassType type = PassType.Opaque)
        {
            var pipeline = new RenderPipeline(this.Device, new NullMeterRegistry());

            var gBuffer = new GBuffer(this.Device, width, height);
            var viewPoint = new PerspectiveCamera(gBuffer.AspectRatio);
            viewPoint.Move(position, lookAt);
            var pass = new Pass(type, 0);

            var dynamicTexture = new DynamicTexture(entity, pipeline, viewPoint, gBuffer, pass, label);
            this.Linker.AddComponent(entity, dynamicTexture);

            return dynamicTexture;
        }
    }
}

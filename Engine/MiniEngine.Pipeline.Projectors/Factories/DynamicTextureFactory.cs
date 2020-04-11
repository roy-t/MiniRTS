using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Projectors.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;
using MiniEngine.Telemetry;

namespace MiniEngine.Pipeline.Projectors.Factories
{
    public sealed class DynamicTextureFactory : AComponentFactory<DynamicTexture>
    {
        public DynamicTextureFactory(GraphicsDevice device, IComponentContainer<DynamicTexture> container)
            : base(device, container) { }

        public DynamicTexture Construct(Entity entity, Vector3 position, Vector3 lookAt, int width, int height, TextureCube skybox, string label, PassType type = PassType.Opaque)
        {
            var pipeline = new RenderPipeline(this.Device, new NullMeterRegistry());

            var gBuffer = new GBuffer(this.Device, width, height);
            var viewPoint = new PerspectiveCamera(width, height);
            viewPoint.Move(position, lookAt);
            var pass = new Pass(type, 0);

            var dynamicTexture = new DynamicTexture(entity, pipeline, viewPoint, gBuffer, skybox, pass, label);

            this.Container.Add(entity, dynamicTexture);

            return dynamicTexture;
        }
    }
}

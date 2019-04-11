using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Projectors.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Projectors.Factories
{
    public sealed class ProjectorFactory : AComponentFactory<Projector>
    {
        public ProjectorFactory(GraphicsDevice device, EntityLinker linker) 
            : base(device, linker)
        {
        }

        public Projector Construct(Entity entity, Texture2D texture, Color tint, Meters maxDistance, Vector3 position, Vector3 lookAt)
        {
            var viewPoint = new PerspectiveCamera(new Viewport(0, 0, texture.Width, texture.Height));
            viewPoint.Move(position, lookAt);
            viewPoint.SetPlanes(1.0f, maxDistance);

            var projector = new Projector(texture, tint, maxDistance, viewPoint);
            this.Linker.AddComponent(entity, projector);

            return projector;
        }
    }
}

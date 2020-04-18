using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Projectors.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Projectors.Factories
{
    public sealed class ProjectorFactory : AComponentFactory<Projector>
    {
        public ProjectorFactory(GraphicsDevice device, IComponentContainer<Projector> container)
            : base(device, container) { }

        public Projector Construct(Entity entity, Texture2D texture, Color tint, Vector3 position, Vector3 lookAt)
        {
            var mask = new Texture2D(this.Device, 1, 1, false, SurfaceFormat.Color);
            mask.SetData(new Color[] { Color.White });

            return this.Construct(entity, texture, mask, tint, position, lookAt);
        }

        public Projector Construct(Entity entity, Texture2D texture, Texture2D mask, Color tint, Vector3 position, Vector3 lookAt)
        {
            var projector = new Projector(entity, texture, mask, tint, position, lookAt, 1.0f, 25.0f);

            this.Container.Add(projector);

            return projector;
        }
    }
}

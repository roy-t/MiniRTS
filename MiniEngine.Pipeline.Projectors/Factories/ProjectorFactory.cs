using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Projectors.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Projectors.Factories
{
    public sealed class ProjectorFactory : AComponentFactory<Projector>
    {
        public ProjectorFactory(GraphicsDevice device, EntityLinker linker) 
            : base(device, linker)
        {
        }

        public Projector Construct(Entity entity, Texture2D texture, Vector3 position, Vector3 lookAt)
        {
            var projector = new Projector(texture, position, lookAt);
            this.Linker.AddComponent(entity, projector);

            return projector;
        }
    }
}

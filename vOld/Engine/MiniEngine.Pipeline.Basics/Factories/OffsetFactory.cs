using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Basics.Factories
{
    public sealed class OffsetFactory : AComponentFactory<Offset>
    {
        public OffsetFactory(GraphicsDevice _, IComponentContainer<Offset> container)
            : base(_, container) { }

        public Offset Construct(Entity entity, Vector3 position, float yaw, float pitch, float roll, Entity target)
        {
            var offset = new Offset(entity, position, yaw, pitch, roll, target);
            this.Container.Add(offset);

            return offset;
        }
    }
}

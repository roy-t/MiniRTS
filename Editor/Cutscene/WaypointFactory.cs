using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;
using MiniEngine.Units;

namespace MiniEngine.CutScene
{
    public sealed class WaypointFactory : AComponentFactory<Waypoint>
    {
        public WaypointFactory(GraphicsDevice device, IComponentContainer<Waypoint> container)
            : base(device, container) { }

        public void Construct(Entity entity, MetersPerSecond speed, Vector3 position, Vector3 lookAt)
        {
            var waypoint = new Waypoint(entity, speed, position, lookAt);
            this.Container.Add(entity, waypoint);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;
using MiniEngine.Units;

namespace MiniEngine.CutScene
{
    public sealed class WaypointFactory : AComponentFactory<Waypoint>
    {
        public WaypointFactory(GraphicsDevice device, EntityLinker linker) 
            : base(device, linker) { }

        public void Construct(Entity entity, MetersPerSecond speed, Vector3 position)
        {
            var waypoint = new Waypoint(speed, position);
            this.Linker.AddComponent(entity, waypoint);
        }
    }
}

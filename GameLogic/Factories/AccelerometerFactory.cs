using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic.Vehicles.Fighter;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.GameLogic.Factories
{
    public sealed class AccelerometerFactory : AComponentFactory<Accelerometer>
    {
        public AccelerometerFactory(GraphicsDevice _, IComponentContainer<Accelerometer> container)
            : base(_, container) { }

        public Accelerometer Construct(Entity target)
        {
            var accelerometer = new Accelerometer(target);
            this.Container.Add(accelerometer);
            return accelerometer;
        }
    }
}

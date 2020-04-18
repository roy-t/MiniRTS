using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Basics.Factories
{
    public sealed class PoseFactory : AComponentFactory<Pose>
    {
        public PoseFactory(GraphicsDevice _, IComponentContainer<Pose> container)
            : base(_, container) { }

        public Pose Construct(Entity entity, Vector3 position, float scale = 1.0f, float yaw = 0.0f, float pitch = 0.0f, float roll = 0.0f)
            => this.Construct(entity, position, Vector3.One * scale, yaw, pitch, roll);

        public Pose Construct(Entity entity, Vector3 position, Vector3 scale, float yaw, float pitch, float roll)
        {
            var pose = new Pose(entity, position, scale, yaw, pitch, roll);
            this.Container.Add(pose);

            return pose;
        }
    }
}

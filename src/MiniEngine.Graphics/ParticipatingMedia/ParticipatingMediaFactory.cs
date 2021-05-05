using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Geometry.Generators;
using MiniEngine.Graphics.Physics;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Graphics.ParticipatingMedia
{
    [Service]
    public sealed class ParticipatingMediaFactory
    {
        private readonly GraphicsDevice Device;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;

        public ParticipatingMediaFactory(GraphicsDevice device, EntityAdministrator entities, ComponentAdministrator components)
        {
            this.Device = device;
            this.Entities = entities;
            this.Components = components;
        }

        public (ParticipatingMediaComponent, TransformComponent) Create(int resolutionWidth, int resolutionHeight)
        {
            var entity = this.Entities.Create();
            var cube = CubeGenerator.Generate(this.Device);

            var participatingMedia = ParticipatingMediaComponent.Create(entity, this.Device, cube, resolutionWidth, resolutionHeight, 1.0f, Color.White);
            var transform = new TransformComponent(entity);

            this.Components.Add(participatingMedia);
            this.Components.Add(transform);

            return (participatingMedia, transform);
        }
    }
}

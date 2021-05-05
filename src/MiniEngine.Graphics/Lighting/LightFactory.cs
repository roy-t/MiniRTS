using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Physics;
using MiniEngine.Graphics.Shadows;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Graphics.Lighting
{
    [Service]
    public sealed class LightFactory
    {
        private readonly GraphicsDevice Device;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;

        public LightFactory(GraphicsDevice device, EntityAdministrator entities, ComponentAdministrator components)
        {
            this.Device = device;
            this.Entities = entities;
            this.Components = components;
        }

        public (PointLightComponent pointLight, TransformComponent transform) CreatePointLight()
        {
            var entity = this.Entities.Create();

            var pointLight = new PointLightComponent(entity, Color.White, 100.0f);
            var transform = new TransformComponent(entity);

            this.Components.Add(pointLight, transform);

            return (pointLight, transform);
        }

        public (SpotLightComponent spotLight, ShadowMapComponent shadowMap, CameraComponent viewPoint) CreateSpotLight(int resolution)
        {
            var entity = this.Entities.Create();

            var spotLight = new SpotLightComponent(entity, Color.White, 100.0f);
            var shadowMap = ShadowMapComponent.Create(entity, this.Device, resolution);
            var camera = new CameraComponent(entity, new PerspectiveCamera(this.Device.Viewport.AspectRatio));

            this.Components.Add(spotLight, shadowMap, camera);

            return (spotLight, shadowMap, camera);
        }
    }
}

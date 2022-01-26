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
        private static readonly float[] DefaultCascadeDistances =
        {
            0.075f,
            0.15f,
            0.3f,
            1.0f
        };

        private readonly GraphicsDevice Device;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;

        public LightFactory(GraphicsDevice device, EntityAdministrator entities, ComponentAdministrator components)
        {
            this.Device = device;
            this.Entities = entities;
            this.Components = components;
        }

        public (PointLightComponent, TransformComponent) CreatePointLight()
        {
            var entity = this.Entities.Create();

            var pointLight = new PointLightComponent(entity, Color.White, 100.0f);
            var transform = new TransformComponent(entity);

            this.Components.Add(pointLight, transform);

            return (pointLight, transform);
        }

        public (SpotLightComponent, ShadowMapComponent, CameraComponent) CreateSpotLight(int resolution)
        {
            var entity = this.Entities.Create();

            var spotLight = new SpotLightComponent(entity, Color.White, 100.0f);
            var shadowMap = ShadowMapComponent.Create(entity, this.Device, resolution);
            var viewPoint = new CameraComponent(entity, new PerspectiveCamera(this.Device.Viewport.AspectRatio));

            this.Components.Add(spotLight, shadowMap, viewPoint);

            return (spotLight, shadowMap, viewPoint);
        }

        public (SunlightComponent, CascadedShadowMapComponent, CameraComponent) CreateSunLight(int resolution)
        {
            var entity = this.Entities.Create();

            var sunlight = new SunlightComponent(entity, Color.White, 1.0f);
            var shadowMap = CascadedShadowMapComponent.Create(entity, this.Device, resolution, DefaultCascadeDistances);
            var viewPoint = new CameraComponent(entity, new PerspectiveCamera(1.0f));

            this.Components.Add(sunlight, shadowMap, viewPoint);

            return (sunlight, shadowMap, viewPoint);
        }
    }
}

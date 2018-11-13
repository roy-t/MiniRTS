using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Effects;
using MiniEngine.Primitives;
using MiniEngine.Systems;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Pipeline.Shadows.Systems;

namespace MiniEngine.Pipeline.Lights.Systems
{
    public sealed class ShadowCastingLightSystem : ISystem
    {
        private readonly GraphicsDevice Device;

        private readonly FullScreenTriangle FullScreenTriangle;

        private readonly Dictionary<Entity, ShadowCastingLight> Lights;
        private readonly ShadowCastingLightEffect Effect;

        private readonly ShadowMapSystem ShadowMapSystem;

        public ShadowCastingLightSystem(
            GraphicsDevice device,
            ShadowCastingLightEffect effect,
            ShadowMapSystem shadowMapSystem)
        {
            this.Device = device;
            this.Effect = effect;
            this.ShadowMapSystem = shadowMapSystem;

            this.FullScreenTriangle = new FullScreenTriangle();

            this.Lights = new Dictionary<Entity, ShadowCastingLight>();
        }

        public bool Contains(Entity entity) => this.Lights.ContainsKey(entity);

        public string Describe(Entity entity)
        {
            var light = this.Lights[entity];
            return
                $"shadow casting light, direction: {light.ViewPoint.LookAt - light.ViewPoint.Position}, color: {light.Color}";
        }

        public void Remove(Entity entity)
        {
            this.Lights.Remove(entity);
            this.ShadowMapSystem.Remove(entity);
        }

        public void Add(Entity entity, Vector3 position, Vector3 lookAt, Color color)
        {
            var shadowCastingLight = new ShadowCastingLight(position, lookAt, color);

            this.Lights.Add(entity, shadowCastingLight);
            this.ShadowMapSystem.Add(entity, new Reference<IViewPoint>(shadowCastingLight.ViewPoint));
        }

        public void RenderLights(PerspectiveCamera perspectiveCamera, GBuffer gBuffer)
        {
            using (this.Device.ShadowCastingLightState())
            {
                foreach (var lightEntity in this.Lights)
                {
                    var light = lightEntity.Value;
                    var shadowMap = this.ShadowMapSystem.Get(lightEntity.Key);

                    // G-Buffer input                    
                    this.Effect.NormalMap = gBuffer.NormalTarget;
                    this.Effect.DepthMap = gBuffer.DepthTarget;

                    // Light properties                    
                    this.Effect.LightDirection = light.ViewPoint.Forward;
                    this.Effect.LightPosition = light.ViewPoint.Position;
                    this.Effect.Color = light.Color;

                    // Camera properties for specular reflections
                    this.Effect.CameraPosition = perspectiveCamera.Position;
                    this.Effect.InverseViewProjection = perspectiveCamera.InverseViewProjection;

                    // Shadow properties
                    this.Effect.ShadowMap = shadowMap.DepthMap;
                    this.Effect.ColorMap = shadowMap.ColorMap;
                    this.Effect.LightViewProjection = light.ViewPoint.ViewProjection;

                    this.Effect.Apply();

                    this.FullScreenTriangle.Render(this.Device);
                }
            }
        }
    }
}
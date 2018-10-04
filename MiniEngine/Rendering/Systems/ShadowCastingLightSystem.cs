using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Components;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Systems;

namespace MiniEngine.Rendering.Systems
{
    public sealed class ShadowCastingLightSystem : ISystem
    {
        private readonly GraphicsDevice Device;

        private readonly FullScreenTriangle FullScreenTriangle;

        private readonly Dictionary<Entity, ShadowCastingLight> Lights;
        private readonly Effect ShadowCastingLightEffect;

        private readonly ShadowMapSystem ShadowMapSystem;

        public ShadowCastingLightSystem(
            GraphicsDevice device,
            Effect shadowCastingLightEffect,
            ShadowMapSystem shadowMapSystem)
        {
            this.Device = device;
            this.ShadowCastingLightEffect = shadowCastingLightEffect;
            this.ShadowMapSystem = shadowMapSystem;

            this.FullScreenTriangle = new FullScreenTriangle();

            this.Lights = new Dictionary<Entity, ShadowCastingLight>();
        }

        public bool Contains(Entity entity)
        {
            return this.Lights.ContainsKey(entity);
        }

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
            this.ShadowMapSystem.Add(entity, shadowCastingLight.ViewPoint);
        }

        public void RenderLights(PerspectiveCamera perspectiveCamera, GBuffer gBuffer)
        {
            using (this.Device.LightState())
            {
                foreach (var lightEntity in this.Lights)
                {
                    var light = lightEntity.Value;
                    var shadowMap = this.ShadowMapSystem.Get(lightEntity.Key);

                    // G-Buffer input                        
                    this.ShadowCastingLightEffect.Parameters["NormalMap"].SetValue(gBuffer.NormalTarget);
                    this.ShadowCastingLightEffect.Parameters["DepthMap"].SetValue(gBuffer.DepthTarget);

                    // Light properties
                    this.ShadowCastingLightEffect.Parameters["LightDirection"].SetValue(
                        Vector3.Normalize(light.ViewPoint.LookAt - light.ViewPoint.Position));
                    this.ShadowCastingLightEffect.Parameters["LightPosition"].SetValue(light.ViewPoint.Position);
                    this.ShadowCastingLightEffect.Parameters["Color"].SetValue(light.ColorVector);

                    // Camera properties for specular reflections
                    this.ShadowCastingLightEffect.Parameters["CameraPosition"].SetValue(perspectiveCamera.Position);
                    this.ShadowCastingLightEffect.Parameters["InverseViewProjection"]
                        .SetValue(perspectiveCamera.InverseViewProjection);

                    // Shadow properties
                    this.ShadowCastingLightEffect.Parameters["ShadowMap"].SetValue(shadowMap.DepthMap);
                    this.ShadowCastingLightEffect.Parameters["ColorMap"].SetValue(shadowMap.ColorMap);
                    this.ShadowCastingLightEffect.Parameters["LightViewProjection"]
                        .SetValue(light.ViewPoint.ViewProjection);

                    foreach (var pass in this.ShadowCastingLightEffect.Techniques[0].Passes)
                    {
                        pass.Apply();
                        this.FullScreenTriangle.Render(this.Device);
                    }
                }
            }
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Components;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Systems;
using System.Collections.Generic;

namespace MiniEngine.Rendering.Systems
{
    public sealed class ShadowCastingLightSystem : ISystem
    {
        private readonly GraphicsDevice Device;        
        private readonly Effect ShadowCastingLightEffect;

        private readonly ShadowMapSystem ShadowMapSystem;

        private readonly Quad Quad;

        private readonly Dictionary<Entity, ShadowCastingLight> Lights;

        public ShadowCastingLightSystem(GraphicsDevice device, Effect shadowCastingLightEffect, ShadowMapSystem shadowMapSystem)
        {
            this.Device = device;
            this.ShadowCastingLightEffect = shadowCastingLightEffect;
            this.ShadowMapSystem = shadowMapSystem;

            this.Quad = new Quad();

            this.Lights = new Dictionary<Entity, ShadowCastingLight>();
        }

        public void Add(Entity entity, Vector3 position, Vector3 lookAt, Color color)
        {
            var shadowCastingLight = new ShadowCastingLight(position, lookAt, color);

            this.Lights.Add(entity, shadowCastingLight);
            this.ShadowMapSystem.Add(entity, shadowCastingLight);
        }

        public bool Contains(Entity entity) => this.Lights.ContainsKey(entity);

        public string Describe(Entity entity)
        {
            var light = this.Lights[entity];            
            return $"shadow casting light, direction: {light.LookAt - light.Position}, color: {light.Color}";
        }

        public void Remove(Entity entity)
        {
            this.Lights.Remove(entity);
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
                    this.ShadowCastingLightEffect.Parameters["LightDirection"].SetValue(Vector3.Normalize(light.LookAt - light.Position));
                    this.ShadowCastingLightEffect.Parameters["LightPosition"].SetValue(light.Position);
                    this.ShadowCastingLightEffect.Parameters["Color"].SetValue(light.ColorVector);

                    // Camera properties for specular reflections
                    this.ShadowCastingLightEffect.Parameters["CameraPosition"].SetValue(perspectiveCamera.Position);
                    this.ShadowCastingLightEffect.Parameters["InverseViewProjection"].SetValue(perspectiveCamera.InverseViewProjection);

                    // Shadow properties
                    this.ShadowCastingLightEffect.Parameters["ShadowMap"].SetValue(shadowMap.DepthMap);
                    this.ShadowCastingLightEffect.Parameters["LightViewProjection"].SetValue(light.ViewProjection);                    

                    foreach (var pass in this.ShadowCastingLightEffect.Techniques[0].Passes)
                    {                      
                        pass.Apply();
                        this.Quad.Render(this.Device);
                    }
                }                
            }
        }
    }
}

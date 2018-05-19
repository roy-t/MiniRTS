using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using DirectionalLight = MiniEngine.Rendering.Components.DirectionalLight;

namespace MiniEngine.Rendering.Systems
{
    public sealed class DirectionalLightSystem
    {
        private readonly GraphicsDevice Device;
        private readonly Effect Effect;        
        private readonly Quad Quad;

        private readonly Dictionary<Entity, DirectionalLight> Lights;

        public DirectionalLightSystem(GraphicsDevice device, Effect effect)
        {
            this.Device = device;
            this.Effect = effect;            
            this.Quad = new Quad();

            this.Lights = new Dictionary<Entity, DirectionalLight>();
        }

        public void Add(Entity entity, Vector3 direction, Color color)
        {
            this.Lights.Add(entity, new DirectionalLight(direction, color));
        }

        public void Remove(Entity entity)
        {
            this.Lights.Remove(entity);
        }

        public void Render(
            PerspectiveCamera perspectiveCamera,
            RenderTarget2D color,
            RenderTarget2D normal,
            RenderTarget2D depth)
        {            
            using (this.Device.LightState())
            {
                foreach (var light in this.Lights.Values)
                {
                    // G-Buffer input                        
                    this.Effect.Parameters["NormalMap"].SetValue(normal);
                    this.Effect.Parameters["DepthMap"].SetValue(depth);

                    // Light properties
                    this.Effect.Parameters["LightDirection"].SetValue(light.Direction);
                    this.Effect.Parameters["Color"].SetValue(light.ColorVector);

                    // Camera properties for specular reflections
                    this.Effect.Parameters["CameraPosition"].SetValue(perspectiveCamera.Position);
                    this.Effect.Parameters["InverseViewProjection"].SetValue(perspectiveCamera.InverseViewProjection);

                    foreach (var pass in this.Effect.Techniques[0].Passes)
                    {                       
                        pass.Apply();
                        this.Quad.Render(this.Device);
                    }
                }
            }
        }        
    }
}

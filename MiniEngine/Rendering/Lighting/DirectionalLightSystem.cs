using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering.Lighting
{
    public sealed class DirectionalLightSystem
    {
        private readonly GraphicsDevice Device;
        private readonly Effect Effect;        
        private readonly Quad Quad;

        public DirectionalLightSystem(GraphicsDevice device, Effect effect)
        {
            this.Device = device;
            this.Effect = effect;            
            this.Quad = new Quad();
        }

        public void Render(
            IEnumerable<DirectionalLight> lights,
            Camera camera,
            RenderTarget2D color,
            RenderTarget2D normal,
            RenderTarget2D depth)
        {            
            using (this.Device.LightState())
            {
                foreach (var light in lights)
                {
                    foreach (var pass in this.Effect.Techniques[0].Passes)
                    {
                        // G-Buffer input                        
                        this.Effect.Parameters["NormalMap"].SetValue(normal);
                        this.Effect.Parameters["DepthMap"].SetValue(depth);

                        // Light properties
                        this.Effect.Parameters["LightDirection"].SetValue(light.Direction);
                        this.Effect.Parameters["Color"].SetValue(light.ColorVector);

                        // Camera properties for specular reflections
                        this.Effect.Parameters["CameraPosition"].SetValue(camera.Position);
                        this.Effect.Parameters["InverseViewProjection"].SetValue(camera.InverseViewProjection);

                        pass.Apply();
                        this.Quad.Render(this.Device);
                    }
                }
            }
        }
    }
}

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

        public void Render(IEnumerable<DirectionalLight> lights, Camera camera, RenderTarget2D color, RenderTarget2D normal, RenderTarget2D depth, Vector2 halfPixel)
        {
            var invertViewProjection = Matrix.Invert(camera.View * camera.Projection);

            using (this.Device.GeometryState())
            {
                foreach (var light in lights)
                {
                    foreach (var pass in this.Effect.Techniques[0].Passes)
                    {
                        this.Effect.Parameters["ColorMap"].SetValue(color);
                        this.Effect.Parameters["NormalMap"].SetValue(normal);
                        this.Effect.Parameters["DepthMap"].SetValue(depth);
                        this.Effect.Parameters["LightDirection"].SetValue(light.Direction);
                        this.Effect.Parameters["Color"].SetValue(light.ColorVector);
                        this.Effect.Parameters["CameraPosition"].SetValue(camera.Position);
                        this.Effect.Parameters["InvertViewProjection"].SetValue(invertViewProjection);
                        this.Effect.Parameters["HalfPixel"].SetValue(halfPixel);

                        pass.Apply();
                        this.Quad.Render(this.Device);
                    }                    
                }
            }
        }        
    }
}

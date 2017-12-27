using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering.Lighting
{
    public sealed class PointLightSystem
    {
        private readonly GraphicsDevice Device;
        private readonly Effect Effect;
        private readonly Model Sphere;

        public PointLightSystem(GraphicsDevice device, Effect effect, Model sphere)
        {
            this.Device = device;
            this.Effect = effect;
            this.Sphere = sphere;
        }

        public void Render(
            IEnumerable<PointLight> lights,
            Camera camera,
            RenderTarget2D color,
            RenderTarget2D normal,
            RenderTarget2D depth,
            Vector2 halfPixel)
        {
            var invertViewProjection = Matrix.Invert(camera.View * camera.Projection);

            using (this.Device.LightState())
            {
                foreach (var light in lights)
                {
                    foreach (var pass in this.Effect.Techniques[0].Passes)
                    {
                        // G-Buffer input
                        this.Effect.Parameters["ColorMap"].SetValue(color);
                        this.Effect.Parameters["NormalMap"].SetValue(normal);
                        this.Effect.Parameters["DepthMap"].SetValue(depth);

                        // Light properties
                        var sphereWorldMatrix = Matrix.CreateScale(light.Radius) * Matrix.CreateTranslation(light.Position);

                        this.Effect.Parameters["World"].SetValue(sphereWorldMatrix);
                        this.Effect.Parameters["LightPosition"].SetValue(light.Position);
                        this.Effect.Parameters["Color"].SetValue(light.ColorVector);                        
                        this.Effect.Parameters["Radius"].SetValue(light.Radius);
                        this.Effect.Parameters["Intensity"].SetValue(light.Intensity);

                        // Camera properties for specular reflections
                        this.Effect.Parameters["View"].SetValue(camera.View);
                        this.Effect.Parameters["Projection"].SetValue(camera.Projection);
                        this.Effect.Parameters["InvertViewProjection"].SetValue(invertViewProjection);
                        this.Effect.Parameters["CameraPosition"].SetValue(camera.Position);

                        // Alignment
                        this.Effect.Parameters["HalfPixel"].SetValue(halfPixel);

                        // If the camera is inside the light's radius we invert the cull direction
                        // otherwise the camera's sphere model is clipped
                        var inside = Vector3.Distance(camera.Position, light.Position) < light.Radius;
                        this.Device.RasterizerState = inside 
                            ? RasterizerState.CullClockwise 
                            : RasterizerState.CullCounterClockwise;
                     
                        pass.Apply();

                        foreach (var mesh in this.Sphere.Meshes)
                        {
                            foreach (var meshPart in mesh.MeshParts)
                            {
                                this.Device.Indices = meshPart.IndexBuffer;
                                this.Device.SetVertexBuffer(meshPart.VertexBuffer);

                                this.Device.DrawIndexedPrimitives(
                                    PrimitiveType.TriangleList,
                                    0,
                                    meshPart.StartIndex,
                                    meshPart.NumVertices);
                            }
                        }
                    }
                }
            }
        }
    }
}

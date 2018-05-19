using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Components;

namespace MiniEngine.Rendering.Systems
{
    public sealed class PointLightSystem
    {
        private readonly GraphicsDevice Device;
        private readonly Effect Effect;
        private readonly Model Sphere;

        private readonly Dictionary<Entity, PointLight> Lights;

        public PointLightSystem(GraphicsDevice device, Effect effect, Model sphere)
        {
            this.Device = device;
            this.Effect = effect;
            this.Sphere = sphere;

            this.Lights = new Dictionary<Entity, PointLight>();
        }

        public void Add(Entity entity, Vector3 position, Color color, float radius, float intensity)
        {
            this.Lights.Add(entity, new PointLight(position, color, radius, intensity));
        }

        public void Remove(Entity entity)
        {
            this.Lights.Remove(entity);
        }

        public void Render(PerspectiveCamera perspectiveCamera, RenderTarget2D color, RenderTarget2D normal, RenderTarget2D depth)
        {            
            using (this.Device.LightState())
            {
                foreach (var light in this.Lights.Values)
                {
                    // G-Buffer input                        
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
                    this.Effect.Parameters["View"].SetValue(perspectiveCamera.View);
                    this.Effect.Parameters["Projection"].SetValue(perspectiveCamera.Projection);
                    this.Effect.Parameters["InverseViewProjection"].SetValue(perspectiveCamera.InverseViewProjection);
                    this.Effect.Parameters["CameraPosition"].SetValue(perspectiveCamera.Position);

                    // If the camera is inside the light's radius we invert the cull direction
                    // otherwise the camera's sphere model is clipped
                    var inside = Vector3.Distance(perspectiveCamera.Position, light.Position) < light.Radius;
                    this.Device.RasterizerState = inside
                        ? RasterizerState.CullClockwise
                        : RasterizerState.CullCounterClockwise;

                    foreach (var pass in this.Effect.Techniques[0].Passes)
                    {                                          
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
                                    meshPart.PrimitiveCount);
                            }
                        }
                    }
                }
            }
        }
    }
}

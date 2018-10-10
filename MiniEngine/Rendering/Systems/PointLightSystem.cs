using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Components;
using MiniEngine.Rendering.Effects;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Systems;

namespace MiniEngine.Rendering.Systems
{
    public sealed class PointLightSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly PointLightEffect Effect;

        private readonly Dictionary<Entity, PointLight> Lights;
        private readonly Model Sphere;

        public PointLightSystem(GraphicsDevice device, PointLightEffect effect, Model sphere)
        {
            this.Device = device;
            this.Effect = effect;
            this.Sphere = sphere;

            this.Lights = new Dictionary<Entity, PointLight>();
        }

        public bool Contains(Entity entity)
        {
            return this.Lights.ContainsKey(entity);
        }

        public string Describe(Entity entity)
        {
            var light = this.Lights[entity];
            return $"point light, position: {light.Position}, color: {light.Color}";
        }

        public void Remove(Entity entity)
        {
            this.Lights.Remove(entity);
        }

        public void Add(Entity entity, Vector3 position, Color color, float radius, float intensity)
        {
            this.Lights.Add(entity, new PointLight(position, color, radius, intensity));
        }

        public void Render(PerspectiveCamera perspectiveCamera, GBuffer gBuffer)
        {
            using (this.Device.LightState())
            {
                foreach (var light in this.Lights.Values)
                {
                    // G-Buffer input                        
                    this.Effect.NormalMap = gBuffer.NormalTarget;
                    this.Effect.DepthMap = gBuffer.DepthTarget;

                    // Light properties
                    var sphereWorldMatrix = Matrix.CreateScale(light.Radius) * Matrix.CreateTranslation(light.Position);
                    this.Effect.World = sphereWorldMatrix;
                    this.Effect.LightPosition = light.Position;
                    this.Effect.Color = light.Color;
                    this.Effect.Radius = light.Radius;
                    this.Effect.Intensity = light.Intensity;

                    // Camera properties for specular reflections
                    this.Effect.View = perspectiveCamera.View;
                    this.Effect.Projection = perspectiveCamera.Projection;
                    this.Effect.InverseViewProjection = perspectiveCamera.InverseViewProjection;
                    this.Effect.CameraPosition = perspectiveCamera.Position;

                    // If the camera is inside the light's radius we invert the cull direction
                    // otherwise the camera's sphere model is clipped
                    var inside = Vector3.Distance(perspectiveCamera.Position, light.Position) < light.Radius;
                    this.Device.RasterizerState = inside
                        ? RasterizerState.CullClockwise
                        : RasterizerState.CullCounterClockwise;

                    this.Effect.Apply();

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
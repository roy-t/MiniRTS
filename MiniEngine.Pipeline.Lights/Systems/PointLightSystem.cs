using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Effects;
using MiniEngine.Primitives;
using MiniEngine.Systems;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Pipeline.Lights.Components;

namespace MiniEngine.Pipeline.Lights.Systems
{
    public sealed class PointLightSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly PointLightEffect Effect;

        private readonly List<PointLight> Lights;
        private readonly Model Sphere;
        private readonly EntityLinker EntityLinker;

        public PointLightSystem(GraphicsDevice device, PointLightEffect effect, Model sphere, EntityLinker entityLinker)
        {
            this.Device = device;
            this.Effect = effect;
            this.Sphere = sphere;
            this.EntityLinker = entityLinker;
            this.Lights = new List<PointLight>();
        }

        public void Render(PerspectiveCamera perspectiveCamera, GBuffer gBuffer)
        {
            this.Lights.Clear();
            this.EntityLinker.GetComponents(this.Lights);

            using (this.Device.LightState())
            {
                foreach (var light in this.Lights)
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
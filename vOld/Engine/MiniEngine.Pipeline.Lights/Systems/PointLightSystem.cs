using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Effects.Wrappers;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Pipeline.Lights.Utilities;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Lights.Systems
{
    public sealed class PointLightSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly PointLightEffect Effect;

        private readonly IComponentContainer<PointLight> Lights;
        private readonly IComponentContainer<Pose> Poses;
        private readonly Model Sphere;

        public PointLightSystem(GraphicsDevice device, EffectFactory effectFactory, LightPrimitiveLoader lightPrimitiveLoader, IComponentContainer<PointLight> lights, IComponentContainer<Pose> poses)
        {
            this.Device = device;
            this.Effect = effectFactory.Construct<PointLightEffect>();
            this.Sphere = lightPrimitiveLoader.UnitSphere();
            this.Lights = lights;
            this.Poses = poses;
        }

        public void Render(PerspectiveCamera perspectiveCamera, GBuffer gBuffer)
        {
            this.Device.LightState();

            for (var iLight = 0; iLight < this.Lights.Count; iLight++)
            {
                var light = this.Lights[iLight];
                var pose = this.Poses.Get(light.Entity);

                // G-Buffer input                        
                this.Effect.NormalMap = gBuffer.NormalTarget;
                this.Effect.DepthMap = gBuffer.DepthTarget;

                // Light properties
                var sphereWorldMatrix = Matrix.CreateScale(light.Radius) * Matrix.CreateTranslation(pose.Position);
                this.Effect.World = sphereWorldMatrix;
                this.Effect.LightPosition = pose.Position;
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
                var inside = Vector3.Distance(perspectiveCamera.Position, pose.Position) < light.Radius;
                this.Device.RasterizerState = inside
                    ? RasterizerState.CullClockwise
                    : RasterizerState.CullCounterClockwise;

                this.Effect.Apply();

                for (var iMesh = 0; iMesh < this.Sphere.Meshes.Count; iMesh++)
                {
                    var mesh = this.Sphere.Meshes[iMesh];
                    for (var iPart = 0; iPart < mesh.MeshParts.Count; iPart++)
                    {
                        var meshPart = mesh.MeshParts[iPart];

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
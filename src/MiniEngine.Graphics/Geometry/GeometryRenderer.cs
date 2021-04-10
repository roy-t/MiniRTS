using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Visibility;
using MiniEngine.Systems.Components;

namespace MiniEngine.Graphics.Geometry
{
    [Service]
    public sealed class GeometryRenderer
    {
        public const int MaxInstances = 1024;
        private readonly GraphicsDevice Device;
        private readonly IComponentContainer<InstancingComponent> Instances;
        private readonly VertexBuffer InstanceBuffer;

        public GeometryRenderer(GraphicsDevice device, IComponentContainer<InstancingComponent> instances)
        {
            this.Device = device;
            this.Instances = instances;
            this.InstanceBuffer = new VertexBuffer(device, InstancingVertex.Declaration, MaxInstances, BufferUsage.WriteOnly);
        }

        public void Draw<T>(IReadOnlyList<Pose> inView, IGeometryRendererUser<T> user, T parameter)
        {
            for (var i = 0; i < inView.Count; i++)
            {
                var pose = inView[i];
                for (var j = 0; j < pose.Model.Meshes.Count; j++)
                {
                    var mesh = pose.Model.Meshes[j];
                    user.SetEffectParameters(mesh.Material, mesh.Offset * pose.Transform, parameter);
                    this.Draw(user, pose, mesh);
                }
            }
        }

        public void Draw(IReadOnlyList<Pose> inView, IGeometryRendererUser user)
        {
            for (var i = 0; i < inView.Count; i++)
            {
                var pose = inView[i];
                for (var j = 0; j < pose.Model.Meshes.Count; j++)
                {
                    var mesh = pose.Model.Meshes[j];
                    user.SetEffectParameters(mesh.Material, mesh.Offset * pose.Transform);
                    this.Draw(user, pose, mesh);
                }
            }
        }

        private void Draw(IGeometryRendererUserBase user, Pose pose, GeometryMesh mesh)
        {
            if (this.Instances.Contains(pose.Entity))
            {
                var instances = this.Instances.Get(pose.Entity);
                this.DrawInstanced(user, mesh.Geometry, instances);
            }
            else
            {
                this.Draw(user, mesh.Geometry);
            }
        }

        private void DrawInstanced(IGeometryRendererUserBase user, GeometryData geometry, InstancingComponent instances)
        {
            this.InstanceBuffer.SetData(instances.VertexData);

            this.Device.SetVertexBuffers(new VertexBufferBinding(geometry.VertexBuffer), new VertexBufferBinding(this.InstanceBuffer, 0, 1));
            this.Device.Indices = geometry.IndexBuffer;

            user.ApplyInstancedEffect();
            this.Device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.Primitives, instances.Instances);
        }

        private void Draw(IGeometryRendererUserBase user, GeometryData geometry)
        {
            this.Device.SetVertexBuffer(geometry.VertexBuffer);
            this.Device.Indices = geometry.IndexBuffer;

            user.ApplyEffect();
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.Primitives);
        }
    }

    public interface IGeometryRendererUserBase
    {
        void ApplyEffect();
        void ApplyInstancedEffect();
    }

    public interface IGeometryRendererUser : IGeometryRendererUserBase
    {
        void SetEffectParameters(Material material, Matrix transform);
    }

    public interface IGeometryRendererUser<T> : IGeometryRendererUserBase
    {
        void SetEffectParameters(Material material, Matrix transform, T parameter);
    }
}

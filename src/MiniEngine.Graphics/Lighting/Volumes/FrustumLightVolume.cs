using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;

namespace MiniEngine.Graphics.Lighting.Volumes
{
    [Service]
    public sealed class FrustumLightVolume : IDisposable
    {
        private const int TriangleCount = 12;

        private readonly IndexBuffer Indices;
        private readonly VertexBuffer Vertices;

        public FrustumLightVolume(GraphicsDevice device)
        {
            const float left = -1.0f;
            const float right = 1.0f;
            const float bottom = -1.0f;
            const float top = 1.0f;
            const float near = 0.0f;
            const float far = 1.0f;

            var vertices = new LightVolumeVertex[]
            {
                new LightVolumeVertex(new Vector3(left, top, near)),
                new LightVolumeVertex(new Vector3(right, top, near)),
                new LightVolumeVertex(new Vector3(right, bottom, near)),
                new LightVolumeVertex(new Vector3(left, bottom, near)),
                new LightVolumeVertex(new Vector3(left, top, far)),
                new LightVolumeVertex(new Vector3(right, top, far)),
                new LightVolumeVertex(new Vector3(right, bottom, far)),
                new LightVolumeVertex(new Vector3(left, bottom, far))
            };

            const int nearTopLeft = 0;
            const int nearTopRight = 1;
            const int nearBottomRight = 2;
            const int nearBottomLeft = 3;
            const int farTopLeft = 4;
            const int farTopRight = 5;
            const int farBottomRight = 6;
            const int farBottomLeft = 7;

            var indices = new short[]
            {
                // Far plane
                farBottomLeft, farTopLeft, farTopRight,
                farTopRight, farBottomRight, farBottomLeft,

                // Near plane
                nearBottomRight, nearTopRight, nearTopLeft,
                nearTopLeft, nearBottomLeft, nearBottomRight,

                // Left plane
                nearBottomLeft, nearTopLeft, farTopLeft,
                farTopLeft, farBottomLeft, nearBottomLeft,

                // Right plane
                farBottomRight, farTopRight, nearTopRight,
                nearTopRight, nearBottomRight, farBottomRight,

                // Top plane
                nearTopRight, farTopRight, farTopLeft,
                farTopLeft, nearTopLeft, nearTopRight,

                // Bottom plane
                nearBottomLeft, farBottomLeft, farBottomRight,
                farBottomRight, nearBottomRight, nearBottomLeft
            };

            this.Vertices = new VertexBuffer(device, LightVolumeVertex.Declaration, vertices.Length, BufferUsage.None);
            this.Vertices.SetData(vertices);

            this.Indices = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
            this.Indices.SetData(indices);
        }

        public void Render(GraphicsDevice device)
        {
            device.SetVertexBuffer(this.Vertices);
            device.Indices = this.Indices;
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, TriangleCount);
        }

        public void Dispose()
        {
            this.Vertices.Dispose();
            this.Indices.Dispose();
        }
    }
}

﻿using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Geometry.Generators;

namespace MiniEngine.Graphics.Lighting.Volumes
{
    [Service]
    public sealed class SphereLightVolume
    {
        private readonly GeometryData Sphere;

        public SphereLightVolume(GraphicsDevice device)
        {
            this.Sphere = SphereGenerator.Generate(device, 3);
        }

        public void Render(GraphicsDevice device)
        {
            device.SetVertexBuffer(this.Sphere.VertexBuffer);
            device.Indices = this.Sphere.IndexBuffer;
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.Sphere.Primitives);
        }
    }
}

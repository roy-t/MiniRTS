﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering.Primitives
{
    public sealed class Quad
    {
        private readonly GraphicsDevice Device;
        private readonly GBufferVertex[] Vertices;
        private readonly short[] Indices;

        public Quad(GraphicsDevice device)
        {
            this.Device = device;
            this.Vertices = new[]
            {
                new GBufferVertex(
                    new Vector3(1, -1, 0),
                    new Vector2(1, 1)),
                new GBufferVertex(
                    new Vector3(-1, -1, 0),
                    new Vector2(0, 1)),
                new GBufferVertex(
                    new Vector3(-1, 1, 0),
                    new Vector2(0, 0)),
                new GBufferVertex(
                    new Vector3(1, 1, 0),
                    new Vector2(1, 0))
            };

            this.Indices = new short[] { 0, 1, 2, 2, 3, 0 };
        }
               
        public void Render()
        {            
            this.Device.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList,
                this.Vertices,
                0,
                4,
                this.Indices,
                0,
                2);
        }
    }
}
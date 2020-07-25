using System;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.VertexTypes;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Models.Components
{
    public sealed class Geometry : IComponent
    {
        public Geometry(Entity entity,
            GBufferVertex[] vertices,
            short[] indices,
            Texture2D diffuseMap,
            Texture2D specularMap,
            Texture2D normalMap,
            Texture2D reflectionMap,
            Texture2D mask,
            TextureCube skyBox,
            PrimitiveType primitiveType = PrimitiveType.TriangleList)
        {
            this.Entity = entity;
            this.Vertices = vertices;
            this.Indices = indices;
            this.PrimitiveType = primitiveType;
            this.DiffuseMap = diffuseMap;
            this.SpecularMap = specularMap;
            this.NormalMap = normalMap;
            this.ReflectionMap = reflectionMap;
            this.Mask = mask;
            this.SkyBox = skyBox;
        }

        public Entity Entity { get; }

        public PrimitiveType PrimitiveType { get; }

        public GBufferVertex[] Vertices { get; }
        public int VertexOffset => 0;

        public int VertexCount => this.Vertices.Length;

        public short[] Indices { get; }

        public int IndexOffset => 0;

        public int PrimitiveCount
        {
            get
            {
                return this.PrimitiveType switch
                {
                    PrimitiveType.TriangleList => this.Indices.Length / 3,
                    PrimitiveType.TriangleStrip => this.Indices.Length - 2,
                    PrimitiveType.LineList => this.Indices.Length / 2,
                    PrimitiveType.LineStrip => this.Indices.Length - 1,
                    _ => throw new NotSupportedException($"Primitive type {this.PrimitiveType} is not supported"),
                };
            }
        }

        [Editor(nameof(DiffuseMap))]
        public Texture2D DiffuseMap { get; }

        [Editor(nameof(SpecularMap))]
        public Texture2D SpecularMap { get; }

        [Editor(nameof(NormalMap))]
        public Texture2D NormalMap { get; }

        [Editor(nameof(ReflectionMap))]
        public Texture2D ReflectionMap { get; }

        [Editor(nameof(Mask))]
        public Texture2D Mask { get; }

        [Editor(nameof(SkyBox))]
        public TextureCube SkyBox { get; }
    }
}

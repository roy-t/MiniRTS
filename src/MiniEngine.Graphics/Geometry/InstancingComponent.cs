using System;
using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class InstancingComponent : AComponent
    {
        public InstancingComponent(Entity entity, InstancingVertex[] vertexData)
            : base(entity)
        {
            this.VertexData = vertexData;
        }

        public InstancingVertex[] VertexData { get; }

        public int Instances => this.VertexData.Length;


        public static InstancingComponent Create(Entity entity, Matrix[] instances)
        {
            var vertexData = new InstancingVertex[instances.Length];
            unsafe
            {
                fixed (void* pInstances = &instances[0])
                fixed (void* pVertexData = &vertexData[0])
                {
                    var destinationSize = sizeof(InstancingVertex) * vertexData.Length;
                    var bytesToCopy = sizeof(Matrix) * instances.Length;
                    Buffer.MemoryCopy(pInstances, pVertexData, destinationSize, bytesToCopy);
                }
            }

            return new InstancingComponent(entity, vertexData);
        }
    }
}

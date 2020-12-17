using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MiniEngine.ContentPipeline.Serialization.Readers;
using MiniEngine.ContentPipeline.Shared;

namespace MiniEngine.ContentPipeline.Serialization.Writers
{
    [ContentTypeWriter]
    internal sealed class GeometryDataWriter : ContentTypeWriter<GeometryDataContent>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
            => typeof(GeometryDataReader).AssemblyQualifiedName;

        public override string GetRuntimeType(TargetPlatform targetPlatform)
            => typeof(GeometryData).AssemblyQualifiedName;

        protected override void Write(ContentWriter output, GeometryDataContent value)
            => WriteGeometryData(output, value);

        public static void WriteGeometryData(ContentWriter output, GeometryDataContent value)
        {
            output.Write(value.Name);
            Write(output, value.Vertices);
            Write(output, value.Indices);
            Write(output, value.Bounds);
        }

        private static void Write(ContentWriter output, BoundingSphere value)
        {
            output.Write(value.Center);
            output.Write(value.Radius);
        }

        // From VertexBufferWriter in MonoGame.Framework.Content.Pipeline
        private static void Write(ContentWriter output, VertexBufferContent value)
        {
            output.WriteRawObject(value.VertexDeclaration);
            output.Write((uint)(value.VertexData.Length / value.VertexDeclaration.VertexStride));
            output.Write(value.VertexData);
        }

        // From IndexBufferWriter in MonoGame.Framework.Content.Pipeline
        private static void Write(ContentWriter output, IndexCollection value)
        {
            // Check if the buffer and can be saved as Int16.
            var shortIndices = true;
            foreach (var index in value)
            {
                if (index > ushort.MaxValue)
                {
                    shortIndices = false;
                    break;
                }
            }

            output.Write(shortIndices);

            var byteCount = shortIndices
                                ? value.Count * 2
                                : value.Count * 4;

            output.Write(byteCount);
            if (shortIndices)
            {
                foreach (var item in value)
                    output.Write((ushort)item);
            }
            else
            {
                foreach (var item in value)
                    output.Write(item);
            }
        }
    }
}

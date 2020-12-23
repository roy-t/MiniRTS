using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.ContentPipeline.Shared;

namespace MiniEngine.ContentPipeline.Serialization.Readers
{
    internal sealed class GeometryDataReader : ContentTypeReader<GeometryData>
    {
        protected override GeometryData Read(ContentReader input, GeometryData _)
            => Read(input);

        public static GeometryData Read(ContentReader input)
        {
            var device = input.GetGraphicsDevice();

            var name = input.ReadString();
            var vertices = ReadVertexBuffer(input, device);
            var indices = ReadIndexBuffer(input, device);
            var bounds = ReadBoundingSphere(input);

            return new GeometryData(name, vertices, indices, bounds);
        }

        private static BoundingSphere ReadBoundingSphere(ContentReader input)
        {
            var center = input.ReadVector3();
            var radius = input.ReadSingle();
            return new BoundingSphere(center, radius);
        }

        private static VertexBuffer ReadVertexBuffer(ContentReader input, GraphicsDevice device)
        {
            var declaration = input.ReadRawObject<VertexDeclaration>();
            var vertexCount = (int)input.ReadUInt32();
            var dataSize = vertexCount * declaration.VertexStride;
            var data = new byte[dataSize];
            input.Read(data, 0, dataSize);

            var buffer = new VertexBuffer(device, declaration, vertexCount, BufferUsage.WriteOnly);

            buffer.SetData(data, 0, dataSize);
            return buffer;
        }

        private static IndexBuffer ReadIndexBuffer(ContentReader input, GraphicsDevice device)
        {
            var sixteenBits = input.ReadBoolean();
            var dataSize = input.ReadInt32();
            var data = new byte[dataSize];
            input.Read(data, 0, dataSize);

            var indexBuffer = new IndexBuffer(device,
                sixteenBits ? IndexElementSize.SixteenBits : IndexElementSize.ThirtyTwoBits,
                dataSize / (sixteenBits ? 2 : 4), BufferUsage.WriteOnly);

            indexBuffer.SetData(data, 0, dataSize);
            return indexBuffer;
        }
    }
}

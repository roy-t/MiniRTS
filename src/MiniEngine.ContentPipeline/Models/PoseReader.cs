//using System;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
//using MiniEngine.ContentPipeline.Shared;

//namespace MiniEngine.ContentPipeline.Models
//{
//    public class PoseReader : ContentTypeReader<Pose>
//    {
//        protected override Pose Read(ContentReader input, Pose existingInstance)
//        {
//            if (existingInstance != null)
//            {
//                throw new InvalidOperationException("Existing instance was not null?");
//            }

// var graphics =
// (GraphicsDeviceManager)input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceManager));
// var content = input.ContentManager;

// var offset = input.ReadMatrix(); var material = ReadMaterial(input, content); var bounds =
// ReadBoundingSphere(input); var vertexBuffer = ReadVertexBuffer(input, graphics); var indexBuffer
// = ReadIndexBuffer(input, graphics);

// var geometry = new GeometryData(vertexBuffer, indexBuffer, bounds);

// return new Pose(geometry, material, Matrix.Identity); }

// private static Material ReadMaterial(ContentReader input, ContentManager content) { var albedo =
// input.ReadExternalReference<Texture2D>(); var normal = input.ReadExternalReference<Texture2D>();

// return new Material(albedo, normal, 0.0f, 1.0f); }

// private static BoundingSphere ReadBoundingSphere(ContentReader input) { var center =
// input.ReadVector3(); var radius = input.ReadSingle(); return new BoundingSphere(center, radius); }

// private static VertexBuffer ReadVertexBuffer(ContentReader input, GraphicsDeviceManager graphics)
// { var declaration = input.ReadRawObject<VertexDeclaration>(); var vertexCount =
// (int)input.ReadUInt32(); var dataSize = vertexCount * declaration.VertexStride; var data = new
// byte[dataSize]; input.Read(data, 0, dataSize);

// var buffer = new VertexBuffer(graphics.GraphicsDevice, declaration, vertexCount, BufferUsage.None);

// buffer.SetData(data, 0, dataSize); return buffer; }

// private static IndexBuffer ReadIndexBuffer(ContentReader input, GraphicsDeviceManager graphics) {
// var sixteenBits = input.ReadBoolean(); var dataSize = input.ReadInt32(); var data = new
// byte[dataSize]; input.Read(data, 0, dataSize);

// var indexBuffer = new IndexBuffer(graphics.GraphicsDevice, sixteenBits ?
// IndexElementSize.SixteenBits : IndexElementSize.ThirtyTwoBits, dataSize / (sixteenBits ? 2 : 4), BufferUsage.None);

//            indexBuffer.SetData(data, 0, dataSize);
//            return indexBuffer;
//        }
//    }
//}

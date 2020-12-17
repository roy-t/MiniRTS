//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content.Pipeline;
//using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
//using Microsoft.Xna.Framework.Content.Pipeline.Processors;
//using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
//using MiniEngine.ContentPipeline.Shared;

//namespace MiniEngine.ContentPipeline.Models
//{
//    [ContentTypeWriter]
//    public sealed class PoseWriter : ContentTypeWriter<PoseContent>
//    {
//        public override string GetRuntimeReader(TargetPlatform targetPlatform)
//            => typeof(PoseReader).AssemblyQualifiedName;

// public override string GetRuntimeType(TargetPlatform targetPlatform)
// => typeof(Pose).AssemblyQualifiedName;

// public override bool CanDeserializeIntoExistingObject => false;

// protected override void Write(ContentWriter output, PoseContent value) { output.Write(value.Offset);

// Write(output, value.Material); Write(output, value.Bounds); Write(output, value.Vertices);
// Write(output, value.Indices); }

// private static void Write(ContentWriter output, MaterialReferences material) {
// output.WriteExternalReference(material.Albedo); output.WriteExternalReference(material.Normal); }

// private static void Write(ContentWriter output, BoundingSphere value) {
// output.Write(value.Center); output.Write(value.Radius); }

// // From VertexBufferWriter in MonoGame.Framework.Content.Pipeline private static void
// Write(ContentWriter output, VertexBufferContent value) {
// output.WriteRawObject(value.VertexDeclaration); output.Write((uint)(value.VertexData.Length /
// value.VertexDeclaration.VertexStride)); output.Write(value.VertexData); }

// // From IndexBufferWriter in MonoGame.Framework.Content.Pipeline private static void
// Write(ContentWriter output, IndexCollection value) { // Check if the buffer and can be saved as
// Int16. var shortIndices = true; foreach (var index in value) { if (index > ushort.MaxValue) {
// shortIndices = false; break; } }

// output.Write(shortIndices);

// var byteCount = shortIndices ? value.Count * 2 : value.Count * 4;

//            output.Write(byteCount);
//            if (shortIndices)
//            {
//                foreach (var item in value)
//                    output.Write((ushort)item);
//            }
//            else
//            {
//                foreach (var item in value)
//                    output.Write(item);
//            }
//        }
//    }
//}

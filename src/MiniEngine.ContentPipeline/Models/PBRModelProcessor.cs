using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.ContentPipeline.Models;
using MiniEngine.ContentPipeline.Shared;

namespace MiniEngine.ContentPipeline
{
    [ContentProcessor(DisplayName = "MiniEngine :: Model with PBR textures")]
    public class PBRModelProcessor : ContentProcessor<NodeContent, PoseContent>
    {
        [DisplayName("Fallback - Albedo")]
        public string FallbackAlbedo { get; set; } = "materials/albedo.tga";

        [DisplayName("Fallback - Metalicness")]
        public string FallbackMetalicness { get; set; } = "materials/metalicness.tga";

        [DisplayName("Fallback - Normal")]
        public string FallbackNormal { get; set; } = "materials/normal.tga";

        [DisplayName("Fallback - Roughness")]
        public string FallbackRoughness { get; set; } = "materials/roughness.tga";

        [DisplayName("Fallback - Mask")]
        public string FallbackMask { get; set; } = "materials/mask.tga";

        public override PoseContent Process(NodeContent input, ContentProcessorContext context)
        {
            var content = new PoseContent
            {
                Bounds = new BoundingSphere(Vector3.Up, 10.0f)
            };
            content.Indices.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            content.Vertices.VertexDeclaration = GeometryVertex.GetContent();
            content.Vertices.Write(0, 4, typeof(Vector4), new Vector4[] { Vector4.One, Vector4.Zero, Vector4.One });
            content.Material = this.ProcessMaterials(input, context);

            return content;
        }

        private MaterialReferences ProcessMaterials(NodeContent input, ContentProcessorContext context)
        {
            // TODO:
            // - Build all distinct materials found that we're going to use
            // - Store their references so we can easily look them up when generating the geometry
            // (so we can store them in the MaterialReferences/PoseContent
            var lookUp = new MaterialLookup(this.FallbackAlbedo, this.FallbackMetalicness, this.FallbackNormal, this.FallbackRoughness, this.FallbackMask);

            var nodes = input.AsEnumerable().SelectDeep(n => n.Children).ToList();
            var meshes = nodes.FindAll(n => n is MeshContent).Cast<MeshContent>().ToList();
            var geometries = meshes.SelectMany(m => m.Geometry).ToList();
            var materials = geometries.Select(g => g.Material).Distinct().ToList();

            var foo = materials.First(x => x.Textures.Count == 5);

            var albedo = lookUp.GetAlbedo(foo);
            var normal = lookUp.GetNormal(foo);

            var albedoBuild = context.BuildAsset<TextureContent, Texture2D>(albedo, "TextureProcessor");
            var normalBuild = context.BuildAsset<TextureContent, Texture2D>(normal, "TextureProcessor");

            var material = new MaterialReferences(albedoBuild, normalBuild);

            var bar = geometries.Where(g => g.Material == foo).First();
            var vertexBuffer = bar.Vertices.CreateVertexBuffer();
            //vertexBuffer.Write(bar.)

            return material;
        }
    }

    // From ModelProcessor in MonoGame.FrameworkContent.Pipeline
    internal static class ModelEnumerableExtensions
    {
        /// <summary>
        /// Returns each element of a tree structure in hierarchical order.
        /// </summary>
        /// <typeparam name="T">The enumerated type.</typeparam>
        /// <param name="source">The enumeration to traverse.</param>
        /// <param name="selector">A function which returns the children of the element.</param>
        /// <returns>An IEnumerable whose elements are in tree structure heriarchical order.</returns>
        public static IEnumerable<T> SelectDeep<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            var stack = new Stack<T>(source.Reverse());
            while (stack.Count > 0)
            {
                // Return the next item on the stack.
                var item = stack.Pop();
                yield return item;

                // Get the children from this item.
                var children = selector(item);

                // If we have no children then skip it.
                if (children == null)
                    continue;

                // We're using a stack, so we need to push the children on in reverse to get the
                // correct order.
                foreach (var child in children.Reverse())
                    stack.Push(child);
            }
        }

        /// <summary>
        /// Returns an enumerable from a single element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            yield return item;
        }
    }
}

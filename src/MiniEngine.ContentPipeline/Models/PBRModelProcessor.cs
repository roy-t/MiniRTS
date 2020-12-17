using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using MiniEngine.ContentPipeline.Models;

using M = MiniEngine.ContentPipeline.Serialization;

using X = Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace MiniEngine.ContentPipeline
{
    [ContentProcessor(DisplayName = "MiniEngine :: Model with PBR textures")]
    internal sealed class PBRModelProcessor : ContentProcessor<NodeContent, M.GeometryModelContent>
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

        private readonly MaterialBuilder MaterialBuilder;

        public PBRModelProcessor()
        {
            this.MaterialBuilder = new MaterialBuilder(new MaterialLookup(this.FallbackAlbedo, this.FallbackMetalicness, this.FallbackNormal, this.FallbackRoughness, this.FallbackMask));
        }

        public override M.GeometryModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            var nodes = input.AsEnumerable().SelectDeep(n => n.Children).ToList();
            var meshes = nodes.FindAll(n => n is MeshContent).Cast<MeshContent>().ToList();

            var materials = this.ProcessMaterials(meshes, context);

            var model = new M.GeometryModelContent();

            var meshCounter = 0;
            foreach (var mesh in meshes)
            {
                var meshName = string.IsNullOrWhiteSpace(mesh.Name) ? $"{meshCounter}" : mesh.Name;
                meshCounter++;

                var geometryCounter = 0;
                foreach (var geometry in mesh.Geometry)
                {
                    var geometryName = string.IsNullOrWhiteSpace(geometry.Name) ? $"{geometryCounter}" : geometry.Name;
                    geometryCounter++;

                    var name = string.Join(".", meshName, geometryName);
                    var vertexBuffer = geometry.Vertices.CreateVertexBuffer();
                    var indices = geometry.Indices;
                    var bounds = BoundingSphere.CreateFromPoints(mesh.Positions);

                    var geometryData = new M.GeometryDataContent(name, vertexBuffer, indices, bounds);

                    var material = materials[geometry];
                    var geometryMesh = new M.GeometryMeshContent(geometryData, material, mesh.AbsoluteTransform);

                    model.Add(geometryMesh);
                }
            }

            return model;
        }

        private Dictionary<X.GeometryContent, M.MaterialContent> ProcessMaterials(List<MeshContent> meshes, ContentProcessorContext context)
        {
            var cache = new Dictionary<X.GeometryContent, M.MaterialContent>();

            foreach (var mesh in meshes)
            {
                foreach (var geometry in mesh.Geometry)
                {
                    var material = this.MaterialBuilder.Build(geometry.Material, context);
                    cache.Add(geometry, material);
                }
            }

            return cache;
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

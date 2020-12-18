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
    [ContentProcessor(DisplayName = "GeometryModel - MiniEngine")]
    internal sealed class GeometryModelProcessor : ContentProcessor<NodeContent, M.GeometryModelContent>
    {
        [DisplayName("Fallback - Albedo")]
        public string FallbackAlbedo { get; set; } = "materials/albedo.tga";

        [DisplayName("Fallback - Metalicness")]
        public string FallbackMetalicness { get; set; } = "materials/metalicness.tga";

        [DisplayName("Fallback - Normal")]
        public string FallbackNormal { get; set; } = "materials/normal.tga";

        [DisplayName("Fallback - Roughness")]
        public string FallbackRoughness { get; set; } = "materials/roughness.tga";

        [DisplayName("Fallback - Ambient Occlusion")]
        public string FallbackAmbientOcclusion { get; set; } = "materials/ao.tga";

        private readonly MaterialBuilder MaterialBuilder;

        private static readonly IList<string> AcceptableVertexChannelNames =
           new[]
           {
                VertexChannelNames.TextureCoordinate(0),
                VertexChannelNames.Normal(0)
           };

        public GeometryModelProcessor()
        {
            this.MaterialBuilder = new MaterialBuilder(new MaterialLookup(this.FallbackAlbedo, this.FallbackMetalicness, this.FallbackNormal, this.FallbackRoughness, this.FallbackAmbientOcclusion));
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

                    StripVertexChannels(geometry);

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

        private static void StripVertexChannels(GeometryContent geometry)
        {
            var channels = geometry.Vertices.Channels;
            for (var i = channels.Count - 1; i >= 0; i--)
            {
                var vertexChannelName =
               geometry.Vertices.Channels[i].Name;

                if (!AcceptableVertexChannelNames.Contains(vertexChannelName))
                {
                    geometry.Vertices.Channels.Remove(vertexChannelName);
                }
            }
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
            };

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

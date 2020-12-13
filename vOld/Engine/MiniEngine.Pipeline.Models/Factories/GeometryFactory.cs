using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives.VertexTypes;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Models.Factories
{
    public sealed class GeometryFactory : AComponentFactory<Geometry>
    {
        private readonly IComponentContainer<Bounds> Bounds;

        public GeometryFactory(GraphicsDevice device,
            IComponentContainer<Geometry> container,
            IComponentContainer<Bounds> bounds)
            : base(device, container)
        {
            this.Bounds = bounds;
        }

        public Geometry Construct(Entity entity, GBufferVertex[] vertices, int[] indices, Texture2D diffuseMap, PrimitiveType primitiveType = PrimitiveType.TriangleList)
        {
            var specularMap = this.GenerateTexture(Color.White, 1, 1);
            var normalMap = this.GenerateTexture(new Color(0.5f, 0.5f, 1.0f, 1.0f), 1, 1);
            var reflectionMap = this.GenerateTexture(Color.Black, 1, 1);
            var mask = this.GenerateTexture(Color.White, 1, 1);
            var skybox = this.GenerateSkybox(Color.White);

            var geometry = new Geometry(entity, vertices, indices, diffuseMap, specularMap, normalMap, reflectionMap, mask, skybox, primitiveType);
            this.Container.Add(geometry);

            BoundaryComputer.ComputeExtremes(geometry, out var min, out var max);
            var bounds = new Bounds(entity, min, max);
            this.Bounds.Add(bounds);

            return geometry;
        }

        public override void Deconstruct(Entity entity)
        {
            this.Bounds.Remove(entity);
            base.Deconstruct(entity);
        }


        private Texture2D GenerateTexture(Color color, int width, int height)
        {
            var texture = new Texture2D(this.Device, width, height);
            texture.SetData(Enumerable.Repeat(color, width * height).ToArray());

            return texture;
        }

        private TextureCube GenerateSkybox(Color color)
        {
            var skybox = new TextureCube(this.Device, 1, false, SurfaceFormat.Color);
            skybox.SetData(CubeMapFace.PositiveX, new Color[] { color });
            skybox.SetData(CubeMapFace.NegativeX, new Color[] { color });
            skybox.SetData(CubeMapFace.PositiveY, new Color[] { color });
            skybox.SetData(CubeMapFace.NegativeY, new Color[] { color });
            skybox.SetData(CubeMapFace.PositiveZ, new Color[] { color });
            skybox.SetData(CubeMapFace.NegativeZ, new Color[] { color });

            return skybox;
        }
    }
}

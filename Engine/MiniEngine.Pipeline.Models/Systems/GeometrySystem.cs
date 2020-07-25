using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.Wrappers;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Models.Systems
{
    public sealed class GeometrySystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly IComponentContainer<Geometry> Geometries;
        private readonly IComponentContainer<Pose> Poses;
        private readonly IComponentContainer<Bounds> Bounds;
        private readonly RenderEffect Effect;
        private readonly TextureCube NullSkybox;

        public GeometrySystem(
            GraphicsDevice device,
            EffectFactory effectFactory,
            IComponentContainer<Geometry> geometries,
            IComponentContainer<Pose> poses,
            IComponentContainer<Bounds> bounds)
        {
            this.Device = device;
            this.Geometries = geometries;
            this.Poses = poses;
            this.Bounds = bounds;

            this.Effect = effectFactory.Construct<RenderEffect>();

            this.NullSkybox = new TextureCube(device, 1, false, SurfaceFormat.Color);
            this.NullSkybox.SetData(CubeMapFace.PositiveX, new Color[] { Color.White });
            this.NullSkybox.SetData(CubeMapFace.NegativeX, new Color[] { Color.White });
            this.NullSkybox.SetData(CubeMapFace.PositiveY, new Color[] { Color.White });
            this.NullSkybox.SetData(CubeMapFace.NegativeY, new Color[] { Color.White });
            this.NullSkybox.SetData(CubeMapFace.PositiveZ, new Color[] { Color.White });
            this.NullSkybox.SetData(CubeMapFace.NegativeZ, new Color[] { Color.White });
        }

        public void Render(PerspectiveCamera camera, GBuffer gBuffer)
        {
            for (var i = 0; i < this.Geometries.Count; i++)
            {
                var geometry = this.Geometries[i];
                var bounds = this.Bounds[i];

                if (bounds.IsInView)
                {
                    var pose = this.Poses[i];

                    this.Effect.TextureOffset = Vector2.Zero;
                    this.Effect.World = pose.Matrix;
                    this.Effect.View = camera.View;
                    this.Effect.Projection = camera.Projection;
                    this.Effect.InverseViewProjection = camera.InverseViewProjection;
                    this.Effect.Skybox = this.NullSkybox;
                    this.Effect.CameraPosition = camera.Position;
                    this.Effect.TextureScale = Vector2.One;

                    this.Effect.Apply(Effects.Techniques.RenderEffectTechniques.Deferred);

                    this.DrawGeometry(geometry);
                }
            }
        }


        public void RenderShadowMaps(PerspectiveCamera _, RenderTarget2D shadowMap)
        {
            // TODO: we can propably render shadows from here instead of creating batches for the shadow map system
        }

        private void DrawGeometry(Geometry geometry)
        {
            this.Device.DrawUserIndexedPrimitives(
                geometry.PrimitiveType,
                geometry.Vertices,
                geometry.VertexOffset,
                geometry.VertexCount,
                geometry.Indices,
                geometry.IndexOffset,
                geometry.PrimitiveCount
            );
        }
    }
}

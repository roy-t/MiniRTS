using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.Wrappers;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Models.Components;
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
        }

        public void Render(PerspectiveCamera camera)
        {
            for (var i = 0; i < this.Geometries.Count; i++)
            {
                var geometry = this.Geometries[i];
                var bounds = this.Bounds.Get(geometry.Entity);

                if (bounds.IsInView)
                {
                    var pose = this.Poses.Get(geometry.Entity);

                    this.Effect.DiffuseMap = geometry.DiffuseMap;
                    this.Effect.SpecularMap = geometry.SpecularMap;
                    this.Effect.NormalMap = geometry.NormalMap;
                    this.Effect.ReflectionMap = geometry.ReflectionMap;
                    this.Effect.Mask = geometry.Mask;
                    this.Effect.Skybox = geometry.SkyBox;

                    this.Effect.TextureOffset = Vector2.Zero;
                    this.Effect.World = pose.Matrix;
                    this.Effect.View = camera.View;
                    this.Effect.Projection = camera.Projection;
                    this.Effect.InverseViewProjection = camera.InverseViewProjection;
                    this.Effect.CameraPosition = camera.Position;
                    this.Effect.TextureScale = Vector2.One;

                    this.Effect.Apply(Effects.Techniques.RenderEffectTechniques.Deferred);

                    this.DrawGeometry(geometry);
                }
            }
        }


        //public void RenderShadowMaps(PerspectiveCamera _, RenderTarget2D shadowMap)
        //{
        //    // TODO: we can propably render shadows from here instead of creating batches for the shadow map system
        //}

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

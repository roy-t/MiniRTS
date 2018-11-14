using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Shadows.Components;
using MiniEngine.Pipeline.Shadows.Utilities;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Units;
using System;
using System.Collections.Generic;

namespace MiniEngine.Pipeline.Shadows.Systems
{
    public sealed class CascadedShadowMapSystem : IUpdatableSystem
    {
        private const int DefaultResolution = 1024;
        private static readonly float[] DefaultCascadeDistances =
        {
            0.075f,
            0.15f,
            0.3f,
            1.0f
        };

        private readonly GraphicsDevice Device;
        private readonly EntityCreator EntityCreator;
        private readonly Dictionary<Entity, CascadedShadowMap> ShadowMaps;
        private readonly Dictionary<Entity, CascadeInfo> Cascades;
        private readonly ShadowMapSystem ShadowMapSystem;

        private readonly Frustum Frustum;

        public CascadedShadowMapSystem(GraphicsDevice device, EntityCreator entityCreator, ShadowMapSystem shadowMapSystem)
        {
            this.Device = device;
            this.EntityCreator = entityCreator;            
            this.ShadowMapSystem = shadowMapSystem;

            this.ShadowMaps = new Dictionary<Entity, CascadedShadowMap>();
            this.Cascades = new Dictionary<Entity, CascadeInfo>();

            this.Frustum = new Frustum();
        }

        public void Add(Entity entity, Vector3 position, Vector3 lookAt, int cascades, int resolution, float[] cascadeDistances)
        {
            var shadowMap = new CascadedShadowMap(this.Device, resolution, cascades);
            this.ShadowMaps.Add(entity, shadowMap);

            var cascade = new CascadeInfo(position, lookAt, cascades, resolution, cascadeDistances);
            this.Cascades.Add(entity, cascade);

            var childEntities = this.EntityCreator.CreateChildEntities(entity, cascades);
            for (var i = 0; i < cascades; i++)
            {
                this.ShadowMapSystem.Add(childEntities[i], shadowMap.DepthMapArray, shadowMap.ColorMapArray, i, cascade.ShadowCameras[i]);
            }            
        }

        public void Add(Entity entity, Vector3 position, Vector3 lookAt, int cascades, int resolution = DefaultResolution)
            => this.Add(entity, position, lookAt, cascades, resolution, DefaultCascadeDistances);

        public CascadedShadowMap GetMaps(Entity entity) => this.ShadowMaps[entity];
        public CascadeInfo GetCascades(Entity entity) => this.Cascades[entity];

        public bool Contains(Entity entity) => this.ShadowMaps.ContainsKey(entity);
        public string Describe(Entity entity)
        {
            var map = this.ShadowMaps[entity];
            return $"cascaded shadow map, dimensions: {map.DepthMapArray.Width}x{map.DepthMapArray.Height}, cascades: {map.Cascades}";
        }
        public void Remove(Entity entity)
        {
            this.ShadowMaps.Remove(entity);
            var children = this.EntityCreator.GetChilderen(entity);
            foreach(var child in children)
            {
                this.ShadowMapSystem.Remove(child);
            }

            this.Cascades.Remove(entity);
        }

        public void Update(PerspectiveCamera perspectiveCamera, Seconds elapsed)
        {
            foreach (var cascade in this.Cascades.Values)
            {
                this.Frustum.ResetToViewVolume();
                this.Frustum.Transform(perspectiveCamera.InverseViewProjection);
                cascade.GlobalShadowMatrix =
                    ShadowMath.CreateGlobalShadowMatrix(cascade.SurfaceToLightVector, this.Frustum);

                for (var cascadeIndex = 0; cascadeIndex < cascade.CascadeSplits.Length; cascadeIndex++)
                {
                    this.Frustum.ResetToViewVolume();
                    // Transform to world space
                    this.Frustum.Transform(perspectiveCamera.InverseViewProjection);

                    // Slice the frustum
                    var nearZ = cascadeIndex == 0 ? 0.0f : cascade.CascadeDistances[cascadeIndex - 1];
                    var farZ = cascade.CascadeDistances[cascadeIndex];
                    this.Frustum.Slice(nearZ, farZ);

                    cascade.ShadowCameras[cascadeIndex].CoverFrustum(cascade.SurfaceToLightVector, this.Frustum, cascade.Resolution);                  

                    // ViewProjection matrix of the shadow camera that transforms to texture space [0, 1] instead of [-1, 1]
                    var shadowMatrix = cascade.ShadowCameras[cascadeIndex].ViewProjection.TextureScaleTransform();

                    // Store the split distance in terms of view space depth
                    var clipDistance = perspectiveCamera.FarPlane - perspectiveCamera.NearPlane;
                    cascade.CascadeSplits[cascadeIndex] = perspectiveCamera.NearPlane + (farZ * clipDistance);

                    // Find scale and offset of this cascade in world space                    
                    var invCascadeMat = Matrix.Invert(shadowMatrix);
                    var cascadeCorner = Vector4.Transform(Vector3.Zero, invCascadeMat).ScaleToVector3();
                    cascadeCorner = Vector4.Transform(cascadeCorner, cascade.GlobalShadowMatrix).ScaleToVector3();

                    // Do the same for the upper corner
                    var otherCorner = Vector4.Transform(Vector3.One, invCascadeMat).ScaleToVector3();
                    otherCorner = Vector4.Transform(otherCorner, cascade.GlobalShadowMatrix).ScaleToVector3();

                    // Calculate the scale and offset
                    var cascadeScale = Vector3.One / (otherCorner - cascadeCorner);
                    cascade.CascadeOffsets[cascadeIndex] = new Vector4(-cascadeCorner, 0.0f);
                    cascade.CascadeScales[cascadeIndex] = new Vector4(cascadeScale, 1.0f);
                }
            }
        }
    }
}

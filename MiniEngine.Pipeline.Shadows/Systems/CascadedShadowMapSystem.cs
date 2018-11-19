using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Shadows.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Units;
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

        private static readonly Matrix TexScaleTransform = Matrix.CreateScale(0.5f, -0.5f, 1.0f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.0f);

        private readonly GraphicsDevice Device;
        private readonly EntityLinker EntityLinker;
        private readonly List<CascadedShadowMap> ShadowMaps;
        private readonly List<CascadeInfo> Cascades;

        private readonly Frustum Frustum;

        public CascadedShadowMapSystem(GraphicsDevice device, EntityLinker entityLinker)
        {
            this.Device = device;
            this.EntityLinker = entityLinker;

            this.ShadowMaps = new List<CascadedShadowMap>();
            this.Cascades = new List<CascadeInfo>();

            this.Frustum = new Frustum();
        }

        public void Add(Entity entity, Vector3 position, Vector3 lookAt, int cascades, int resolution, float[] cascadeDistances)
        {
            var cascadedShadowMap = new CascadedShadowMap(this.Device, resolution, cascades);
            this.EntityLinker.AddComponent(entity, cascadedShadowMap);

            var cascade = new CascadeInfo(position, lookAt, cascades, resolution, cascadeDistances);
            this.EntityLinker.AddComponent(entity, cascade);
            
            for (var i = 0; i < cascades; i++)
            {                
                var shadowMap = new ShadowMap(cascadedShadowMap.DepthMapArray, cascadedShadowMap.ColorMapArray, i, cascade.ShadowCameras[i]);
                this.EntityLinker.AddComponent(entity, shadowMap);
            }            
        }

        public void Add(Entity entity, Vector3 position, Vector3 lookAt, int cascades, int resolution = DefaultResolution)
            => this.Add(entity, position, lookAt, cascades, resolution, DefaultCascadeDistances);

        public void Remove(Entity entity)
        {
            this.EntityLinker.RemoveComponents<CascadedShadowMap>(entity);
            this.EntityLinker.RemoveComponents<CascadeInfo>(entity);
            this.EntityLinker.RemoveComponents<ShadowMap>(entity);            
        }

        public void Update(PerspectiveCamera perspectiveCamera, Seconds elapsed)
        {
            this.Cascades.Clear();
            this.EntityLinker.GetComponentsOfType(this.Cascades);


            foreach (var cascade in this.Cascades)
            {
                this.Frustum.ResetToViewVolume();
                this.Frustum.Transform(perspectiveCamera.InverseViewProjection);
                cascade.GlobalShadowMatrix = CreateGlobalShadowMatrix(cascade.SurfaceToLightVector, this.Frustum);

                for (var cascadeIndex = 0; cascadeIndex < cascade.CascadeSplits.Length; cascadeIndex++)
                {
                    this.Frustum.ResetToViewVolume();
                    // Transform to world space
                    this.Frustum.Transform(perspectiveCamera.InverseViewProjection);

                    // Slice the frustum
                    var nearZ = cascadeIndex == 0 ? 0.0f : cascade.CascadeDistances[cascadeIndex - 1];
                    var farZ = cascade.CascadeDistances[cascadeIndex];
                    this.Frustum.Slice(nearZ, farZ);

                    // Place a camera at the intersection of bounding sphere of the frustum and the ray
                    // from the frustum's center in the direction of the surface to light vector
                    cascade.ShadowCameras[cascadeIndex].CoverFrustum(cascade.SurfaceToLightVector, this.Frustum, cascade.Resolution);

                    // ViewProjection matrix of the shadow camera that transforms to texture space [0, 1] instead of [-1, 1]
                    var shadowMatrix = cascade.ShadowCameras[cascadeIndex].ViewProjection * TexScaleTransform;

                    // Store the split distance in terms of view space depth
                    var clipDistance = perspectiveCamera.FarPlane - perspectiveCamera.NearPlane;
                    cascade.CascadeSplits[cascadeIndex] = perspectiveCamera.NearPlane + (farZ * clipDistance);

                    // Find scale and offset of this cascade in world space                    
                    var invCascadeMat = Matrix.Invert(shadowMatrix);
                    var cascadeCorner = ScaleToVector3(Vector4.Transform(Vector3.Zero, invCascadeMat));
                    cascadeCorner = ScaleToVector3(Vector4.Transform(cascadeCorner, cascade.GlobalShadowMatrix));

                    // Do the same for the upper corner
                    var otherCorner = ScaleToVector3(Vector4.Transform(Vector3.One, invCascadeMat));
                    otherCorner = ScaleToVector3(Vector4.Transform(otherCorner, cascade.GlobalShadowMatrix));

                    // Calculate the scale and offset
                    var cascadeScale = Vector3.One / (otherCorner - cascadeCorner);
                    cascade.CascadeOffsets[cascadeIndex] = new Vector4(-cascadeCorner, 0.0f);
                    cascade.CascadeScales[cascadeIndex] = new Vector4(cascadeScale, 1.0f);
                }
            }
        }

        public bool Contains(Entity entity) => false;
        public string Describe(Entity entity) => "";

        /// <summary>
        /// Create the shadow matrix that covers the entire frustum in texture space
        /// </summary>
        private static Matrix CreateGlobalShadowMatrix(Vector3 surfaceToLightDirection, Frustum frustum)
        {
            var frustumCenter = frustum.ComputeCenter();
            var shadowCameraPos = frustumCenter + (surfaceToLightDirection * -0.5f);

            var view = Matrix.CreateLookAt(shadowCameraPos, frustumCenter, Vector3.Up);
            var projection = Matrix.CreateOrthographicOffCenter(-0.5f, 0.5f, -0.5f, 0.5f, 0.0f, 1.0f);

            return view * projection * TexScaleTransform;
        }

        private static Vector3 ScaleToVector3(Vector4 value) => new Vector3(value.X, value.Y, value.Z) / value.W;
    }
}

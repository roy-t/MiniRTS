using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Shadows.Components;
using MiniEngine.Pipeline.Shadows.Factories;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Units;
using System.Collections.Generic;

namespace MiniEngine.Pipeline.Shadows.Systems
{
    public sealed class CascadedShadowMapSystem : IUpdatableSystem
    {        
        private static readonly Matrix TexScaleTransform = Matrix.CreateScale(0.5f, -0.5f, 1.0f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.0f);

        private readonly GraphicsDevice Device;
        private readonly EntityLinker Linker;
        private readonly CascadedShadowMapFactory ComponentFactory;
        private readonly List<ShadowMapCascades> ShadowMaps;
        private readonly Frustum Frustum;

        public CascadedShadowMapSystem(GraphicsDevice device, EntityLinker linker, CascadedShadowMapFactory componentFactory)
        {
            this.Device = device;
            this.Linker = linker;
            this.ComponentFactory = componentFactory;

            this.ShadowMaps = new List<ShadowMapCascades>();
            this.Frustum = new Frustum();
        }     

        public void Update(PerspectiveCamera perspectiveCamera, Seconds elapsed)
        {
            this.ShadowMaps.Clear();
            this.Linker.GetComponentsOfType(this.ShadowMaps);

            foreach (var shadowMapCascade in this.ShadowMaps)
            {
                this.Frustum.ResetToViewVolume();
                this.Frustum.Transform(perspectiveCamera.InverseViewProjection);
                shadowMapCascade.GlobalShadowMatrix = CreateGlobalShadowMatrix(shadowMapCascade.SurfaceToLightVector, this.Frustum);

                for (var cascadeIndex = 0; cascadeIndex < shadowMapCascade.CascadeSplits.Length; cascadeIndex++)
                {
                    this.Frustum.ResetToViewVolume();
                    // Transform to world space
                    this.Frustum.Transform(perspectiveCamera.InverseViewProjection);

                    // Slice the frustum
                    var nearZ = cascadeIndex == 0 ? 0.0f : shadowMapCascade.CascadeDistances[cascadeIndex - 1];
                    var farZ = shadowMapCascade.CascadeDistances[cascadeIndex];
                    this.Frustum.Slice(nearZ, farZ);

                    // Place a camera at the intersection of bounding sphere of the frustum and the ray
                    // from the frustum's center in the direction of the surface to light vector
                    shadowMapCascade.ShadowCameras[cascadeIndex].CoverFrustum(shadowMapCascade.SurfaceToLightVector, this.Frustum, shadowMapCascade.Resolution);

                    // ViewProjection matrix of the shadow camera that transforms to texture space [0, 1] instead of [-1, 1]
                    var shadowMatrix = shadowMapCascade.ShadowCameras[cascadeIndex].ViewProjection * TexScaleTransform;

                    // Store the split distance in terms of view space depth
                    var clipDistance = perspectiveCamera.FarPlane - perspectiveCamera.NearPlane;
                    shadowMapCascade.CascadeSplits[cascadeIndex] = perspectiveCamera.NearPlane + (farZ * clipDistance);

                    // Find scale and offset of this cascade in world space                    
                    var invCascadeMat = Matrix.Invert(shadowMatrix);
                    var cascadeCorner = ScaleToVector3(Vector4.Transform(Vector3.Zero, invCascadeMat));
                    cascadeCorner = ScaleToVector3(Vector4.Transform(cascadeCorner, shadowMapCascade.GlobalShadowMatrix));

                    // Do the same for the upper corner
                    var otherCorner = ScaleToVector3(Vector4.Transform(Vector3.One, invCascadeMat));
                    otherCorner = ScaleToVector3(Vector4.Transform(otherCorner, shadowMapCascade.GlobalShadowMatrix));

                    // Calculate the scale and offset
                    var cascadeScale = Vector3.One / (otherCorner - cascadeCorner);
                    shadowMapCascade.CascadeOffsets[cascadeIndex] = new Vector4(-cascadeCorner, 0.0f);
                    shadowMapCascade.CascadeScales[cascadeIndex] = new Vector4(cascadeScale, 1.0f);
                }
            }
        }

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
        
        public void Remove(Entity entity) => throw new System.NotImplementedException();
    }
}

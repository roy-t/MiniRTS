using System;
using Microsoft.Xna.Framework;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Utilities.Extensions;

namespace MiniEngine.Rendering
{
    public static class ShadowMath
    {
        /// <summary>
        /// Create the shadow matrix that covers the entire frustum
        /// </summary>
        public static Matrix CreateGlobalShadowMatrix(Vector3 surfaceToLightDirection, Frustum frustum)
        {
            var frustumCenter = frustum.ComputeCenter();

            var shadowCameraPos = frustumCenter + surfaceToLightDirection * -0.5f;
            var shadowCamera = new OrthographicCamera(-0.5f, 0.5f, -0.5f, 0.5f, 0.0f, 1.0f);
            shadowCamera.Move(shadowCameraPos, frustumCenter);

            return (shadowCamera.View * shadowCamera.Projection).TextureScaleTransform();
        }


        public static IViewPoint CreateShadowCamera(
            Vector3 surfaceToLightVector,
            Frustum frustum,
            int shadowMapResolution)
        {
            // Compute the bounding sphere of the frustum slice, round the center
            // so our shadows are more stable when moving the camera around
            var bounds = frustum.ComputeBounds();

            // WARNING: This might cause problems when moving the shadow caster around in that case use
            // var radius = (float)Math.Ceiling(bounds.Radius * 16.0f) / 16.0f;
            var radius = (float) Math.Ceiling(bounds.Radius);

            var shadowCamera = new OrthographicCamera(
                -radius,
                radius,
                -radius,
                radius,
                0.0f,
                radius * 2);

            // Compute the position of the shadow camera (the position where we're going to capture our shadow maps from)
            var shadowCameraPos = bounds.Center + surfaceToLightVector * radius;

            shadowCamera.Move(shadowCameraPos, bounds.Center);
            return Stabilize(shadowCamera, shadowMapResolution);
        }

        private static IViewPoint Stabilize(IViewPoint viewPoint, int shadowMapResolution)
        {
            // Create the rounding matrix, by projecting the world-space origin and determining
            // the fractional offset in texel space
            var shadowMatrixTemp = viewPoint.View * viewPoint.Projection;
            var shadowOrigin = new Vector3(0.0f, 0.0f, 0.0f);
            shadowOrigin = Vector3.Transform(shadowOrigin, shadowMatrixTemp);
            shadowOrigin = shadowOrigin * (shadowMapResolution / 2.0f);

            var roundedOrigin = shadowOrigin.Round();
            var roundOffset = roundedOrigin - shadowOrigin;
            roundOffset = roundOffset * (2.0f / shadowMapResolution);
            roundOffset.Z = 0.0f;

            var shadowProj = viewPoint.Projection;
            shadowProj.M41 += roundOffset.X;
            shadowProj.M42 += roundOffset.Y;
            shadowProj.M43 += roundOffset.Z;

            return new ViewPoint(viewPoint.View, shadowProj, viewPoint.Position, viewPoint.Forward);
        }
    }
}
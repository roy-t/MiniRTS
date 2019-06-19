using System;
using Microsoft.Xna.Framework;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Primitives
{
    public static class ProjectionMath
    {       
        public static Matrix ComputeProjectionMatrixThatFitsFrustum(PerspectiveCamera camera, BoundingFrustum frustum, float epsilon = 1.0f)
        {
            var corners = frustum.GetCorners();
            var far = float.MinValue;
            for(var i = 0; i < BoundingFrustum.CornerCount; i++)
            {
                far = Math.Max(far, Vector3.Distance(corners[i], camera.Position));            
            }

            var near = camera.NearPlane;
            far += epsilon;
            var projectionMatrix = camera.Projection;
            projectionMatrix.M33 = far / (near - far);
            projectionMatrix.M43 = near * far / (near - far);

            return projectionMatrix;
        }

        /// <summary>
        /// Converts screen-space {X, Y} coordinates to texture coordinates
        /// So from {[-1..1], [-1..1]} to {[0..1], [1..0]}
        /// </summary>
        public static Vector2 ToUv(float x, float y)
            => new Vector2((x * 0.5f) + 0.5f, (y * -0.5f) + 0.5f);

        /// <summary>
        /// Converts a world position to a position on screen
        /// </summary>
        public static Vector2 WorldToView(Vector3 worldPosition, Matrix viewProjection)
        {
            var x = (worldPosition.X * viewProjection.M11) + (worldPosition.Y * viewProjection.M21)
                                                           + (worldPosition.Z * viewProjection.M31)
                                                           + viewProjection.M41;

            var y = (worldPosition.X * viewProjection.M12) + (worldPosition.Y * viewProjection.M22)
                                                           + (worldPosition.Z * viewProjection.M32)
                                                           + viewProjection.M42;

            var w = (worldPosition.X * viewProjection.M14) + (worldPosition.Y * viewProjection.M24)
                                                           + (worldPosition.Z * viewProjection.M34)
                                                           + viewProjection.M44;

            if (!WithinEpsilon(w, 1.0f))
            {
                x = x / w;
                y = y / w;
            }

            return new Vector2(x, y);
        }

        private static bool WithinEpsilon(float a, float b)
        {
            float num = a - b;
            if (-1.401298E-45f <= num)
            {
                return num <= 1.401298E-45f;
            }
            return false;
        }

        public static bool IsInFrontOffCamera(Vector3 point, IViewPoint viewPoint)
        {
            var cornerDirection = Vector3.Normalize(point - viewPoint.Position);
            var dot = Vector3.Dot(viewPoint.Forward, cornerDirection);

            return dot > 0;
        }

        public static bool IsBehindCamera(Vector3 point, IViewPoint viewPoint)
        {
            var cornerDirection = Vector3.Normalize(point - viewPoint.Position);
            var dot = Vector3.Dot(viewPoint.Forward, cornerDirection);

            return dot < 0;            
        }
    }
}

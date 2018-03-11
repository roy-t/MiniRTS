using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MiniEngine.Rendering;
using MiniEngine.Rendering.Lighting;

namespace MiniEngine.Mathematics
{
    // TODO: clean up code, currently copied from tutorial as-is.
    public static class Frustum
    {
        // From: http://dev.theomader.com/cascaded-shadow-mapping-1/

        public static FrustumSplitProjection[] SplitFrustum(Matrix shadowView, Camera camera, BoundingBox sceneBoundingBox, float[] viewSpaceSplitDistances)
        {
            // determine clip space split distances
            var clipSpaceSplitDistances = viewSpaceSplitDistances
                .Select(d =>
                {
                    var c = Vector4.Transform(new Vector3(0, 0, d), camera.Projection);
                    return c.Z / c.W;
                })
                .ToArray();

            // determine split projections
            return Enumerable.Range(0, viewSpaceSplitDistances.Length - 1).Select(i =>
            {
                var n = clipSpaceSplitDistances[i];
                var f = clipSpaceSplitDistances[i + 1];

                // get frustum split corners and transform into shadow space
                var frustumCorners = SplitFrustum(n, f, camera.InverseViewProjection)
                    .Select(v => Vector3.Transform(v, shadowView))
                    .ToArray();

                var min = frustumCorners.Aggregate(Vector3.Min);
                var max = frustumCorners.Aggregate(Vector3.Max);
                              
                // determine the min/max z values based on arena bounding box
                var bounds = sceneBoundingBox.Transform(shadowView);
                var minZ = -bounds.Max.Z;
                var maxZ = -bounds.Min.Z;

                // return orthographic projection
                return new FrustumSplitProjection(f, Matrix.CreateOrthographicOffCenter(min.X, max.X, min.Y, max.Y, minZ, maxZ));
            }).ToArray();
        }

        public static IEnumerable<Vector3> SplitFrustum(float clipSpaceNear, float clipSpaceFar,
                                                 Matrix viewProjectionInverse)
        {
            var clipCorners = new[]
            {
                new Vector3( -1,  1, clipSpaceNear ),
                new Vector3(  1,  1, clipSpaceNear ),
                new Vector3(  1, -1, clipSpaceNear ),
                new Vector3( -1, -1, clipSpaceNear ),
                new Vector3( -1,  1, clipSpaceFar  ),
                new Vector3(  1,  1, clipSpaceFar  ),
                new Vector3(  1, -1, clipSpaceFar  ),
                new Vector3( -1, -1, clipSpaceFar  )
            };

            return clipCorners.Select(v =>
            {
                var vt = Vector4.Transform(v, viewProjectionInverse);
                vt /= vt.W;

                return new Vector3(vt.X, vt.Y, vt.Z);
            });
        }

        public static float ViewSpaceToClipSpace(float distance, Matrix projectionMatrix)
        {
            var c = Vector4.Transform(new Vector3(0, 0, distance), projectionMatrix);
            return c.Z / c.W;
        }
    }
}

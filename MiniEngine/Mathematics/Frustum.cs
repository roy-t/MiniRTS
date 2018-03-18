using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using MiniEngine.Rendering;

namespace MiniEngine.Mathematics
{
    // TODO: clean up code, currently copied from tutorial as-is.
    public static class Frustum
    {
        // From: http://dev.theomader.com/cascaded-shadow-mapping-1/

        public static FrustumSplitProjection[] SplitFrustum(Matrix shadowView, Camera camera, BoundingBox sceneBoundingBox, int shadowMapSize, float[] viewSpaceSplitDistances)
        {
            var viewDistance = Vector3.Distance(sceneBoundingBox.Min, sceneBoundingBox.Max);            
            var splitPlanes = PracticalSplitScheme(viewSpaceSplitDistances.Length - 1, 1, viewDistance)
                .Select(v => -v)
                .ToArray();

            // determine clip space split distances
            var splitDistances = viewSpaceSplitDistances
                .Select(d =>
                {
                    var c = Vector4.Transform(new Vector3(0, 0, d), camera.Projection);
                    return c.Z / c.W;
                })
                .ToArray();

            // determine split projections
            return Enumerable.Range(0, splitDistances.Length - 1).Select(i =>
            {
                var n = splitDistances[i];
                var f = splitDistances[i + 1];

                var viewSplit = SplitFrustum(n, f, camera.InverseViewProjection).ToArray();
                var frustumCorners = viewSplit.Select(v => Vector3.Transform(v, shadowView)).ToArray();
                var cameraPosition = Vector3.Transform(camera.InverseView.Translation, shadowView);

                var viewMin = frustumCorners.Aggregate(Vector3.Min);
                var viewMax = frustumCorners.Aggregate(Vector3.Max);

                var arenaBB = sceneBoundingBox.Transform(shadowView);

                var minZ = -arenaBB.Max.Z;
                var maxZ = -arenaBB.Min.Z;

                var range = -splitPlanes[i + 1];
                //var range = Math.Max(
                //    1.0f / camera.Projection.M11 * -splitPlanes[i + 1] * 2.0f,
                //    -splitPlanes[i + 1] - (-splitPlanes[i])
                //);

                var padding = 0.0f;
                var quantizationStep = (range + padding) / shadowMapSize;

                var x = DetermineShadowMinMax1D(frustumCorners.Select(v => v.X), cameraPosition.X, range);
                var y = DetermineShadowMinMax1D(frustumCorners.Select(v => v.Y), cameraPosition.Y, range);

                var projectionMin = new Vector3(x[0], y[0], minZ);
                var projectionMax = new Vector3(x[1], y[1], maxZ);

                //range += padding;
                //projectionMin.X -= padding / 2.0f;
                //projectionMin.Y -= padding / 2.0f;

                //var qx = (float) Math.IEEERemainder(projectionMin.X, quantizationStep);
                //var qy = (float)Math.IEEERemainder(projectionMin.Y, quantizationStep);

                //projectionMin.X = projectionMin.X - qx;
                //projectionMin.Y = projectionMin.Y - qx;

                //projectionMax.X = projectionMin.X + range;
                //projectionMax.Y = projectionMin.Y + range;

                var projection = Matrix.CreateOrthographicOffCenter(
                    projectionMin.X,
                    projectionMax.X,
                    projectionMin.Y,
                    projectionMax.Y,
                    projectionMin.Z,
                    projectionMax.Z);

                var view = shadowView;
                var viewTranslation = view.Translation;
                viewTranslation.X -= (float)Math.IEEERemainder(viewTranslation.X, 2.0f * range / shadowMapSize);
                viewTranslation.Y -= (float)Math.IEEERemainder(viewTranslation.Y, 2.0f * range / shadowMapSize);
                viewTranslation.Z -= (float)Math.IEEERemainder(viewTranslation.Z, 2.0f * range / shadowMapSize);

                view.Translation = viewTranslation;
                
                return new FrustumSplitProjection(f, projection, view);


                //var n = clipSpaceSplitDistances[i];
                //var f = clipSpaceSplitDistances[i + 1];

                //// get frustum split corners and transform into shadow space
                //var frustumCorners = SplitFrustum(n, f, camera.InverseViewProjection)
                //    .Select(v => Vector3.Transform(v, shadowView))
                //    .ToArray();

                //var bounds = sceneBoundingBox.Transform(shadowView);

                //var x = frustumCorners.Select(c => c.X).Min();
                //var y = frustumCorners.Select(c => c.Y).Min();
                //var min = new Vector3(x, y, -bounds.Max.Z);


                //x = frustumCorners.Select(c => c.X).Max();
                //y = frustumCorners.Select(c => c.Y).Max();
                //var max = new Vector3(x, y, -bounds.Min.Z);

                //var q = 1.0f / shadowMapSize;
                //min.X = (float)Math.Round(min.X / q) * q;
                //min.Y = (float)Math.Round(min.Y / q) * q;
                //max.X = (float)Math.Round(max.X / q) * q;
                //max.Y = (float)Math.Round(max.Y / q) * q;

                ////var quantizationStep = 1.0f / shadowMapSize;
                ////var qx = (float)Math.IEEERemainder(min.X, quantizationStep);
                ////var qy = (float)Math.IEEERemainder(min.Y, quantizationStep);

                ////min.X -= qx;
                ////min.Y -= qy;

                ////max.X -= qx;
                ////max.Y -= qy;

                ////min.X = (float)Math.Floor(min.X);
                ////min.Y = (float)Math.Floor(min.Y);
                ////min.Z = (float)Math.Floor(min.Z);

                ////max.X = (float) Math.Ceiling(max.X);
                ////max.Y = (float)Math.Ceiling(max.Y);
                ////max.Z = (float)Math.Ceiling(max.Z);

                //// return orthographic projection
                //return new FrustumSplitProjection(f, Matrix.CreateOrthographicOffCenter(min.X, max.X, min.Y, max.Y, min.Z, max.Z), shadowView);
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

        public static IEnumerable<float> PracticalSplitScheme(int numSplits, float n, float f)
        {
            for (int i = 0; i < numSplits; ++i)
            {
                float p = ((float)i) / numSplits;
                float c_log = n * (float)System.Math.Pow(f / n, p);
                float c_lin = n + (f - n) * p;

                yield return 0.5f * (c_log + c_lin);
            }

            yield return f;
        }

        public static float[] DetermineShadowMinMax1D(IEnumerable<float> values, float cam, float desiredSize)
        {
            var min = values.Min();
            var max = values.Max();

            if (cam > max)
            {
                return new[] { max - desiredSize, max };
            }
            else if (cam < min)
            {
                return new[] { min, min + desiredSize };
            }
            else
            {
                var currentSize = max - min;
                var l = (cam - min) / currentSize * desiredSize;
                var r = (max - cam) / currentSize * desiredSize;

                return new[]
                {
                    cam - l,
                    cam + r
                };
            }
        }
    }
}

﻿using System;
using Microsoft.Xna.Framework;
using MiniEngine.Graphics.Camera;

namespace MiniEngine.Graphics.Shadows
{
    public sealed class Frustum
    {
        private readonly Vector3[] Corners;

        public Frustum()
        {
            this.Corners = new Vector3[8];
        }

        public BoundingSphere ComputeBounds()
        {
            var center = this.ComputeCenter();

            var radius = 0.0f;
            for (var i = 0; i < 8; i++)
            {
                var dist = Vector3.Distance(this.Corners[i], center);
                radius = Math.Max(dist, radius);
            }

            return new BoundingSphere(center, radius);
        }

        public Vector3 ComputeCenter()
        {
            var center = Vector3.Zero;
            for (var i = 0; i < 8; i++)
            {
                center += this.Corners[i];
            }

            center /= 8;
            return center;
        }

        public void TransformToCameraFrustumInWorldSpace(PerspectiveCamera camera)
        {
            this.ResetToViewVolume();
            var viewProjectionToWorld = Matrix.Invert(camera.ViewProjection);
            this.Transform(viewProjectionToWorld);
        }

        public void Slice(float start, float end)
        {
            for (var i = 0; i < 4; i++)
            {
                // Ray from one of the sides of the frustum that connects a near plane and a far plane corner 
                var ray = this.Corners[i + 4] - this.Corners[i];

                // Slice by moving the corners on the ray
                this.Corners[i + 4] = this.Corners[i] + (ray * end);
                this.Corners[i] = this.Corners[i] + (ray * start);
            }
        }

        private void ResetToViewVolume()
        {
            this.Corners[0] = new Vector3(-1.0f, 1.0f, 0.0f);
            this.Corners[1] = new Vector3(1.0f, 1.0f, 0.0f);
            this.Corners[2] = new Vector3(1.0f, -1.0f, 0.0f);
            this.Corners[3] = new Vector3(-1.0f, -1.0f, 0.0f);
            this.Corners[4] = new Vector3(-1.0f, 1.0f, 1.0f);
            this.Corners[5] = new Vector3(1.0f, 1.0f, 1.0f);
            this.Corners[6] = new Vector3(1.0f, -1.0f, 1.0f);
            this.Corners[7] = new Vector3(-1.0f, -1.0f, 1.0f);
        }

        private void Transform(Matrix matrix)
        {
            for (var i = 0; i < 8; i++)
            {
                var vector = Vector4.Transform(this.Corners[i], matrix);
                this.Corners[i] = new Vector3(vector.X / vector.W, vector.Y / vector.W, vector.Z / vector.W);
            }
        }
    }
}

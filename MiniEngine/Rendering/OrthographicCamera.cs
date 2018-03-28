using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MiniEngine.Mathematics;

namespace MiniEngine.Rendering
{
    public class OrthographicCamera : IViewPoint
    {        
        public OrthographicCamera(float minX, float minY, float maxX, float maxY, float nearZ, float farZ)
        {
            this.MinX = minX;
            this.MinY = minY;
            this.MaxX = maxX;
            this.MaxY = maxY;
            this.NearZ = nearZ;
            this.FarZ = farZ;

            Move(Vector3.Backward * 10, Vector3.Zero);
        }

        public float MinX { get; }
        public float MinY { get; }
        public float MaxX { get; }
        public float MaxY { get; }
        public float NearZ { get; }
        public float FarZ { get; }

        public Vector3 Position { get; private set; }
        public Vector3 LookAt { get; private set; }

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }
        public Matrix ViewProjection { get; private set; }

        public Matrix InverseView { get; private set; }
        public Matrix InverseProjection { get; private set; }
        public Matrix InverseViewProjection { get; private set; }

        public void Move(Vector3 position, Vector3 lookAt)
        {
            this.Position = position;
            this.LookAt = lookAt;

            this.View = Matrix.CreateLookAt(position, lookAt, Vector3.Up);
            this.Projection = Matrix.CreateOrthographicOffCenter(
                this.MinX,
                this.MaxX,
                this.MinY,
                this.MaxY,
                this.NearZ,
                this.FarZ);
            this.ViewProjection = this.View * this.Projection;

            this.InverseView = Matrix.Invert(this.View);
            this.InverseProjection = Matrix.Invert(this.Projection);
            this.InverseViewProjection = Matrix.Invert(this.ViewProjection);
        }

        public void Stabilize(int resolution)
        {            
            var shadowMatrixTemp = this.ViewProjection;
            var shadowOrigin = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            shadowOrigin = Vector4.Transform(shadowOrigin, shadowMatrixTemp);
            shadowOrigin = shadowOrigin * (resolution / 2.0f);

            var roundedOrigin = shadowOrigin.Round();
            var roundOffset = roundedOrigin - shadowOrigin;
            roundOffset = roundOffset * (2.0f / resolution);
            roundOffset.Z = 0.0f;
            roundOffset.W = 0.0f;

            var shadowProj = this.Projection;
            shadowProj.M41 += roundOffset.X;
            shadowProj.M42 += roundOffset.Y;
            shadowProj.M43 += roundOffset.Z;
            shadowProj.M44 += roundOffset.W;

            this.Projection = shadowProj;
        }
    }
}

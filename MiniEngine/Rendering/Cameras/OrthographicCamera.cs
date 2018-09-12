using Microsoft.Xna.Framework;

namespace MiniEngine.Rendering.Cameras
{
    public sealed class OrthographicCamera : IMovableViewPoint
    {
        private readonly float MinX;
        private readonly float MaxX;
        private readonly float MinY;        
        private readonly float MaxY;
        private readonly float NearZ;
        private readonly float FarZ;

        public OrthographicCamera(
            float minX,
            float maxX,
            float minY,            
            float maxY,
            float nearZ,
            float farZ)
        {
            this.MinX  = minX;            
            this.MaxX  = maxX;

            this.MinY  = minY;
            this.MaxY  = maxY;

            this.NearZ = nearZ;
            this.FarZ  = farZ;

            Move(Vector3.Backward * 10, Vector3.Zero);
        }

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }
        public BoundingFrustum Frustum { get; private set; }

        public void Move(Vector3 position, Vector3 lookAt)
        {
            this.View = Matrix.CreateLookAt(position, lookAt, Vector3.Up);
            this.Projection = Matrix.CreateOrthographicOffCenter(
                this.MinX,
                this.MaxX,
                this.MinY,
                this.MaxY,
                this.NearZ,
                this.FarZ);

            this.Frustum = new BoundingFrustum(this.View * this.Projection);
        }       
    }
}

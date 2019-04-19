using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Primitives.VertexTypes;

namespace MiniEngine.Primitives
{
    /// <summary>
    /// A Quad that can be wrapped around 3D objects covering them entirely in screen coordinates
    /// </summary>
    public sealed class WrappableQuad
    {
        private readonly GraphicsDevice Device;
        private readonly GBufferVertex[] Vertices;
        private readonly short[] Indices;
        private readonly short[] LineIndices;
        
        private Vector3[] Corners;

        private float minX;
        private float maxX;
        private float minY;
        private float maxY;

        public WrappableQuad(GraphicsDevice device)
        {
            this.Device = device;

            this.Vertices = new[]
            {
                new GBufferVertex(
                    new Vector3(-1, -1, 0),
                    new Vector2(0, 1)),
                new GBufferVertex(
                    new Vector3(-1, 1, 0),
                    new Vector2(0, 0)),                                
                new GBufferVertex(
                    new Vector3(1, -1, 0),
                    new Vector2(1, 1)),
                    new GBufferVertex(
                    new Vector3(1, 1, 0),
                    new Vector2(1, 0)),
            };

            this.Indices = new short[] { 0, 1, 2, 3};
            this.LineIndices = new short[] { 0, 1, 3, 2, 0 };
            this.Corners = new Vector3[BoundingBox.CornerCount];
        }

        public void WrapOnScreen(BoundingBox bounds, PerspectiveCamera camera)
        {
            bounds.GetCorners(this.Corners);
            this.WrapOnScreen(this.Corners, camera);
        }

        public void WrapOnScreen(BoundingFrustum frustum, PerspectiveCamera camera)
        {
            frustum.GetCorners(this.Corners);
            //this.Corners = GetCorners(frustum, camera.Frustum, camera.Forward * 0.01f);
            this.WrapOnScreen(this.Corners, camera);
        }

        public void Reset()
        {
            this.minX = -1.0f;
            this.maxX = 1.0f;

            this.minY = -1.0f;
            this.maxY = 1.0f;

            this.Vertices[0].Position = new Vector4(this.minX, this.minY, 0, 1);
            this.Vertices[0].TextureCoordinate = ProjectionMath.ToUv(this.minX, this.minY);

            this.Vertices[1].Position = new Vector4(this.minX, this.maxY, 0, 1);
            this.Vertices[1].TextureCoordinate = ProjectionMath.ToUv(this.minX, this.maxY);

            this.Vertices[2].Position = new Vector4(this.maxX, this.minY, 0, 1);
            this.Vertices[2].TextureCoordinate = ProjectionMath.ToUv(this.maxX, this.minY);

            this.Vertices[3].Position = new Vector4(this.maxX, this.maxY, 0, 1);
            this.Vertices[3].TextureCoordinate = ProjectionMath.ToUv(this.maxX, this.maxY);
        }

        /// <summary>
        /// Computes the projected coordinates of the corners and then wraps
        /// the quad around them on screen. Scales the UV coordinates accordingly
        /// </summary>
        public void WrapOnScreen(Vector3[] corners, PerspectiveCamera camera)
        {         
            //Clip(corners, camera.Frustum);            

            this.minX = float.MaxValue;
            this.maxX = float.MinValue;

            this.minY = float.MaxValue;
            this.maxY = float.MinValue;

            for (var i = 0; i < corners.Length; i++)
            {
                var corner = corners[i];
                
                var projectedCorner = ProjectionMath.WorldToView(corner, camera.ViewProjection);

                if (IsBehindCamera(corner, camera))
                {                    
                    projectedCorner.X = -projectedCorner.X;
                    projectedCorner.Y = -projectedCorner.Y;
                }

                this.minX = Math.Min(this.minX, projectedCorner.X);
                this.maxX = Math.Max(this.maxX, projectedCorner.X);

                this.minY = Math.Min(this.minY, projectedCorner.Y);
                this.maxY = Math.Max(this.maxY, projectedCorner.Y);
            }

            this.Vertices[0].Position = new Vector4(this.minX, this.minY, 0, 1);
            this.Vertices[0].TextureCoordinate = ProjectionMath.ToUv(this.minX, this.minY);

            this.Vertices[1].Position = new Vector4(this.minX, this.maxY, 0, 1);
            this.Vertices[1].TextureCoordinate = ProjectionMath.ToUv(this.minX, this.maxY);

            this.Vertices[2].Position = new Vector4(this.maxX, this.minY, 0, 1);
            this.Vertices[2].TextureCoordinate = ProjectionMath.ToUv(this.maxX, this.minY);

            this.Vertices[3].Position = new Vector4(this.maxX, this.maxY, 0, 1);
            this.Vertices[3].TextureCoordinate = ProjectionMath.ToUv(this.maxX, this.maxY);
        }

        //public static Vector3[] GetCorners(BoundingFrustum boundingFrustum, BoundingFrustum cameraFrustum, Vector3 forward)
        //{
        //    //const int nearTopLeft = 0;
        //    //const int nearTopRight = 1;
        //    //const int nearBottomRight = 2;
        //    //const int nearBottomLeft = 3;
        //    //const int farTopLeft = 4;
        //    //const int farTopRight = 5;
        //    //const int farBottomRight = 6;
        //    //const int farBottomLeft = 7;

        //    var boundCorners = boundingFrustum.GetCorners();

        //    foreach(var corner in boundCorners)
        //    {

        //    }

        //    cameraFrustum.Intersects(boundingFrustum.Near) == PlaneIntersectionType.

        //}


        public static bool IsReal(float f)
        {            
            return !(float.IsNaN(f) || float.IsInfinity(f));
        }


        private static Plane[] GetPlanes(BoundingFrustum frustum)
        {
            return new Plane[]
            {
                frustum.Near,
                frustum.Far,
                frustum.Left,
                frustum.Right,
                frustum.Top,
                frustum.Bottom
            };
        }

        private static Vector3 IntersectionPoint(Plane a, Plane b, Plane c)
        {
            Vector3.Cross(ref b.Normal, ref c.Normal, out Vector3 vector);
            Vector3.Dot(ref a.Normal, ref vector, out float num);
            num *= -1f;
            Vector3.Cross(ref b.Normal, ref c.Normal, out vector);
            Vector3.Multiply(ref vector, a.D, out Vector3 vector2);
            Vector3.Cross(ref c.Normal, ref a.Normal, out vector);
            Vector3.Multiply(ref vector, b.D, out Vector3 vector3);
            Vector3.Cross(ref a.Normal, ref b.Normal, out vector);
            Vector3.Multiply(ref vector, c.D, out Vector3 vector4);

            return new Vector3
            (
                (vector2.X + vector3.X + vector4.X) / num,
                (vector2.Y + vector3.Y + vector4.Y) / num,
                (vector2.Z + vector3.Z + vector4.Z) / num
            );
        }


        private static void Clip(Vector3[] corners, BoundingFrustum frustum)
        {

           
            
            
            
            
            
            
            
            
            
            // I have to somehow clip the bounding frustum, min doesnt work as points can lie outside the bounding frustum but inside 
            // the axis aligned bounding box of the frustum.


            //var frustumCorners = frustum.GetCorners();

            //var minX = float.MaxValue;
            //var minY = float.MaxValue;
            //var minZ = float.MaxValue;

            //var maxX = float.MinValue;
            //var maxY = float.MinValue;
            //var maxZ = float.MinValue;

            //for (var i = 0; i < frustumCorners.Length; i++)
            //{
            //    var corner = frustumCorners[i];
            //    minX = Math.Min(minX, corner.X);
            //    minY = Math.Min(minY, corner.Y);
            //    minZ = Math.Min(minZ, corner.Z);

            //    maxX = Math.Max(maxX, corner.X);
            //    maxY = Math.Max(maxY, corner.Y);
            //    maxZ = Math.Max(maxZ, corner.Z);
            //}

            //for(var i = 0; i < corners.Length; i++)
            //{
            //    var corner = corners[i];

            //    corner.X = MathHelper.Clamp(corner.X, minX, maxX);
            //    corner.Y = MathHelper.Clamp(corner.Y, minY, maxY);
            //    corner.Z = MathHelper.Clamp(corner.Z, minZ, maxZ);

            //    if (corners[i] != corner)
            //    {
            //        corners[i] = corner;
            //    }
            //}

            //var nearTopLeft     = frustumCorners[0];
            //var nearTopRight    = frustumCorners[1];
            //var nearBottomRight = frustumCorners[2];
            //var nearBottomLeft  = frustumCorners[3];
            //var farTopLeft      = frustumCorners[4];
            //var farTopRight     = frustumCorners[5];
            //var farBottomRight  = frustumCorners[6];
            //var farBottomLeft   = frustumCorners[7];


            //for (var i = 0; i < corners.Length; i++)
            //{
            //    var corner = corners[i];


            //    // 0 NEAR
            //    // 1 FAR
            //    // 2 LEFT
            //    // 3 RIGHT
            //    // 4 TOP
            //    // 5 BOTTOM

            //    //IntersectionPoint(ref NEAR, ref LEFT, ref TOP, out _corners[0]); // NEAR TOP LEFT
            //    //IntersectionPoint(ref NEAR, ref RIGHT, ref TOP, out _corners[1]);
            //    //IntersectionPoint(ref NEAR, ref RIGHT, ref BOTTOM, out _corners[2]);
            //    //IntersectionPoint(ref NEAR, ref LEFT, ref BOTTOM, out _corners[3]);
            //    //IntersectionPoint(ref FAR, ref LEFT, ref TOP, out _corners[4]);
            //    //IntersectionPoint(ref FAR, ref RIGHT, ref TOP, out _corners[5]);
            //    //IntersectionPoint(ref FAR, ref RIGHT, ref BOTTOM, out _corners[6]);
            //    //IntersectionPoint(ref FAR, ref LEFT, ref BOTTOM, out _corners[7]);


            //    // The planes of a bounding frustum all point outward                                
            //    if (frustum.Contains(corner) == ContainmentType.Disjoint)
            //    {
            //        if(InFrontOf(frustum.Left, corner))
            //        {

            //        }


            //        //// Near Top Left
            //        //if(InFrontOf(frustum.Near, corner) && InFrontOf(frustum.Top, corner) && InFrontOf(frustum.Left, corner))
            //        //{
            //        //    corners[i] = frustumCorners[0];
            //        //}
            //        //// Near Top Right
            //        //else if (InFrontOf(frustum.Near, corner) && InFrontOf(frustum.Top, corner) && InFrontOf(frustum.Right, corner))
            //        //{
            //        //    corners[i] = frustumCorners[1];
            //        //}
            //        //// Near Bottom Right
            //        //else if (InFrontOf(frustum.Near, corner) && InFrontOf(frustum.Bottom, corner) && InFrontOf(frustum.Right, corner))
            //        //{
            //        //    corners[i] = frustumCorners[2];
            //        //}
            //        //// Near Bottom Left
            //        //else if (InFrontOf(frustum.Near, corner) && InFrontOf(frustum.Bottom, corner) && InFrontOf(frustum.Left, corner))
            //        //{
            //        //    corners[i] = frustumCorners[3];
            //        //}

            //        //// Far Top Left
            //        //if (InFrontOf(frustum.Far, corner) && InFrontOf(frustum.Top, corner) && InFrontOf(frustum.Left, corner))
            //        //{
            //        //    corners[i] = frustumCorners[4];
            //        //}
            //        //// Far Top Right
            //        //else if (InFrontOf(frustum.Far, corner) && InFrontOf(frustum.Top, corner) && InFrontOf(frustum.Right, corner))
            //        //{
            //        //    corners[i] = frustumCorners[5];
            //        //}
            //        //// Far Bottom Right
            //        //else if (InFrontOf(frustum.Far, corner) && InFrontOf(frustum.Bottom, corner) && InFrontOf(frustum.Right, corner))
            //        //{
            //        //    corners[i] = frustumCorners[6];
            //        //}
            //        //// Far Bottom Left
            //        //else if (InFrontOf(frustum.Far, corner) && InFrontOf(frustum.Bottom, corner) && InFrontOf(frustum.Left, corner))
            //        //{
            //        //    corners[i] = frustumCorners[7];
            //        //}
            //    }
            //}
        }

        private static bool InFrontOf(Plane plane, Vector3 point) 
            => ((point.X * plane.Normal.X) + (point.Y * plane.Normal.Y) + (point.Z * plane.Normal.Z) + plane.D) > 0;

        private static bool IsBehindCamera(Vector3 corner, PerspectiveCamera camera)
        {
            var cornerDirection = Vector3.Normalize(corner - camera.Position );
            var dot = Vector3.Dot(camera.Forward, cornerDirection);
           
            return dot < 0;
        }
       
        public void Render() 
            => this.Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, this.Vertices, 0, 4, this.Indices, 0, 2);

        
        public void RenderOutline()
            => this.Device.DrawUserIndexedPrimitives(PrimitiveType.LineStrip, this.Vertices, 0, 4, this.LineIndices, 0, 4);



        public void Render(BoundingFrustum frustum)
        {
            frustum.GetCorners(this.Corners);

            var nearTopLeft     = this.Corners[0];
            var nearTopRight    = this.Corners[1];
            var nearBottomRight = this.Corners[2];
            var nearBottomLeft  = this.Corners[3];
            var farTopLeft      = this.Corners[4];
            var farTopRight     = this.Corners[5];
            var farBottomRight  = this.Corners[6];
            var farBottomLeft   = this.Corners[7];

            var uvNearTopLeft     = new Vector2(0, 0);
            var uvNearTopRight    = new Vector2(1, 0);
            var uvNearBottomRight = new Vector2(1, 1);
            var uvNearBottomLeft  = new Vector2(0, 1);

            var uvFarTopLeft      = new Vector2(0, 0);
            var uvFarTopRight     = new Vector2(1, 0);
            var uvFarBottomRight  = new Vector2(1, 1);
            var uvFarBottomLeft   = new Vector2(0, 1);

            var vertices = new GBufferVertex[]
            {
                // Far plane
                new GBufferVertex(farBottomLeft, uvFarBottomLeft),
                new GBufferVertex(farTopLeft, uvFarTopLeft),
                new GBufferVertex(farTopRight, uvFarTopRight),
                new GBufferVertex(farBottomRight, uvFarBottomRight),

                // Left plane
                new GBufferVertex(nearBottomLeft, uvNearBottomLeft),
                new GBufferVertex(nearTopLeft, uvNearTopLeft),
                new GBufferVertex(farTopLeft, uvFarTopLeft),
                new GBufferVertex(farBottomLeft, uvFarBottomLeft),

                // Right plane
                new GBufferVertex(farBottomRight, uvFarBottomRight),
                new GBufferVertex(farTopRight, uvFarTopRight),
                new GBufferVertex(nearTopRight, uvNearTopRight),
                new GBufferVertex(nearBottomRight, uvNearBottomRight),

                // Top plane
                new GBufferVertex(nearTopRight, uvNearTopRight),
                new GBufferVertex(farTopRight, uvFarTopRight),
                new GBufferVertex(farTopLeft, uvFarTopLeft),
                new GBufferVertex(nearTopLeft, uvNearTopLeft),
                                              
                // Bottom plane
                new GBufferVertex(nearBottomLeft, uvNearBottomLeft),
                new GBufferVertex(farBottomLeft, uvFarBottomLeft),
                new GBufferVertex(farBottomRight, uvFarBottomRight),
                new GBufferVertex(nearBottomRight, uvNearBottomRight),
            };

            var indices = new short[]
            {
                0, 1, 2,
                2, 3, 0,

                4, 5, 6,
                6, 7, 4,

                8, 9, 10,
                10, 11, 8,

                12, 13, 14,
                14, 15, 12,

                16, 17, 18,
                18, 19, 16
            };

            this.Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);


            //    new GBufferVertex(
            //        new Vector3(-1, -1, 0),
            //        new Vector2(0, 1)),
            //    new GBufferVertex(
            //        new Vector3(-1, 1, 0),
            //        new Vector2(0, 0)),                                
            //    new GBufferVertex(
            //        new Vector3(1, -1, 0),
            //        new Vector2(1, 1)),
            //    new GBufferVertex(
            //        new Vector3(1, 1, 0),
            //        new Vector2(1, 0)),
        }
    }
}

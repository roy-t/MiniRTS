using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Shadows.Utilities;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Pipeline.Shadows.Components
{
    public sealed class CascadeInfo
    {
        public CascadeInfo(Vector3 position, Vector3 lookAt, int cascades, int resolution, float[] cascadeDistances)
        {
            this.CascadeSplits = new float[cascades];
            this.CascadeOffsets = new Vector4[cascades];
            this.CascadeScales = new Vector4[cascades];
            this.GlobalShadowMatrix = Matrix.Identity;

            this.Move(position, lookAt);
            this.Resolution = resolution;
            this.CascadeDistances = cascadeDistances;

            this.ShadowCameras = Reference<IViewPoint>.CreateEmpty(cascades);
        }

        public Reference<IViewPoint>[] ShadowCameras { get; }

        public float[] CascadeSplits { get; }
        public Vector4[] CascadeOffsets { get; }
        public Vector4[] CascadeScales { get; }
        public Matrix GlobalShadowMatrix { get; set; }
        
        public Vector3 Position { get; private set; }
        public Vector3 LookAt { get; private set; }
        public Vector3 SurfaceToLightVector { get; private set; }
        
        public int Resolution { get; }
        public float[] CascadeDistances { get; }

        public void Move(Vector3 position, Vector3 lookAt)
        {
            this.Position = position;
            this.LookAt = lookAt;
            this.SurfaceToLightVector = Vector3.Normalize(position - lookAt);
        }
    }
}

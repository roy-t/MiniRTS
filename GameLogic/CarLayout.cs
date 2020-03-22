using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Models.Components;
using ModelExtension;

namespace MiniEngine.GameLogic
{
    public sealed class CarLayout
    {
        private readonly AModel Model;
        private readonly Matrix[] Bones;

        public CarLayout(AModel model)
        {
            this.Model = model;
            this.Bones = new Matrix[model.Model.Bones.Count];
            model.Model.CopyAbsoluteBoneTransformsTo(this.Bones);
        }

        /// <summary>
        /// Calculates a matrix that moves you to the reference frame where the origin is where the wheel is connected to the vehicle
        /// </summary>
        public Matrix GetToWheelConnection(WheelPosition wheel)
        {
            var name = WheelNameLookUp.GetCarWheelMeshName(wheel);
            var mesh = this.GetMeshMatrix(name);

            var world = this.Model.Pose.Matrix;
            return mesh * world;
        }

        /// <summary>
        /// Calculates a matrix that moves you to the reference frame where of the wheel
        /// </summary>
        public Matrix GetToWheel(WheelPosition wheel)
        {
            var toWheelMesh = this.GetToWheelConnection(wheel);

            var skinBoneName = WheelNameLookUp.GetCarWheelSkinBoneName(wheel);
            var skinBoneMatrix = this.GetSkinBoneMatrix(skinBoneName);

            return skinBoneMatrix * toWheelMesh;
        }

        public float GetWheelRadius(WheelPosition wheel)
        {
            var name = WheelNameLookUp.GetCarWheelMeshName(wheel);
            var mesh = this.GetMesh(name);

            // TODO: this should probably also take into account the scale in the absolutebone transforms
            return mesh.BoundingSphere.Radius * this.Model.Scale.X;
        }

        public Vector3 GetWheelPosition(WheelPosition wheel)
        {
            var matrix = this.GetToWheel(wheel);
            return Vector3.Transform(Vector3.Zero, matrix);
        }

        public Vector3 GetFrontAxlePosition()
        {
            var toFrontLeft = this.GetToWheelConnection(WheelPosition.FrontLeft);
            var toFrontRight = this.GetToWheelConnection(WheelPosition.FrontRight);

            var a = Vector3.Transform(Vector3.Zero, toFrontLeft);
            var b = Vector3.Transform(Vector3.Zero, toFrontRight);

            return Vector3.Lerp(a, b, 0.5f);
        }

        public Vector3 GetRearAxlePosition()
        {
            var toRearLeft = this.GetToWheelConnection(WheelPosition.RearLeft);
            var toRearRight = this.GetToWheelConnection(WheelPosition.RearRight);

            var a = Vector3.Transform(Vector3.Zero, toRearLeft);
            var b = Vector3.Transform(Vector3.Zero, toRearRight);

            return Vector3.Lerp(a, b, 0.5f);
        }

        private Matrix GetMeshMatrix(string meshName)
        {
            var mesh = this.GetMesh(meshName);
            return this.Bones[mesh.ParentBone.Index];
        }

        private ModelMesh GetMesh(string meshName)
        {
            for (var i = 0; i < this.Model.Model.Meshes.Count; i++)
            {
                var mesh = this.Model.Model.Meshes[i];
                if (mesh.Name.Equals(meshName, StringComparison.OrdinalIgnoreCase))
                {
                    return mesh;
                }
            }

            throw new Exception($"Mesh {meshName} not found");
        }

        private Matrix GetSkinBoneMatrix(string boneName)
        {
            var skinningData = this.Model.Model.Tag as SkinningData;

            for (var i = 0; i < skinningData.BoneNames.Count; i++)
            {
                if (skinningData.BoneNames[i].Equals(boneName, StringComparison.OrdinalIgnoreCase))
                {
                    var transforms = this.Model.Animation.GetSkinTransforms();
                    return transforms[i];
                }
            }

            return Matrix.Identity;
        }
    }
}

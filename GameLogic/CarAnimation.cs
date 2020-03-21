using System;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Units;
using ModelExtension;

namespace MiniEngine.GameLogic
{
    public class CarAnimation : AAnimation
    {
        private SkinningData skinningData;
        private Matrix[] boneTransforms;
        private Matrix[] worldTransforms;
        private Matrix[] skinTransforms;

        public CarAnimation() : base() { }

        public override void SetTarget(AModel target)
        {
            if ((target.Model.Tag as SkinningData) == null)
            {
                throw new ArgumentException("Target does not have skinning data ", nameof(target));
            }

            this.skinningData = target.Model.Tag as SkinningData;

            this.boneTransforms = new Matrix[this.skinningData.BindPose.Count];
            this.worldTransforms = new Matrix[this.skinningData.BindPose.Count];
            this.skinTransforms = new Matrix[this.skinningData.BindPose.Count];

            this.WheelRoll = new float[4];
            this.WheelYaw = new float[4];

            base.SetTarget(target);
        }

        public void Update(Seconds elapsed)
        {
            this.skinningData.BindPose.CopyTo(this.boneTransforms);
            this.worldTransforms[0] = this.boneTransforms[0] * Matrix.Identity;

            for (var bone = 1; bone < this.worldTransforms.Length; bone++)
            {
                var parentBone = this.skinningData.SkeletonHierarchy[bone];
                var worldTransform = this.boneTransforms[bone] * this.worldTransforms[parentBone];

                if (this.IsWheel(bone))
                {
                    var wheelIndex = GetWheelIndex(bone);
                    var wheelMatrix = Matrix.CreateRotationY(this.WheelRoll[wheelIndex]) * Matrix.CreateRotationZ(this.WheelYaw[wheelIndex]);
                    worldTransform = wheelMatrix * worldTransform;
                }

                this.worldTransforms[bone] = worldTransform;
            }

            for (var bone = 0; bone < this.skinTransforms.Length; bone++)
            {
                this.skinTransforms[bone] = this.skinningData.InverseBindPose[bone] * this.worldTransforms[bone];
            }

            Array.Copy(this.skinTransforms, 0, this.SkinTransforms, 0, this.skinTransforms.Length);
        }

        private int GetWheelIndex(int bone)
        {
            if (this.IsFrontLeftWheel(bone))
            {
                return (int)WheelPosition.FrontLeft;
            }

            if (this.IsFrontRightWheel(bone))
            {
                return (int)WheelPosition.FrontRight;
            }

            if (this.IsRearLeftWheel(bone))
            {
                return (int)WheelPosition.RearLeft;
            }

            if (this.IsRearRightWheel(bone))
            {
                return (int)WheelPosition.RearRight;
            }

            throw new Exception($"Could not find wheel index for bone {bone}");
        }

        public float[] WheelRoll { get; set; }
        public float[] WheelYaw { get; set; }

        private bool IsWheel(int bone) =>
            this.IsFrontLeftWheel(bone) ||
            this.IsFrontRightWheel(bone) ||
            this.IsRearLeftWheel(bone) ||
            this.IsRearRightWheel(bone);

        private bool IsFrontLeftWheel(int bone) => this.skinningData.BoneNames[bone].Equals(WheelNameLookUp.GetCarWheelSkinBoneName(WheelPosition.FrontLeft), StringComparison.OrdinalIgnoreCase);
        private bool IsFrontRightWheel(int bone) => this.skinningData.BoneNames[bone].Equals(WheelNameLookUp.GetCarWheelSkinBoneName(WheelPosition.FrontRight), StringComparison.OrdinalIgnoreCase);
        private bool IsRearLeftWheel(int bone) => this.skinningData.BoneNames[bone].Equals(WheelNameLookUp.GetCarWheelSkinBoneName(WheelPosition.RearLeft), StringComparison.OrdinalIgnoreCase);
        private bool IsRearRightWheel(int bone) => this.skinningData.BoneNames[bone].Equals(WheelNameLookUp.GetCarWheelSkinBoneName(WheelPosition.RearRight), StringComparison.OrdinalIgnoreCase);
    }
}

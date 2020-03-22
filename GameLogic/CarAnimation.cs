using System;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Systems;
using MiniEngine.Units;

namespace MiniEngine.GameLogic
{
    public sealed class CarAnimation : AAnimation
    {
        private readonly Matrix[] BoneTransforms;
        private readonly Matrix[] WorldTransforms;
        private readonly Matrix[] SkinTransforms;

        public CarAnimation(Entity entity, AModel model)
            : base(entity, model)
        {
            this.BoneTransforms = new Matrix[this.SkinningData.BindPose.Count];
            this.WorldTransforms = new Matrix[this.SkinningData.BindPose.Count];
            this.SkinTransforms = new Matrix[this.SkinningData.BindPose.Count];

            this.WheelRoll = new float[4];
            this.WheelYaw = new float[4];
        }

        public override void Update(Seconds elapsed)
        {
            this.SkinningData.BindPose.CopyTo(this.BoneTransforms);
            this.WorldTransforms[0] = this.BoneTransforms[0] * Matrix.Identity;

            for (var bone = 1; bone < this.WorldTransforms.Length; bone++)
            {
                var parentBone = this.SkinningData.SkeletonHierarchy[bone];
                var worldTransform = this.BoneTransforms[bone] * this.WorldTransforms[parentBone];

                if (this.IsWheel(bone))
                {
                    var wheelIndex = GetWheelIndex(bone);
                    var wheelMatrix = Matrix.CreateRotationY(this.WheelRoll[wheelIndex]) * Matrix.CreateRotationZ(this.WheelYaw[wheelIndex]);
                    worldTransform = wheelMatrix * worldTransform;
                }

                this.WorldTransforms[bone] = worldTransform;
            }

            for (var bone = 0; bone < this.SkinTransforms.Length; bone++)
            {
                this.SkinTransforms[bone] = this.SkinningData.InverseBindPose[bone] * this.WorldTransforms[bone];
            }

            this.CopySkinTransformsToModel(this.SkinTransforms);
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

        private bool IsFrontLeftWheel(int bone) => this.SkinningData.BoneNames[bone].Equals(WheelNameLookUp.GetCarWheelSkinBoneName(WheelPosition.FrontLeft), StringComparison.OrdinalIgnoreCase);
        private bool IsFrontRightWheel(int bone) => this.SkinningData.BoneNames[bone].Equals(WheelNameLookUp.GetCarWheelSkinBoneName(WheelPosition.FrontRight), StringComparison.OrdinalIgnoreCase);
        private bool IsRearLeftWheel(int bone) => this.SkinningData.BoneNames[bone].Equals(WheelNameLookUp.GetCarWheelSkinBoneName(WheelPosition.RearLeft), StringComparison.OrdinalIgnoreCase);
        private bool IsRearRightWheel(int bone) => this.SkinningData.BoneNames[bone].Equals(WheelNameLookUp.GetCarWheelSkinBoneName(WheelPosition.RearRight), StringComparison.OrdinalIgnoreCase);
    }
}

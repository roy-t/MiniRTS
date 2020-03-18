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

        private float accumulator;

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

            base.SetTarget(target);
        }

        public void Update(Seconds elapsed)
        {
            this.accumulator += elapsed;

            this.skinningData.BindPose.CopyTo(this.boneTransforms);
            this.worldTransforms[0] = this.boneTransforms[0] * Matrix.Identity;

            for (var bone = 1; bone < this.worldTransforms.Length; bone++)
            {
                var parentBone = this.skinningData.SkeletonHierarchy[bone];
                var worldTransform = this.boneTransforms[bone] * this.worldTransforms[parentBone];

                if (this.IsFrontLeftWheel(bone))
                {
                    var wheelMatrix = Matrix.CreateRotationZ(this.FrontLeftWheelYaw);
                    worldTransform = wheelMatrix * worldTransform;
                }
                else if (this.IsFrontRightWheel(bone))
                {
                    var wheelMatrix = Matrix.CreateRotationZ(this.FrontRightWheelYaw);
                    worldTransform = wheelMatrix * worldTransform;
                }
                else if (this.IsRearLeftWheel(bone))
                {
                    // Y axis as this is in the space of the wheel, not the world
                    var wheelMatrix = Matrix.CreateRotationY(MathHelper.TwoPi * this.accumulator * 0.25f);
                    worldTransform = wheelMatrix * worldTransform;
                }
                else if (this.IsRearRightWheel(bone))
                {

                }

                this.worldTransforms[bone] = worldTransform;
            }

            for (var bone = 0; bone < this.skinTransforms.Length; bone++)
            {
                this.skinTransforms[bone] = this.skinningData.InverseBindPose[bone] *
                                            this.worldTransforms[bone];
            }

            Array.Copy(this.skinTransforms, 0, this.SkinTransforms, 0, this.skinTransforms.Length);
        }


        public float FrontLeftWheelYaw { get; set; }
        public float FrontRightWheelYaw { get; set; }

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

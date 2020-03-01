using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Units;
using ModelExtension;

namespace MiniEngine.Scenes.Animations
{
    public class CarAnimation : AAnimation
    {
        private static readonly IReadOnlyList<string> Wheels = new List<string>
        {
            "Bone_FL",
            "Bone_FR",
            "Bone_RL",
            "Bone_RR"
        };

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
            var wheelMatrix = Matrix.CreateRotationY(MathHelper.TwoPi * this.accumulator * 0.25f);

            this.skinningData.BindPose.CopyTo(this.boneTransforms);
            this.worldTransforms[0] = this.boneTransforms[0] * Matrix.Identity;

            for (var bone = 1; bone < this.worldTransforms.Length; bone++)
            {
                var parentBone = this.skinningData.SkeletonHierarchy[bone];
                if (this.IsWheel(bone))
                {
                    this.worldTransforms[bone] = wheelMatrix * this.boneTransforms[bone] * this.worldTransforms[parentBone];
                }
                else
                {
                    this.worldTransforms[bone] = this.boneTransforms[bone] * this.worldTransforms[parentBone];
                }
            }

            for (var bone = 0; bone < this.skinTransforms.Length; bone++)
            {
                this.skinTransforms[bone] = this.skinningData.InverseBindPose[bone] *
                                            this.worldTransforms[bone];
            }

            Array.Copy(this.skinTransforms, 0, this.BoneTransforms, 0, this.skinTransforms.Length);
        }

        private bool IsWheel(int bone) =>
            this.IsFrontLeftWheel(bone) ||
            this.IsFrontRightWheel(bone) ||
            this.IsRearLeftWheel(bone) ||
            this.IsRearRightWheel(bone);

        private bool IsFrontLeftWheel(int bone) => this.skinningData.BoneNames[bone] == Wheels[0];
        private bool IsFrontRightWheel(int bone) => this.skinningData.BoneNames[bone] == Wheels[1];
        private bool IsRearLeftWheel(int bone) => this.skinningData.BoneNames[bone] == Wheels[2];
        private bool IsRearRightWheel(int bone) => this.skinningData.BoneNames[bone] == Wheels[3];
    }
}

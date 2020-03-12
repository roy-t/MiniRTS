using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Units;
using ModelExtension;

namespace MiniEngine.Pipeline.Models.Components
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

            // Y axis as this is in the space of the wheel, not the world
            var wheelMatrix = Matrix.CreateRotationY(MathHelper.TwoPi * this.accumulator * 0.25f);

            this.skinningData.BindPose.CopyTo(this.boneTransforms);
            this.worldTransforms[0] = this.boneTransforms[0] * Matrix.Identity;

            for (var bone = 1; bone < this.worldTransforms.Length; bone++)
            {
                var parentBone = this.skinningData.SkeletonHierarchy[bone];
                if (bone == 2/*this.IsWheel(bone)*/)
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

        public int GetBoneIndex(string boneName) => this.skinningData.BoneNames.IndexOf(boneName);

        public Matrix GetTo(string boneName)
        {
            // TODO: see DrawModel how we should combine BoneTransforms to get the the real to!
            //    this.Effect.World = SharedBoneMatrix[mesh.ParentBone.Index] * world;
            // maybe we should incorporate those absolute bone transforms into the animation??


            // TODO: the root bone of the mesh gets the wheel in the right place
            // while the skinning bone makes it possible to animate it
            // To get the world: toWorldBonePosition = bone[x] * modelWorld

            // TODO: figure out how to identify the 'mesh bone' of skeletal bone
            var bones = new Matrix[this.Target.Model.Bones.Count];
            this.Target.Model.CopyAbsoluteBoneTransformsTo(bones);


            var index = GetBoneIndex(boneName);
            return bones[5];// * this.worldTransforms[index];
            //return this.skinningData.BindPose[index];
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

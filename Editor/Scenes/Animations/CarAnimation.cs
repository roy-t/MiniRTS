using System;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Components;
using ModelExtension;

namespace MiniEngine.Scenes.Animations
{
    public class CarAnimation : AAnimation
    {
        public CarAnimation() : base() { }

        public override void SetTarget(AModel target)
        {
            if ((target.Model.Tag as SkinningData) == null)
            {
                throw new ArgumentException("Target does not have skinning data ", nameof(target));
            }
            base.SetTarget(target);
        }

        public void Update()
        {
            var skinningData = this.Target.Model.Tag as SkinningData;

            var boneTransforms = new Matrix[skinningData.BindPose.Count];
            var worldTransforms = new Matrix[skinningData.BindPose.Count];
            var skinTransforms = new Matrix[skinningData.BindPose.Count];

            skinningData.BindPose.CopyTo(boneTransforms);

            worldTransforms[0] = boneTransforms[0] * Matrix.Identity;

            for (var bone = 1; bone < worldTransforms.Length; bone++)
            {
                var parentBone = skinningData.SkeletonHierarchy[bone];


                var fl = skinningData.GetIndex("Bone_FL");
                if (bone == fl)
                {
                    worldTransforms[bone] = Matrix.CreateTranslation(-100, 0, 0) * boneTransforms[bone] * worldTransforms[parentBone];
                }
                else
                {
                    worldTransforms[bone] = boneTransforms[bone] * worldTransforms[parentBone];
                }
            }

            for (int bone = 0; bone < skinTransforms.Length; bone++)
            {
                skinTransforms[bone] = skinningData.InverseBindPose[bone] *
                                            worldTransforms[bone];
            }

            Array.Copy(skinTransforms, 0, this.BoneTransforms, 0, skinTransforms.Length);
        }
    }
}

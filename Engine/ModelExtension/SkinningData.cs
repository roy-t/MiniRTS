using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ModelExtension
{
    public class SkinningData
    {
        public SkinningData(List<Matrix> bindPose, List<Matrix> inverseBindPose, List<int> skeletonHierarchy, List<string> boneNames)
        {
            this.BindPose = bindPose;
            this.InverseBindPose = inverseBindPose;
            this.SkeletonHierarchy = skeletonHierarchy;
            this.BoneNames = boneNames;
        }

        private SkinningData()
        {
        }

        [ContentSerializer]
        public List<Matrix> BindPose { get; private set; }

        [ContentSerializer]
        public List<Matrix> InverseBindPose { get; private set; }

        [ContentSerializer]
        public List<int> SkeletonHierarchy { get; private set; }

        [ContentSerializer]
        public List<string> BoneNames { get; private set; }


        public int GetIndex(string boneName) => this.BoneNames.IndexOf(boneName);
    }
}

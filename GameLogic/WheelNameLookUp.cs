using System;

namespace MiniEngine.GameLogic
{
    public static class WheelNameLookUp
    {
        private const string BoneFL = "BONE_FL";
        private const string BoneFR = "BONE_FR";
        private const string BoneRL = "BONE_RL";
        private const string BoneRR = "BONE_RR";

        private const string WheelMeshFL = "WHEEL_FL";
        private const string WheelMeshFR = "WHEEL_FR";
        private const string WheelMeshRL = "WHEEL_RL";
        private const string WheelMeshRR = "WHEEL_RR";

        public static string GetCarWheelMeshName(WheelPosition wheelType)
        {
            switch (wheelType)
            {
                case WheelPosition.FrontLeft:
                    return WheelMeshFL;
                case WheelPosition.FrontRight:
                    return WheelMeshFR;
                case WheelPosition.RearLeft:
                    return WheelMeshRL;
                case WheelPosition.RearRight:
                    return WheelMeshRR;
                default:
                    throw new ArgumentException($"Unknown wheel type: {wheelType}", nameof(wheelType));
            }
        }

        public static string GetCarWheelSkinBoneName(WheelPosition wheelType)
        {
            switch (wheelType)
            {
                case WheelPosition.FrontLeft:
                    return BoneFL;
                case WheelPosition.FrontRight:
                    return BoneFR;
                case WheelPosition.RearLeft:
                    return BoneRL;
                case WheelPosition.RearRight:
                    return BoneRR;
                default:
                    throw new ArgumentException($"Unknown wheel type: {wheelType}", nameof(wheelType));
            }
        }
    }
}

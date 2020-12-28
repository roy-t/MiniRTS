using System;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace MiniEngine.Gui
{
    public static class ImageUtilities
    {
        public static Vector2 FitToBounds(float sourceWidth, float sourceHeight, float boundsWidth, float boundsHeight)
        {
            var widthRatio = boundsWidth / sourceWidth;
            var heightRatio = boundsHeight / sourceHeight;

            var ratio = Math.Min(widthRatio, heightRatio);
            return new Vector2(sourceWidth * ratio, sourceHeight * ratio);
        }

        public static Vector2 GetWindowSize()
        {
            var width = ImGui.GetWindowWidth();
            var height = ImGui.GetWindowHeight() - (ImGui.GetFrameHeightWithSpacing() * 2);

            return new Vector2(width, height);
        }
    }
}

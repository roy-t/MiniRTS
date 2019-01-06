using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using Num = System.Numerics;

namespace MiniEngine.UI
{
    public static class Editors
    {
        public static void CreateEditor(string parent, string name, object value, object min, object max, Action<object> setter)
        {
            var label = $"{parent}::{name}";

            if (setter == null)
            {
                ImGui.LabelText(label, value.ToString());
                return;
            }

            switch (value)
            {
                case Vector3 vector:
                    var v = ToNumVector3(vector);                    
                    if (ImGui.SliderFloat3(label, ref v, (float)min, (float)max))
                    {
                        setter(ToXNAVector3(v));
                    }
                    break;

                case Color color:
                    var c = ToNumVector4(color.ToVector4());
                    if (ImGui.ColorEdit4(label, ref c))
                    {
                        setter(new Color(ToXNAVector4(c)));
                    }
                    break;

                case float f:
                    if (ImGui.SliderFloat(label, ref f, (float)min, (float)max))
                    {
                        setter(f);
                    }
                    break;

                default:
                    ImGui.LabelText(label + "*", value.ToString());
                    return;
            }
        }

        private static Num.Vector3 ToNumVector3(Vector3 v) => new Num.Vector3(v.X, v.Y, v.Z);
        private static Vector3 ToXNAVector3(Num.Vector3 v) => new Vector3(v.X, v.Y, v.Z);

        private static Num.Vector4 ToNumVector4(Vector4 v) => new Num.Vector4(v.X, v.Y, v.Z, v.W);
        private static Vector4 ToXNAVector4(Num.Vector4 v) => new Vector4(v.X, v.Y, v.Z, v.W);        
    }
}

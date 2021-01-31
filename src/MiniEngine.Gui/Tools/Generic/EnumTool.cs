using System;
using ImGuiNET;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Generic
{
    [Service]
    public sealed class EnumTool : ATool<Enum>
    {
        public override string Name => "Enum";

        public override bool HeaderValue(ref Enum value, ToolState tool)
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            var names = Enum.GetNames(enumType);
            var index = Array.IndexOf(names, name);

            if (ImGui.Combo(NoLabel, ref index, names, names.Length))
            {
                var values = Enum.GetValues(enumType);
                value = (Enum)values.GetValue(index)!;
                return true;
            }

            return false;
        }
    }
}

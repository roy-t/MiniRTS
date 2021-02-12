using System;

namespace MiniEngine.Gui.Tools
{
    public interface ITool
    {
        string Name { get; }

        int Priority { get; }

        Type TargetType { get; }

        ToolState Configure(ToolState tool);

        bool HeaderValue(ref object value, ToolState tool);

        bool Details(ref object value, ToolState tool, Property propertyPath);
    }
}

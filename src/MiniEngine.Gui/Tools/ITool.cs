using System;

namespace MiniEngine.Gui.Tools
{
    public interface ITool
    {
        string Name { get; }

        Type TargetType { get; }

        ToolState Configure(ToolState tool);
    }
}

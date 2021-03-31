using System;
using System.Collections.Generic;
using System.Linq;
using MiniEngine.Gui.Tools.Generic;

namespace MiniEngine.Gui.Tools
{
    public sealed class ToolSelector
    {
        public record TypedTools(Type Type, string[] Names, ITool[] Tools);

        private readonly IReadOnlyList<ITool> Tools;
        private readonly Dictionary<Type, TypedTools> SpecializedTools;

        public ToolSelector(Tool tool, ObjectTemplater templater, IEnumerable<ITool> tools)
        {
            this.Tools = tools
                .Append(new EnumerableTool(tool))
                .Append(new ObjectTool(templater, tool))
                .ToList();
            this.SpecializedTools = new Dictionary<Type, TypedTools>();
        }

        public TypedTools GetAllTools(Type type)
        {
            if (type.Name.Contains("Transform")) { }
            if (!this.SpecializedTools.TryGetValue(type, out var typedTools))
            {
                var tools = new List<ITool>();

                for (var i = 0; i < this.Tools.Count; i++)
                {
                    var tool = this.Tools[i];
                    if (type.IsAssignableTo(tool.TargetType))
                    {
                        tools.Add(tool);
                    }
                }

                var toolsArray = tools.OrderByDescending(t => t.Priority).ToArray();
                var namesArray = toolsArray.Select(t => t.Name).ToArray();

                typedTools = new TypedTools(type, namesArray, toolsArray);
                this.SpecializedTools[type] = typedTools;
            }

            return typedTools;
        }

        public ITool GetBestTool(Type type, ToolState toolState)
        {
            var tools = this.GetAllTools(type);
            return tools.Tools.FirstOrDefault(t => t.Name == toolState.Name) ?? tools.Tools[0];
        }
    }
}

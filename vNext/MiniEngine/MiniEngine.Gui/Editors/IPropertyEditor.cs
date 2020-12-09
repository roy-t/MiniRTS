using System;
using MiniEngine.Systems;

namespace MiniEngine.Gui
{
    public interface IPropertyEditor
    {
        bool Draw(string name, Func<object, object> get, Action<object, object?> set, AComponent component);

        Type TargetType { get; }
    }
}

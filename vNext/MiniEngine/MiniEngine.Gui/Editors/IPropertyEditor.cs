using System;
using MiniEngine.Systems;

namespace MiniEngine.Gui
{
    public interface IPropertyEditor
    {
        void Draw(string name, Func<object, object> get, Action<object, object?> set, AComponent component);
    }
}

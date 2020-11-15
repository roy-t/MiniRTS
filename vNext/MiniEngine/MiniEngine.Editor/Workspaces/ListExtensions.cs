using System.Collections.Generic;

namespace MiniEngine.Editor.Workspaces
{
    public static class ListExtensions
    {
        public static T Next<T>(this List<T> list, T current)
        {
            var index = list.IndexOf(current);
            var nextIndex = (index + 1) % list.Count;
            return list[nextIndex];
        }
    }
}

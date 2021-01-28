using System;
using System.Linq;

namespace MiniEngine.Gui.Tools
{
    public class Property
    {
        public Property(string path)
        {
            this.Path = path;
            this.Name = this.Path.Split('.', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "";

            var parts = path.Split('.', StringSplitOptions.RemoveEmptyEntries);
            this.SortKey = string.Join('.', parts.Reverse());

            this.Id = new IntPtr(this.Path.GetHashCode());
        }

        public string Name { get; }

        public string SortKey { get; }

        public string Path { get; }

        public IntPtr Id { get; }
    }
}

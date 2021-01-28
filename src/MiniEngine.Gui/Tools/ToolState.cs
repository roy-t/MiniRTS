namespace MiniEngine.Gui.Tools
{
    public struct ToolState
    {
        public ToolState(string name, float x, float y, float z)
        {
            this.Name = name;
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public string Name;
        public float X;
        public float Y;
        public float Z;
    }
}

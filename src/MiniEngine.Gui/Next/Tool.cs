namespace MiniEngine.Gui.Next
{
    public struct Tool
    {
        public Tool(string type, float x, float y, float z)
        {
            this.Type = type;
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public string Type;
        public float X;
        public float Y;
        public float Z;
    }
}

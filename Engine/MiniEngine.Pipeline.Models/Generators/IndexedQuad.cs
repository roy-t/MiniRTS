namespace MiniEngine.Pipeline.Models.Generators
{
    readonly struct IndexedQuad
    {
        public IndexedQuad(int topLeftIndex, int topRightIndex, int bottomRightIndex, int bottomLeftIndex)
        {
            this.TopLeftIndex = topLeftIndex;
            this.TopRightIndex = topRightIndex;
            this.BottomRightIndex = bottomRightIndex;
            this.BottomLeftIndex = bottomLeftIndex;
        }

        public int TopLeftIndex { get; }
        public int TopRightIndex { get; }
        public int BottomRightIndex { get; }
        public int BottomLeftIndex { get; }
    }
}


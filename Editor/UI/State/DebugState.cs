using System.Collections.Generic;

namespace MiniEngine.UI.State
{
    public sealed class DebugState
    {
        public DebugState()
        {
            this.DebugDisplay = DebugDisplay.None;
            this.Columns = 4;
            this.TextureContrast = 1.0f;
            this.SelectedRenderTargets = new List<string>();
        }

        public DebugDisplay DebugDisplay { get; set; }
        public int Columns { get; set; }        
        public bool ShowDemo { get; set; }
        public float TextureContrast { get; set; }

        public List<string> SelectedRenderTargets { get; set; }
        public string SelectedRenderTarget { get; set; }                       
    }
}

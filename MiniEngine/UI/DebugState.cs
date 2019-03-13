using System.Collections.Generic;
using System.Xml.Schema;

namespace MiniEngine.UI
{
    public sealed class DebugState
    {
        public DebugState()
        {
            this.DebugDisplay = DebugDisplay.None;
            this.Columns = 4;

            this.SelectedRenderTargets = new List<string>();
        }

        public DebugDisplay DebugDisplay { get; set; }
        public int Columns { get; set; }        
        public bool ShowDemo { get; set; }

        public List<string> SelectedRenderTargets { get; set; }
        public string SelectedRenderTarget { get; set; }
                
        public XmlSchema GetSchema() => null;
    }
}

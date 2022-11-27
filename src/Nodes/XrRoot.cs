using Godot;
using GodotOnReady.Attributes;

namespace GodotMidasDepth.Nodes; 

public partial class XrRoot : Spatial {
    [OnReadyGet("%XrRig")] ARVROrigin _xrRig = default!;
    
    public void SetViewport(Viewport viewport) {
        _xrRig.Set("viewport", viewport.GetPath());
        _xrRig.Call("initialise");
    }
}
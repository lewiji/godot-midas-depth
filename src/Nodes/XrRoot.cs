using Godot;


namespace GodotMidasDepth.Nodes; 

public partial class XrRoot : Node3D {
    /* "%XrRig" */ [Export] public XROrigin3D XrRig = default!;
    
    public void SetViewport(SubViewport viewport) {
        XrRig.Set("viewport", viewport.GetPath());
        XrRig.Call("initialise");
    }
}
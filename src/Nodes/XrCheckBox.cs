using Godot;
using GodotOnReady.Attributes;

namespace GodotMidasDepth.Nodes; 

public partial class XrCheckBox : CheckBox
{
	[Signal]
	public delegate void XrToggled(bool enabled);
	
	[OnReadyGet("%XrCheckBox")] CheckBox _xrCheckbox = default!;
	
	[OnReady]
	void ConnectSignals() {
		_xrCheckbox.Connect("toggled", this, nameof(OnXrToggled));
	}

	[OnReady]
	void FindXrRuntime() {
		var xrInterface = ARVRServer.FindInterface("OpenXR");
		Disabled = xrInterface == null;
		GD.Print($"XR {(Disabled ? "disabled" : "enabled")}");
	}
	
	void OnXrToggled(bool enabled) {
		EmitSignal(nameof(XrToggled), enabled);
	}

	
}
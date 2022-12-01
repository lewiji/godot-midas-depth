using Godot;


namespace GodotMidasDepth.Nodes; 

public partial class XrCheckBox : CheckBox
{
	[Signal]
	public delegate void XrToggledEventHandler(bool enabled);

	public override void _Ready()
	{
		ConnectSignals();
		FindXrRuntime();
	}

	void ConnectSignals() {
		Toggled += OnXrToggled;
	}

	void FindXrRuntime() {
		var xrInterface = XRServer.FindInterface("OpenXR");
		Disabled = xrInterface == null;
		GD.Print($"XR {(Disabled ? "disabled" : "enabled")}");
	}
	
	void OnXrToggled(bool enabled) {
		EmitSignal(SignalName.XrToggled, enabled);
	}

	
}
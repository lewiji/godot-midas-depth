using Godot;
using System;
using GodotOnReady.Attributes;

public partial class VertexShadedMesh : MarginContainer {
	[OnReadyGet("%Viewport")] Viewport _viewport = default!;
	[OnReady] void ConnectSignals() {
		Connect("visibility_changed", this, nameof(OnVisibilityChanged));
	}

	void OnVisibilityChanged() {
		_viewport.GuiDisableInput = !Visible;
	}
}

using Godot;
using System;


public partial class VertexShadedMesh : MarginContainer {
	/* "%SubViewport" */ [Export] public SubViewport Viewport = default!;
	public override void _Ready()
	{
		Connect("visibility_changed",new Callable(this,nameof(OnVisibilityChanged)));
	}

	void OnVisibilityChanged() {
		Viewport.GuiDisableInput = !Visible;
	}
}

using System.Globalization;
using Godot;


namespace GodotMidasDepth.Nodes; 

public partial class VertexShadedMeshSpatial : Node3D {
	/* "%MeshInstance3D" */ [Export] public MeshInstance3D MeshInstance = default!;
	/* "%MeshGimbal" */ [Export] public Node3D MeshGimbal = default!;
	ShaderMaterial? _shaderMaterial;
	bool _primaryMouseDown;
	Vector2 _relativeMouseMovement;
	Vector2 _mouseSpeed;

	[Export] float _translationSpeed = 5f;
	[Export] float _rotateSpeed = 2f;
	[Export] float _rotateSpeedMouse = 0.2f;

	public override void _Ready()
	{
		_shaderMaterial = (ShaderMaterial)((PlaneMesh)MeshInstance.Mesh).Material;
	}

	public void SetShaderDepth(float value) {
		_shaderMaterial?.SetShaderParameter("depth_scale", value);
	}

	public void SetTextures(Texture2D albedo, Texture2D depth) {
		_shaderMaterial?.SetShaderParameter("albedo_texture", albedo);
		_shaderMaterial?.SetShaderParameter("depth_texture", depth);
		((PlaneMesh) MeshInstance.Mesh).Size = albedo.GetSize() * 0.01f;
	}

	public override void _Process(double delta)
	{
		var dt = (float) delta;
		if (Input.IsActionPressed("rotate_cw")) {
			MeshInstance.GlobalRotate(Vector3.Up, -dt * _rotateSpeed);
		}
		if (Input.IsActionPressed("rotate_ccw")) {
			MeshInstance.GlobalRotate(Vector3.Up, dt * _rotateSpeed);
		}
		if (Input.IsActionPressed("rotate_up")) {
			MeshInstance.RotateObjectLocal(Vector3.Right, -dt * _rotateSpeed);
		}
		if (Input.IsActionPressed("rotate_down")) {
			MeshInstance.RotateObjectLocal(Vector3.Right, dt * _rotateSpeed);
		}
		if (Input.IsActionPressed("strafe_left")) {
			MeshGimbal.Translate(new Vector3(-dt * _translationSpeed, 0, 0));
		}
		if (Input.IsActionPressed("strafe_right")) {
			MeshGimbal.Translate(new Vector3(dt * _translationSpeed, 0, 0));
		}
		if (Input.IsActionPressed("strafe_up")) {
			MeshGimbal.Translate(new Vector3(0, dt * _translationSpeed, 0));
		}
		if (Input.IsActionPressed("strafe_down")) {
			MeshGimbal.Translate(new Vector3(0, -dt * _translationSpeed, 0));
		}
		if (Input.IsActionPressed("translate_forward")) {
			MeshGimbal.Translate(new Vector3(0, 0, -dt * _translationSpeed));
		}
		if (Input.IsActionPressed("translate_back")) {
			MeshGimbal.Translate(new Vector3(0, 0, dt * _translationSpeed));
		}

		if (_relativeMouseMovement != Vector2.Zero) {
			MeshInstance.GlobalRotate(Vector3.Up, _relativeMouseMovement.x * dt * _rotateSpeedMouse);
			MeshInstance.RotateObjectLocal(Vector3.Right, _relativeMouseMovement.y * dt * _rotateSpeedMouse);
			_relativeMouseMovement = Vector2.Zero;
		} else if (_mouseSpeed != Vector2.Zero) {
			MeshInstance.GlobalRotate(Vector3.Up, _mouseSpeed.x * dt * _rotateSpeedMouse);
			MeshInstance.RotateObjectLocal(Vector3.Right, _mouseSpeed.y * dt * _rotateSpeedMouse);
			_mouseSpeed = _mouseSpeed.Lerp(Vector2.Zero, 0.4f);
		}
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseButton {ButtonIndex: MouseButton.Left} primaryMouse) {
			_primaryMouseDown = primaryMouse.Pressed;
		}

		if (_primaryMouseDown && @event is InputEventMouseMotion {Relative: {x: not 0, y: not 0 } } mouseMotion) {
			_relativeMouseMovement = mouseMotion.Relative;
			_mouseSpeed = mouseMotion.Velocity * 0.01f;
		}
	}
}
using System.Globalization;
using Godot;
using GodotOnReady.Attributes;

namespace GodotMidasDepth.Nodes; 

public partial class VertexShadedMeshSpatial : Spatial {
	[OnReadyGet("%MeshInstance")] MeshInstance _meshInstance = default!;
	[OnReadyGet("%MeshGimbal")] Spatial _meshGimbal = default!;
	ShaderMaterial? _shaderMaterial;
	bool _primaryMouseDown;
	Vector2 _relativeMouseMovement;
	Vector2 _mouseSpeed;

	[Export] float _translationSpeed = 5f;
	[Export] float _rotateSpeed = 2f;
	[Export] float _rotateSpeedMouse = 0.2f;

	[OnReady] void ConnectSignals() {
		_shaderMaterial = (ShaderMaterial)((PlaneMesh)_meshInstance.Mesh).Material;
	}

	public void SetShaderDepth(float value) {
		_shaderMaterial?.SetShaderParam("depth_scale", value);
	}

	public void SetTextures(Texture albedo, Texture depth) {
		_shaderMaterial?.SetShaderParam("albedo_texture", albedo);
		_shaderMaterial?.SetShaderParam("depth_texture", depth);
		((PlaneMesh) _meshInstance.Mesh).Size = albedo.GetSize() * 0.01f;
	}

	public override void _Process(float delta) {
		if (Input.IsActionPressed("rotate_cw")) {
			_meshInstance.GlobalRotate(Vector3.Up, -delta * _rotateSpeed);
		}
		if (Input.IsActionPressed("rotate_ccw")) {
			_meshInstance.GlobalRotate(Vector3.Up, delta * _rotateSpeed);
		}
		if (Input.IsActionPressed("rotate_up")) {
			_meshInstance.RotateObjectLocal(Vector3.Right, -delta * _rotateSpeed);
		}
		if (Input.IsActionPressed("rotate_down")) {
			_meshInstance.RotateObjectLocal(Vector3.Right, delta * _rotateSpeed);
		}
		if (Input.IsActionPressed("strafe_left")) {
			_meshGimbal.Translate(new Vector3(-delta * _translationSpeed, 0, 0));
		}
		if (Input.IsActionPressed("strafe_right")) {
			_meshGimbal.Translate(new Vector3(delta * _translationSpeed, 0, 0));
		}
		if (Input.IsActionPressed("strafe_up")) {
			_meshGimbal.Translate(new Vector3(0, delta * _translationSpeed, 0));
		}
		if (Input.IsActionPressed("strafe_down")) {
			_meshGimbal.Translate(new Vector3(0, -delta * _translationSpeed, 0));
		}
		if (Input.IsActionPressed("translate_forward")) {
			_meshGimbal.Translate(new Vector3(0, 0, -delta * _translationSpeed));
		}
		if (Input.IsActionPressed("translate_back")) {
			_meshGimbal.Translate(new Vector3(0, 0, delta * _translationSpeed));
		}

		if (_relativeMouseMovement != Vector2.Zero) {
			_meshInstance.GlobalRotate(Vector3.Up, _relativeMouseMovement.x * delta * _rotateSpeedMouse);
			_meshInstance.RotateObjectLocal(Vector3.Right, _relativeMouseMovement.y * delta * _rotateSpeedMouse);
			_relativeMouseMovement = Vector2.Zero;
		} else if (_mouseSpeed != Vector2.Zero) {
			_meshInstance.GlobalRotate(Vector3.Up, _mouseSpeed.x * delta * _rotateSpeedMouse);
			_meshInstance.RotateObjectLocal(Vector3.Right, _mouseSpeed.y * delta * _rotateSpeedMouse);
			_mouseSpeed = _mouseSpeed.LinearInterpolate(Vector2.Zero, 0.4f);
		}
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseButton {ButtonIndex: (int) ButtonList.Left} primaryMouse) {
			_primaryMouseDown = primaryMouse.Pressed;
		}

		if (_primaryMouseDown && @event is InputEventMouseMotion {Relative: {x: not 0, y: not 0 } } mouseMotion) {
			_relativeMouseMovement = mouseMotion.Relative;
			_mouseSpeed = mouseMotion.Speed * 0.01f;
		}
	}
}
using System.Globalization;
using Godot;
using GodotOnReady.Attributes;

namespace GodotMidasDepth.Nodes; 

public partial class VertexShadedMeshSpatial : Spatial {
	[OnReadyGet("%MeshInstance")] MeshInstance _meshInstance = default!;
	[OnReadyGet("%MeshGimbal")] Spatial _meshGimbal = default!;
	ShaderMaterial? _shaderMaterial;

	[Export] float _translationSpeed = 5f;
	[Export] float _rotateSpeed = 2f;

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

	}
}
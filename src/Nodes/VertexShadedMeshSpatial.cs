using System.Globalization;
using Godot;
using GodotOnReady.Attributes;

namespace GodotMidasDepth.Nodes; 

public partial class VertexShadedMeshSpatial : Spatial {
	[OnReadyGet("%MeshInstance")] MeshInstance _meshInstance = default!;
	[OnReadyGet("%MeshGimbal")] Spatial _meshGimbal = default!;
	[OnReadyGet("%DepthSlider")] Slider _depthSlider = default!;
	[OnReadyGet("%DepthEdit")] LineEdit _depthEdit = default!;
	ShaderMaterial? _shaderMaterial;

	[Export] float _translationSpeed = 5f;
	[Export] float _rotateSpeed = 3f;

	[OnReady] void ConnectSignals() {
		_depthSlider.Connect("value_changed", this, nameof(OnDepthChanged));
		_shaderMaterial = (ShaderMaterial)((PlaneMesh)_meshInstance.Mesh).Material;
		OnDepthChanged((float)_depthSlider.Value);
	}


	void OnDepthChanged(float value) {
		_shaderMaterial?.SetShaderParam("depth_scale", value);
		_depthEdit.Text = value.ToString(CultureInfo.CurrentCulture);
	}
	
	
	public void SetTextures(Texture albedo, Texture depth) {
		_shaderMaterial?.SetShaderParam("albedo_texture", albedo);
		_shaderMaterial?.SetShaderParam("depth_texture", depth);
	}

	public override void _Process(float delta) {
		if (Input.IsActionPressed("rotate_cw")) {
			_meshInstance.RotateY(-delta * _rotateSpeed);
		}
		if (Input.IsActionPressed("rotate_ccw")) {
			_meshInstance.RotateY(delta * _rotateSpeed);
			
		}
		if (Input.IsActionPressed("rotate_up")) {
			_meshInstance.RotateX(-delta * _rotateSpeed);
		}
		if (Input.IsActionPressed("rotate_down")) {
			_meshInstance.RotateX(delta * _rotateSpeed);
			
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

	}
}
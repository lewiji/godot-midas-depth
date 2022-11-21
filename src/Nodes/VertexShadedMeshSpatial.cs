using System.Globalization;
using Godot;
using GodotOnReady.Attributes;

namespace GodotMidasDepth.Nodes; 

public partial class VertexShadedMeshSpatial : Spatial {
	[OnReadyGet("%MeshInstance")] MeshInstance _meshInstance = default!;
	[OnReadyGet("%DepthSlider")] Slider _depthSlider = default!;
	[OnReadyGet("%DepthEdit")] LineEdit _depthEdit = default!;
	ShaderMaterial? _shaderMaterial;

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
}
using System.Globalization;
using Godot;
using Godot.Collections;
using GodotOnReady.Attributes;

namespace GodotMidasDepth.Nodes; 

public partial class PreviewPanel : PanelContainer
{
    [OnReadyGet("%Preview2d/%PreviewTextureRect")] TextureRect _previewTextureRect = default!;
    [OnReadyGet("%Preview2d/%OutputTextureRect")] TextureRect _outputTextureRect = default!;
    [OnReadyGet("%Toolbar/%LoadImageButton")] Button _loadImageButton = default!;
    [OnReadyGet("%Toolbar/%SelectedPathLabel")] Label _pathLabel = default!;
    [OnReadyGet("%VertexShadedMesh/%VertexShadedMeshSpatial")] VertexShadedMeshSpatial _vertexShadedMesh = default!;
    [OnReadyGet("%TabContainer")] TabContainer _tabContainer = default!;
    [OnReadyGet("%Toolbar/%XrCheckBox")] XrCheckBox _xrCheckBox = default!;
    [OnReadyGet("%Toolbar/%DepthSlider")] Slider _depthSlider = default!;
	[OnReadyGet("%Toolbar/%DepthSpinBox")] SpinBox _depthSpinBox = default!;
    
    Image? _image;
    Image? _depth;

    [Signal]
    public delegate void LoadImageRequested();
    
    
    static PackedScene _xrRootScene = GD.Load<PackedScene>("res://src/Nodes/XrRoot.tscn");
    enum XrTargetScene {PointCloud, DepthMap}
    XrRoot? _xrRoot;

    [OnReady]
    void ConnectSignals() {
        _loadImageButton.Connect("pressed", this, nameof(OnLoadImagePressed));
        _xrCheckBox.Connect(nameof(XrCheckBox.XrToggled), this, nameof(OnXrToggled));
        _depthSlider.Connect("value_changed", this, nameof(OnDepthChanged));
        _depthSpinBox.Connect("value_changed", this, nameof(OnDepthSpinBoxChanged));
        OnDepthChanged((float)_depthSlider.Value);
    }

    public override void _Process(float delta) {
	    if (Input.IsActionPressed("depth_increase")) {
		    _depthSlider.Value += _depthSlider.Step / delta;
	    }
	    if (Input.IsActionPressed("depth_decrease")) {
		    _depthSlider.Value -= _depthSlider.Step / delta;
	    }
    }

    void OnDepthChanged(float value) {
	    _depthSpinBox.Value = value;
	    _vertexShadedMesh.CallDeferred(nameof(VertexShadedMeshSpatial.SetShaderDepth), value);
    }

    void OnDepthSpinBoxChanged(float value) {
	    if (!Mathf.IsEqualApprox((float)_depthSlider.Value, value)) {
		    _depthSlider.Value = value;
	    }
    }


    void OnXrToggled(bool enabled) {
        if (enabled && _xrRoot == null) {
            _xrRoot = _xrRootScene.Instance<XrRoot>();
            AddChild(_xrRoot);
        }
        else if (enabled && (_xrRoot != null && !_xrRoot.IsInsideTree())) {
            AddChild(_xrRoot);
        }
        else if (!enabled && (_xrRoot != null && _xrRoot.IsInsideTree())) {
            RemoveChild(_xrRoot);
        }

        if (_xrRoot != null && _xrRoot.IsInsideTree()) {
            _xrRoot.SetViewport(_vertexShadedMesh.GetParent<Viewport>());
        }
    }

    public void SwitchTab(int to) {
	    if (_tabContainer.GetTabCount() > to) {
		    _tabContainer.CurrentTab = to;
	    }
    }

    void OnLoadImagePressed() {
        EmitSignal(nameof(LoadImageRequested));
    }

    public void SetPreviewImage(Image image) {
        _image = image;
        var tex = new ImageTexture();
        tex.Flags = (uint)Texture.FlagsEnum.Filter | (uint)Texture.FlagsEnum.Mipmaps | (uint)Texture.FlagsEnum.AnisotropicFilter;
        tex.CreateFromImage(image, (uint)Texture.FlagsEnum.Filter | (uint)Texture.FlagsEnum.Mipmaps | (uint)Texture.FlagsEnum.AnisotropicFilter);
        _previewTextureRect.Texture = tex;
    }

    public void SetPathLabel(string path) {
        _pathLabel.Text = path;
    }
    
    public void SetResultImage(Image image) {
        _depth = image;
        var tex = new ImageTexture();
        tex.CreateFromImage(image, (uint)Texture.FlagsEnum.Filter);
        _outputTextureRect.Texture = tex;
        ResourceSaver.Save("user://input_tmp.png", tex);
        ResourceSaver.Save("user://output_tmp.png", _previewTextureRect.Texture);
    }

    public void CreateVertexShadedMesh(Image outputImage) {
	    if (_image != null) {
		    ImageTexture tex = new ImageTexture();
		    outputImage.Resize(_image.GetWidth(), _image.GetHeight(), Image.Interpolation.Cubic);
		    tex.CreateFromImage(outputImage, (uint)Texture.FlagsEnum.Filter);
		    _vertexShadedMesh.SetTextures(_previewTextureRect.Texture, tex);
	    }
    }
}
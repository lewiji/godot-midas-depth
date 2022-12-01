using System.Globalization;
using Godot;
using Godot.Collections;


namespace GodotMidasDepth.Nodes; 

public partial class PreviewPanel : PanelContainer
{
    /* "%Preview2d/%PreviewTextureRect" */ [Export] public TextureRect PreviewTextureRect = default!;
    /* "%Preview2d/%OutputTextureRectR" */ [Export] public TextureRect OutputTextureRectR = default!;
    /* "%Toolbar/%LoadImageButton" */ [Export] public Button LoadImageButton = default!;
    /* "%Toolbar/%SelectedPathLabel" */ [Export] public Label PathLabel = default!;
    /* "%VertexShadedMesh/%VertexShadedMeshSpatial" */ [Export] public VertexShadedMeshSpatial VertexShadedMesh = default!;
    /* "%TabContainer" */ [Export] public TabContainer TabContainer = default!;
    /* "%Toolbar/%XrCheckBox" */ [Export] public XrCheckBox XrCheckBox = default!;
    /* "%Toolbar/%DepthSlider" */ [Export] public Slider DepthSlider = default!;
		/* "%Toolbar/%DepthSpinBox" */ [Export] public SpinBox DepthSpinBox = default!;
    
    Image? _image { get; set; }
    Image? _depth { get; set; }

    [Signal]
    public delegate void LoadImageRequestedEventHandler();
    
    static PackedScene _xrRootScene = GD.Load<PackedScene>("res://src/Nodes/XrRoot.tscn");
    XrRoot? _xrRoot;

    public override void _Ready()
    {
	    LoadImageButton.Pressed += OnLoadImagePressed;
	    XrCheckBox.XrToggled += OnXrToggled;
	    DepthSlider.ValueChanged += OnDepthChanged;
	    DepthSpinBox.ValueChanged += OnDepthSpinBoxChanged;
	    OnDepthChanged(DepthSlider.Value);
    }

    public override void _Process(double delta) {
	    if (Input.IsActionPressed("depth_increase")) {
		    DepthSlider.Value += DepthSlider.Step / delta;
	    }
	    if (Input.IsActionPressed("depth_decrease")) {
		    DepthSlider.Value -= DepthSlider.Step / delta;
	    }
    }

    void OnDepthChanged(double value) {
	    DepthSpinBox.Value = value;
	    VertexShadedMesh.CallDeferred(nameof(VertexShadedMeshSpatial.SetShaderDepth), value);
    }

    void OnDepthSpinBoxChanged(double value) {
	    if (!Mathf.IsEqualApprox((float)DepthSlider.Value, (float)value)) {
		    DepthSlider.Value = value;
	    }
    }


    void OnXrToggled(bool enabled) {
        if (enabled && _xrRoot == null) {
            _xrRoot = _xrRootScene.Instantiate<XrRoot>();
            AddChild(_xrRoot);
        }
        else if (enabled && (_xrRoot != null && !_xrRoot.IsInsideTree())) {
            AddChild(_xrRoot);
        }
        else if (!enabled && (_xrRoot != null && _xrRoot.IsInsideTree())) {
            RemoveChild(_xrRoot);
        }

        if (_xrRoot != null && _xrRoot.IsInsideTree()) {
            _xrRoot.SetViewport(VertexShadedMesh.GetParent<SubViewport>());
        }
    }

    public void SwitchTab(int to) {
	    if (TabContainer.GetTabCount() > to) {
		    TabContainer.CurrentTab = to;
	    }
    }

    void OnLoadImagePressed()
    {
	    EmitSignal(SignalName.LoadImageRequested);
    }

    public void SetPreviewImage(Image image) {
        _image = image;
        _image.Reference();
        var tex = ImageTexture.CreateFromImage(image);
        PreviewTextureRect.Texture = tex;
    }

    public void SetPathLabel(string path) {
        PathLabel.Text = path;
    }
    
    public void SetResultImage(Image image) {
        _depth = image;
        var tex = ImageTexture.CreateFromImage(image);
        OutputTextureRectR.Texture = tex;
        ResourceSaver.Save(tex , "user://input_tmp.png");
        ResourceSaver.Save(PreviewTextureRect.Texture, "user://output_tmp.png");
    }

    public void CreateVertexShadedMesh(Image outputImage) {
	    if (_image != null) {
		    outputImage.Resize(_image.GetWidth(), _image.GetHeight(), Image.Interpolation.Lanczos);
		    var tex = ImageTexture.CreateFromImage(outputImage);
		    VertexShadedMesh.SetTextures(PreviewTextureRect.Texture, tex);
	    }
    }
}
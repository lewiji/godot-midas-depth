using System.Globalization;
using Godot;
using Godot.Collections;
using GodotOnReady.Attributes;

namespace GodotMidasDepth.Nodes; 

public partial class PreviewPanel : PanelContainer
{
    [OnReadyGet("%PreviewTextureRect")] TextureRect _previewTextureRect = default!;
    [OnReadyGet("%OutputTextureRect")] TextureRect _outputTextureRect = default!;
    [OnReadyGet("%LoadImageButton")] Button _loadImageButton = default!;
    [OnReadyGet("%ProcessDepthButton")] Button _processDepthButton = default!;
    [OnReadyGet("%Preview3DSpatial")] SpatialPreview _spatialPreviewScene = default!;
    [OnReadyGet("%SelectedPathLabel")] Label _pathLabel = default!;
    [OnReadyGet("%PointCloudPreview")] PreviewPointCloud _pointCloud = default!;
    [OnReadyGet("%TabContainer")] TabContainer _tabContainer = default!;
    
    
    Image? _image;
    Image? _depth;

    [Signal]
    public delegate void LoadImageRequested();
    
    [Signal]
    public delegate void ProcessDepthRequested(Image image);
    
    static PackedScene _xrRootScene = GD.Load<PackedScene>("res://src/Nodes/XrRoot.tscn");
    enum XrTargetScene {PointCloud, DepthMap}
    XrRoot? _xrRoot;

    [OnReady]
    void ConnectSignals() {
        _loadImageButton.Connect("pressed", this, nameof(OnLoadImagePressed));
        _processDepthButton.Connect("pressed", this, nameof(EmitProcessDepthRequest));
        _pointCloud.Connect(nameof(PreviewPointCloud.XrToggled), this, nameof(OnXrToggled), new Array() {XrTargetScene.PointCloud});
        _tabContainer.Connect("tab_changed", this, nameof(OnTabChanged));
    }

    void OnTabChanged(int tab) {
        if (tab == 2) {
            _pointCloud.SetMultiMeshTransforms();
        }
    }

    void OnXrToggled(bool enabled, XrTargetScene targetScene) {
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
            switch (targetScene) {
                case XrTargetScene.PointCloud:
                    _xrRoot.SetViewport(_pointCloud.GetParent<Viewport>());
                    break;
                case XrTargetScene.DepthMap:
                    break;
            }
        }
    }

    void OnLoadImagePressed() {
        EmitSignal(nameof(LoadImageRequested));
    }
    
    void EmitProcessDepthRequest() {
        EmitSignal(nameof(ProcessDepthRequested), _image);
    }

    public void SetPreviewImage(Image image) {
        _image = image;
        var tex = new ImageTexture();
        tex.Flags = (uint)Texture.FlagsEnum.Filter | (uint)Texture.FlagsEnum.Mipmaps | (uint)Texture.FlagsEnum.AnisotropicFilter;
        tex.CreateFromImage(image);
        _previewTextureRect.Texture = tex;
    }

    public void SetPathLabel(string path) {
        _pathLabel.Text = path;
    }
    
    public void SetResultImage(Image image) {
        _depth = image;
        var tex = new ImageTexture();
        tex.Flags = (uint)Texture.FlagsEnum.Filter | (uint)Texture.FlagsEnum.Mipmaps | (uint)Texture.FlagsEnum.AnisotropicFilter;
        tex.CreateFromImage(image);
        _outputTextureRect.Texture = tex;
    }

    public void SetSprite3dImage() {
        _spatialPreviewScene.SetSpriteTexture(_previewTextureRect.Texture, _outputTextureRect.Texture);
    }

    public void CreatePointCloud(float[] depth)
    {
        if (_image != null)
        {
            _pointCloud.SetData(_image, depth);
        }
    }
}
using System.Globalization;
using Godot;
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
    
    Image? _image;
    Image? _depth;

    [Signal]
    public delegate void LoadImageRequested();
    
    [Signal]
    public delegate void ProcessDepthRequested(Image image);

    [OnReady]
    void ConnectSignals() {
        _loadImageButton.Connect("pressed", this, nameof(OnLoadImagePressed));
        _processDepthButton.Connect("pressed", this, nameof(EmitProcessDepthRequest));
    }

    [OnReady]
    void ResetTextures() {
        
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
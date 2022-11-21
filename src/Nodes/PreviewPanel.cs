using System.Globalization;
using Godot;
using GodotOnReady.Attributes;

namespace GodotMidasDepth.Nodes; 

public partial class PreviewPanel : PanelContainer
{
    [OnReadyGet("%Preview2d/%PreviewTextureRect")] TextureRect _previewTextureRect = default!;
    [OnReadyGet("%Preview2d/%OutputTextureRect")] TextureRect _outputTextureRect = default!;
    [OnReadyGet("%Toolbar/%LoadImageButton")] Button _loadImageButton = default!;
    [OnReadyGet("%Toolbar/%ProcessDepthButton")] Button _processDepthButton = default!;
    [OnReadyGet("%Preview3d/%Preview3DSpatial")] SpatialPreview _spatialPreviewScene = default!;
    [OnReadyGet("%Toolbar/%SelectedPathLabel")] Label _pathLabel = default!;
    [OnReadyGet("%PointCloud3d/%PointCloudPreview")] PreviewPointCloud _pointCloud = default!;
    [OnReadyGet("%VertexShadedMesh/%VertexShadedMeshSpatial")] VertexShadedMeshSpatial _vertexShadedMesh = default!;
    
    
    
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
        ResourceSaver.Save("user://input_tmp.png", tex);
        ResourceSaver.Save("user://output_tmp.png", _previewTextureRect.Texture);
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

    public void CreateVertexShadedMesh(Image outputImage) {
	    if (_image != null) {
		    ImageTexture tex = new ImageTexture();
		    tex.CreateFromImage(outputImage);
		    _vertexShadedMesh.SetTextures(_previewTextureRect.Texture, tex);
	    }
    }
}
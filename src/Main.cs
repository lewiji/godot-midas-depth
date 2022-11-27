using Godot;
using GodotMidasDepth.Inference;
using GodotMidasDepth.Nodes;
using GodotOnReady.Attributes;

namespace GodotMidasDepth;

public partial class Main : Node {
    [OnReadyGet("%PreviewPanel")] PreviewPanel _previewPanel = default!;
    [OnReadyGet("%ImagePicker")] ImagePicker _imagePicker = default!;

    public static readonly Config Config = new Config();
    InferImageDepth? _inferImageDepth;
    
    [OnReady]
    void LoadInferenceModel() {
        _inferImageDepth = new InferImageDepth();
        _inferImageDepth.LoadModel();
    }

    [OnReady]
    void ConnectSignals() {
        _previewPanel.Connect(nameof(PreviewPanel.LoadImageRequested), _imagePicker, nameof(ImagePicker.Open));
        _imagePicker.Connect(nameof(ImagePicker.ImageLoaded), _previewPanel, nameof(PreviewPanel.SetPreviewImage));
        _imagePicker.Connect(nameof(ImagePicker.PathSelected), _previewPanel, nameof(PreviewPanel.SetPathLabel));
        _imagePicker.Connect(nameof(ImagePicker.ImageLoaded), this, nameof(ProcessDepth));
        _previewPanel.Connect(nameof(PreviewPanel.ProcessDepthRequested), this, nameof(OnProcessDepthRequested));
    }

    void ProcessDepth(Image image) {
        CallDeferred(nameof(RunModel), image);
    }

    void OnProcessDepthRequested(Image image) {
        CallDeferred(nameof(RunModel), image);
    }

    void RunModel(Image image) {
        var output = _inferImageDepth?.Run(image);
        if (output == null) return;
        _previewPanel.SetResultImage(output);
        _previewPanel.SetSprite3dImage();
        if (_inferImageDepth?.GetDataNormalised() is { } depthData)
        {
            _previewPanel.CreatePointCloud(depthData);
        }
    }
}